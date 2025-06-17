using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace ChatMoa_DataBaseServer
{
    internal class ChatRoomClass
    {
        internal static async Task<bool> ChatHandlerAsync(NetworkStream ns, byte opcode, List<string> items, List<string> send_datas)
        {
            bool ans = false;
            string User = items[0];
            string User_Nickname = (await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User)).Nickname;
            List<string> path = new List<string>();
            List<int> delete_index = new List<int>();
            List<int> edit_index = new List<int>();
            var list = new List<(int, object)>();           //int >> 0 == Add | 1 == Edit | 2 == Delete

            if (opcode == 32)           //make chat_room    |   items = {User_id, Friend_id_1, Friend_id_2, ... , Friend_id_n}      | test success
            {
                //"Chat_Room_List" newChatRoom Add + "Chat_Room__Room_Id__Info.ndjson"  Add(not empty) + All "_User_Id__Inform_Box" ChatInviteNotification Add
                // + "Chat_Room__Room_Id___Date_" Add + "User_Info.ndjson" Chat_Room_List Edit + "Chat_Room__Room_Id__Scheduler" .ndjson Add(can empty)
                string new_room_id = await Create_Room_Id();
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");

                try
                {
                    Console.WriteLine(new_room_id);     //debug
                    path.Add(@"\DB\Chat_Room_List.ndjson");
                    list.Add((0, new Chat_Room_List
                    {
                        Room_Id = new_room_id,
                        Users_Num = 1
                    }));

                    path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Info.ndjson");
                    path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Info.ndjson");
                    list.Add((0, new Chat_Room__Room_Id__Info
                    {  //ChatRoom Program manager Info Add
                        User_Id = "000000",
                        Read_Msg_Num = 0,
                        Read_Last_Date = today.Substring(0, 8),
                        Sche_List = "",
                        invite_state = true
                    }));
                    list.Add((0, new Chat_Room__Room_Id__Info
                    {
                        User_Id = User,
                        Read_Msg_Num = 0,
                        Read_Last_Date = today.Substring(0, 8),
                        Sche_List = "",
                        invite_state = true
                    }));

                    for (int i = 1; i < items.Count(); i++)
                    {
                        string friend = items[i];
                        path.Add(@"\DB\Users\" + friend + @"\" + friend + "_Inform_Box.ndjson");
                        list.Add((0, new _User_Id__Inform_Box
                        {
                            Inform_Id = (await DB_IO.LastInformId(path.Last())) + 1,
                            Inform_Kind = "invite",
                            Inform_Date = today.Substring(0, 8),
                            Inform_Str = User_Nickname + "님이 채팅방 초대를 보냈습니다.",
                            need_items = new List<string> { new_room_id },
                            Inform_Checked = false
                        }));
                        path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Info.ndjson");
                        list.Add((0, new Chat_Room__Room_Id__Info
                        {
                            User_Id = friend,
                            Read_Msg_Num = 0,
                            Read_Last_Date = today.Substring(0, 8),
                            Sche_List = "",
                            invite_state = false
                        }));
                        User_Info friend_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == friend);
                        List<string> friend_info_list = friend_info.Waiting_Chat_Room_List;
                        friend_info_list.Add(new_room_id);
                        friend_info.Waiting_Chat_Room_List = friend_info_list;
                        path.Add(@"\DB\User_Info.ndjson");
                        using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                        {
                            int index = 0;
                            var ser = new DataContractJsonSerializer(typeof(User_Info));
                            string line;
                            while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                            {
                                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                {
                                    var row = (User_Info)ser.ReadObject(ms);
                                    if (row.User_Id == friend)
                                    {
                                        edit_index.Add(index);
                                        break;
                                    }
                                }
                                index++;
                            }
                        }
                        list.Add((1, friend_info));
                    }

                    path.Add(@"\DB\ChatRoom\" + new_room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                                + new_room_id + "_" + today.Substring(0, 8) + ".ndjson");
                    string dir = Path.GetDirectoryName(path.Last());
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    list.Add((0, new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = 0,
                        User_Id = "000000",
                        Msg_Kind = 0,
                        Date = today,
                        Msg_Str = "Created a ChatRoom"
                    }));

                    User_Info user_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User);
                    List<string> user_info_list = user_info.Chat_Room_List;
                    user_info_list.Add(new_room_id);
                    user_info.Chat_Room_List = user_info_list;
                    path.Add(@"\DB\User_Info.ndjson");
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(User_Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (User_Info)ser.ReadObject(ms);
                                if (row.User_Id == User)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, user_info));

                    //"Chat_Room__Room_Id__Scheduler.ndjson" empty file create
                    dir = Path.GetDirectoryName(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Scheduler.ndjson");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    if (!File.Exists(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Scheduler.ndjson"))
                    {
                        using (File.Create(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Scheduler.ndjson"))
                        {
                        }
                    }
                    send_datas.Add(new_room_id);
                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
                    send_datas.Add("0");
                }
            }
            // 수정된 opcode 33: 채팅방 초대 → 즉시 참여
            else if (opcode == 33)      //invite and join chat_room  |   items = {User_id, Room_id, Friend_id}
            {
                string room_id = items[1];
                string friend = items[2];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");

                try
                {
                    // 1. 친구를 채팅방에 즉시 추가 (invite_state = true)
                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                    list.Add((0, new Chat_Room__Room_Id__Info
                    {
                        User_Id = friend,
                        Read_Msg_Num = 0,
                        Read_Last_Date = today.Substring(0, 8),
                        Sche_List = "",
                        invite_state = true  // 즉시 활성화
                    }));

                    // 2. 친구의 채팅방 목록에 직접 추가
                    User_Info friend_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == friend);
                    if (friend_info.Chat_Room_List == null)
                        friend_info.Chat_Room_List = new List<string>();

                    if (!friend_info.Chat_Room_List.Contains(room_id))
                        friend_info.Chat_Room_List.Add(room_id);

                    path.Add(@"\DB\User_Info.ndjson");
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(User_Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (User_Info)ser.ReadObject(ms);
                                if (row.User_Id == friend)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, friend_info));

                    // 3. 채팅방 인원 수 증가
                    path.Add(@"\DB\Chat_Room_List.ndjson");
                    Chat_Room_List room_list = await DB_IO.SearchAsync<Chat_Room_List>(path.Last(), r => r.Room_Id == room_id);
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room_List));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room_List)ser.ReadObject(ms);
                                if (row.Room_Id == room_id)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, new Chat_Room_List
                    {
                        Room_Id = room_id,
                        Users_Num = room_list.Users_Num + 1
                    }));

                    // 4. 입장 메시지 추가
                    string msg_file_path = @"\DB\ChatRoom\" + room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                               + room_id + "_" + today.Substring(0, 8) + ".ndjson";
                    string dir = Path.GetDirectoryName(msg_file_path);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    int new_msg_id = 1;
                    try
                    {
                        int last_msg_id = await LastMsgmId(msg_file_path);
                        new_msg_id = last_msg_id + 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting last message ID: {ex.Message}");
                    }

                    path.Add(msg_file_path);
                    list.Add((0, new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = new_msg_id,
                        User_Id = "000000",
                        Msg_Kind = 0,
                        Date = today,
                        Msg_Str = $"{User_Nickname}님이 {(await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == friend)).Nickname}님을 초대했습니다"
                    }));

                    // 5. 친구에게 알림 (선택사항 - 단순 정보성)
                    path.Add(@"\DB\Users\" + friend + @"\" + friend + "_Inform_Box.ndjson");
                    list.Add((0, new _User_Id__Inform_Box
                    {
                        Inform_Id = (await DB_IO.LastInformId(path.Last())) + 1,
                        Inform_Kind = "info",  // 수락/거절이 아닌 단순 정보
                        Inform_Date = today.Substring(0, 8),
                        Inform_Str = $"{User_Nickname}님이 채팅방에 초대했습니다.",
                        need_items = new List<string> { room_id },
                        Inform_Checked = false
                    }));

                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"opcode 33 error: {e.Message}");
                    send_datas.Add("0");
                }
            }
            else if (opcode == 34)      //exit chat_room    |   itmes = {User_id, Room_id}      | test success
            {
                //"Chat_Room_List" Users_Num Edit + "Chat_Room__Room_Id__Info" row Del + "Chat_Room__Room_Id___Date_" exit Msg Add(by admin) + "User_Info" Chat_Room_List Edit
                // if(user is last member)
                //      "Chat_Room_List" row Del + "Chat_Room__Room_Id__Info.ndjson" Del + "Chat_Room__Room_Id__Scheduler.ndjson" Del
                string room_id = items[1];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");

                try
                {
                    path.Add(@"\DB\Chat_Room_List.ndjson");
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room_List));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room_List)ser.ReadObject(ms);
                                if (row.Room_Id == room_id)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, new Chat_Room_List
                    {
                        Room_Id = room_id,
                        Users_Num = (await DB_IO.SearchAsync<Chat_Room_List>(path.Last(), r => r.Room_Id == room_id)).Users_Num - 1
                    }));

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                if (row.User_Id == User)
                                {
                                    delete_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((2, null));

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                    Chat_Room__Room_Id__Info manager_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(path.Last(), r => r.User_Id == "000000");
                    manager_info.Read_Msg_Num++;
                    manager_info.Read_Last_Date = today.Substring(0, 8);
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                if (row.User_Id == "000000")
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, manager_info));

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                               + room_id + "_" + today.Substring(0, 8) + ".ndjson");
                    string dir = Path.GetDirectoryName(path.Last());
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    list.Add((0, new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = (await LastMsgmId(path.Last())) + 1,
                        User_Id = "000000",
                        Msg_Kind = 0,
                        Date = today,
                        Msg_Str = User_Nickname + "님이 채팅방에서 나갔습니다"
                    }));
                    Console.WriteLine("진입");
                    User_Info user_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User);
                    List<string> user_info_list = user_info.Chat_Room_List;
                    user_info_list.Remove(room_id);
                    user_info.Chat_Room_List = user_info_list;
                    path.Add(@"\DB\User_Info.ndjson");
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(User_Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (User_Info)ser.ReadObject(ms);
                                if (row.User_Id == User)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, user_info));
                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
                    send_datas.Add("0");
                }
            }
            else if (opcode == 35)      //enter chat_room   |   items = {User_id, Room_id, Inform_id}       | test success
            {
                //"Chat_Room_List" Users_Num Edit + "Chat_Room__Room_Id__Info" invite_state Edit + "User_Info" Chat_Room_List Edit
                // + "Chat_Room__Room_Id___Date_" enter Msg Add(manager) + "_User_Id__Inform_Box" ChatInviteNotification Del
                string room_id = items[1];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");
                string info_path = @"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson";

                try
                {
                    // 채팅방 정보 파일 존재 확인
                    if (!File.Exists(info_path))
                    {
                        Console.WriteLine($"Chat room info file not found: {info_path}");
                        send_datas.Add("0");
                        return false;
                    }

                    // 관리자 정보 null 체크
                    Chat_Room__Room_Id__Info manager_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == "000000");
                    if (manager_info == null)
                    {
                        Console.WriteLine("Manager info not found in chat room");
                        send_datas.Add("0");
                        return false;
                    }

                    // 채팅방 목록 업데이트
                    path.Add(@"\DB\Chat_Room_List.ndjson");
                    Chat_Room_List room_list = await DB_IO.SearchAsync<Chat_Room_List>(path.Last(), r => r.Room_Id == room_id);
                    if (room_list == null)
                    {
                        Console.WriteLine($"Chat room {room_id} not found in room list");
                        send_datas.Add("0");
                        return false;
                    }

                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room_List));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room_List)ser.ReadObject(ms);
                                if (row != null && row.Room_Id == room_id)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, new Chat_Room_List
                    {
                        Room_Id = room_id,
                        Users_Num = room_list.Users_Num + 1
                    }));

                    // 사용자 채팅방 정보 업데이트 (invite_state를 true로)
                    path.Add(info_path);
                    Chat_Room__Room_Id__Info user_room_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == User);

                    if (user_room_info == null)
                    {
                        Console.WriteLine($"User room info not found for user {User}");
                        send_datas.Add("0");
                        return false;
                    }

                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                if (row != null && row.User_Id == User)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, new Chat_Room__Room_Id__Info
                    {
                        User_Id = User,
                        Read_Msg_Num = manager_info.Read_Msg_Num,
                        Read_Last_Date = manager_info.Read_Last_Date,
                        Sche_List = user_room_info.Sche_List ?? "",
                        invite_state = true
                    }));

                    // 관리자 정보 업데이트
                    path.Add(info_path);
                    manager_info.Read_Msg_Num++;
                    manager_info.Read_Last_Date = today.Substring(0, 8);
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                if (row != null && row.User_Id == "000000")
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, manager_info));

                    // 사용자 정보 업데이트 (채팅방 목록에 추가, 대기 목록에서 제거)
                    User_Info user_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User);
                    if (user_info == null)
                    {
                        Console.WriteLine($"User info not found for user {User}");
                        send_datas.Add("0");
                        return false;
                    }

                    if (user_info.Chat_Room_List == null)
                        user_info.Chat_Room_List = new List<string>();
                    if (user_info.Waiting_Chat_Room_List == null)
                        user_info.Waiting_Chat_Room_List = new List<string>();

                    if (!user_info.Chat_Room_List.Contains(room_id))
                        user_info.Chat_Room_List.Add(room_id);

                    if (user_info.Waiting_Chat_Room_List.Contains(room_id))
                        user_info.Waiting_Chat_Room_List.Remove(room_id);

                    path.Add(@"\DB\User_Info.ndjson");
                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(User_Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (User_Info)ser.ReadObject(ms);
                                if (row != null && row.User_Id == User)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, user_info));

                    // 입장 메시지 추가
                    string msg_file_path = @"\DB\ChatRoom\" + room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                               + room_id + "_" + today.Substring(0, 8) + ".ndjson";
                    string dir = Path.GetDirectoryName(msg_file_path);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    // 새 메시지 ID 안전하게 생성
                    int new_msg_id = 1;
                    try
                    {
                        int last_msg_id = await LastMsgmId(msg_file_path);
                        new_msg_id = last_msg_id + 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting last message ID: {ex.Message}");
                    }

                    path.Add(msg_file_path);
                    list.Add((0, new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = new_msg_id,
                        User_Id = "000000",
                        Msg_Kind = 0,
                        Date = today,
                        Msg_Str = User_Nickname + "님이 채팅방에 입장했습니다"
                    }));

                    // 알림 삭제
                    string inform_path = @"\DB\Users\" + User + @"\" + User + "_Inform_Box.ndjson";
                    if (File.Exists(inform_path))
                    {
                        path.Add(inform_path);
                        using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                        {
                            int i = 0;
                            var ser = new DataContractJsonSerializer(typeof(_User_Id__Inform_Box));
                            string line;
                            while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                            {
                                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                {
                                    var row = (_User_Id__Inform_Box)ser.ReadObject(ms);
                                    if (row != null && row.Inform_Id == Int32.Parse(items[2]))
                                    {
                                        delete_index.Add(i);
                                        break;
                                    }
                                }
                                i++;
                            }
                        }
                        list.Add((2, null));
                    }

                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
                    Console.WriteLine($"opcode 35 error: {e.Message}");
                    Console.WriteLine($"Stack trace: {e.StackTrace}");
                    send_datas.Add("0");
                }
            }
            else if (opcode == 36)      //write chat        |   itmes = {User_id, Room_id, Chat_Category ,Chat_msg}        | test success
            {
                //"Chat_Room__Room_Id__Info" row(manager, user) Edit + "Chat_Room__Room_Id___Date_" Msg Add(by user)
                string room_id = items[1];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");
                string info_path = @"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson";

                try
                {
                    // 채팅방 정보 파일 존재 확인
                    if (!File.Exists(info_path))
                    {
                        Console.WriteLine($"Chat room info file not found: {info_path}");
                        send_datas.Add("0");
                        return false;
                    }

                    // 관리자 정보 null 체크 추가
                    Chat_Room__Room_Id__Info manager_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == "000000");
                    if (manager_info == null)
                    {
                        Console.WriteLine("Manager info not found in chat room");
                        send_datas.Add("0");
                        return false;
                    }

                    // 사용자 정보 null 체크 추가
                    Chat_Room__Room_Id__Info user_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == User);
                    if (user_info == null)
                    {
                        Console.WriteLine($"User info not found for user: {User}");
                        send_datas.Add("0");
                        return false;
                    }

                    // 사용자가 채팅방에 실제로 참여중인지 확인
                    if (!user_info.invite_state)
                    {
                        Console.WriteLine($"User {User} is not active in chat room {room_id}");
                        send_datas.Add("0");
                        return false;
                    }

                    // 관리자 정보 업데이트
                    path.Add(info_path);
                    manager_info.Read_Msg_Num++;
                    manager_info.Read_Last_Date = today.Substring(0, 8);

                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                if (row != null && row.User_Id == "000000")
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, manager_info));

                    // 사용자 정보 업데이트
                    path.Add(info_path);
                    user_info.Read_Msg_Num++;
                    user_info.Read_Last_Date = today.Substring(0, 8);

                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                    {
                        int i = 0;
                        var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                        string line;
                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                if (row != null && row.User_Id == User)
                                {
                                    edit_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((1, user_info));

                    // 메시지 저장 경로 생성
                    string msg_file_path = @"\DB\ChatRoom\" + room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                            + room_id + "_" + today.Substring(0, 8) + ".ndjson";

                    string dir = Path.GetDirectoryName(msg_file_path);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    // 새 메시지 ID 생성 (안전하게)
                    int new_msg_id = 1;
                    try
                    {
                        int last_msg_id = await LastMsgmId(msg_file_path);
                        new_msg_id = last_msg_id + 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting last message ID: {ex.Message}");
                        // 파일이 없거나 오류가 있으면 1부터 시작
                    }

                    // 새 메시지 추가
                    path.Add(msg_file_path);
                    var new_message = new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = new_msg_id,
                        User_Id = User,
                        Msg_Kind = int.Parse(items[2]),
                        Date = today,
                        Msg_Str = items[3]
                    };
                    list.Add((0, new_message));

                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
                    Console.WriteLine($"opcode 36 error: {e.Message}");
                    Console.WriteLine($"Stack trace: {e.StackTrace}");
                    send_datas.Add("0");
                }
            }
            else if (opcode == 37)      //read chat         |   itmes = {User_id, Room_id, Last_read_msg_id, Last_read_msg_date}          | test success
            {
                // Sending Msg with SendAckAsync() + "Chat_Room__Room_Id__Info" row Edit +  
                string room_id = items[1];
                string info_path = @"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson";
                //이미 읽었던 기록을 바탕으로 받아온다면 이어서 읽기(Last_read_msg_id != -1) / 기록이 너무 오래되었거나, 새로운 환경에서 읽는 경우라면 가장 마지막 메세지를 기준으로 3일 전부터 읽어오기(-1)
                try
                {
                    // 관리자 정보 null 체크 추가
                    Chat_Room__Room_Id__Info manager_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == "000000");
                    if (manager_info == null)
                    {
                        send_datas.Add("0");
                        return false;
                    }

                    int last_msg_index = manager_info.Read_Msg_Num;

                    // 사용자 정보 null 체크 추가
                    Chat_Room__Room_Id__Info user_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == User);
                    if (user_info == null)
                    {
                        send_datas.Add("0");
                        return false;
                    }

                    int read_user_msg_index = user_info.Read_Msg_Num;
                    string read_user_msg_date = user_info.Read_Last_Date?.Substring(0, 8) ?? DateTime.Now.ToString("yyyyMMdd");

                    bool update = false;
                    int new_last_read_index = -1;
                    string new_last_read_date = "";

                    if (int.Parse(items[2]) == -1)          //Last_read_msg_date를 포함하여, 2일전까지 읽기
                    {
                        read_user_msg_date = items[3];
                        List<DateTime> three_days = new List<DateTime>();
                        int temp = 0;

                        DateTime dt = DateTime.ParseExact(
                                read_user_msg_date,
                                "yyyyMMdd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None
                            );
                        three_days.Add(dt.AddDays(-2));
                        three_days.Add(dt.AddDays(-1));
                        three_days.Add(dt);

                        foreach (DateTime d in three_days)
                        {
                            string p = @"\DB\ChatRoom\" + room_id + @"\" + d.ToString("yyyy") + @"\Chat_Room_"
                                + room_id + "_" + d.ToString("yyyyMMdd") + ".ndjson";
                            if (File.Exists(p))
                            {
                                new_last_read_date = d.ToString("yyyyMMdd");
                                send_datas.Add(new_last_read_date);
                                using (var src = new StreamReader(p, Encoding.UTF8))
                                {
                                    var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id___Date_));
                                    string line;
                                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                                    {
                                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                        {
                                            var row = (Chat_Room__Room_Id___Date_)ser.ReadObject(ms);
                                            if (row != null)
                                            {
                                                send_datas.Add(line);
                                                new_last_read_index = row.Msg_Id;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                temp++;
                            }
                        }

                        if (temp == 3)          //3일치 간 chatting 없음
                        {
                            new_last_read_index = last_msg_index;
                            send_datas.Add("00000000");
                        }
                        else
                        {
                            if (user_info.Read_Msg_Num < new_last_read_index)
                            {
                                user_info.Read_Msg_Num = new_last_read_index;
                                user_info.Read_Last_Date = new_last_read_date;
                                update = true;
                            }
                        }
                    }
                    else
                    {
                        if (int.Parse(read_user_msg_date) == int.Parse(items[3]))
                        {
                            if (read_user_msg_index > int.Parse(items[2]))
                                read_user_msg_index = int.Parse(items[2]);
                            else if (last_msg_index == int.Parse(items[2]))
                            {
                                new_last_read_index = last_msg_index;
                                send_datas.Add("00000000");
                            }
                            else if (last_msg_index < int.Parse(items[2]))
                                throw new Exception();
                        }
                        else if (int.Parse(read_user_msg_date) > int.Parse(items[3]))
                        {
                            read_user_msg_date = items[3];
                            read_user_msg_index = int.Parse(items[2]);
                        }
                    }

                    if (!update)
                    {
                        string msg_path = @"\DB\ChatRoom\" + room_id + @"\" + read_user_msg_date.Substring(0, 4) + @"\Chat_Room_"
                            + room_id + "_" + read_user_msg_date + ".ndjson";
                        read_user_msg_index++;

                        while (last_msg_index >= read_user_msg_index)
                        {
                            if (File.Exists(msg_path))
                            {
                                send_datas.Add(read_user_msg_date);
                                new_last_read_date = read_user_msg_date;
                                using (var src = new StreamReader(msg_path, Encoding.UTF8))
                                {
                                    var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id___Date_));
                                    string line;
                                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                                    {
                                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                        {
                                            var row = (Chat_Room__Room_Id___Date_)ser.ReadObject(ms);
                                            if (row != null && row.Msg_Id == read_user_msg_index)
                                            {
                                                send_datas.Add(line);
                                                read_user_msg_index++;
                                                new_last_read_index = row.Msg_Id;
                                            }
                                        }
                                    }
                                }
                                if (new_last_read_index > user_info.Read_Msg_Num)
                                {
                                    update = true;
                                    user_info.Read_Msg_Num = new_last_read_index;
                                    user_info.Read_Last_Date = new_last_read_date;
                                }
                            }

                            DateTime dt = DateTime.ParseExact(
                                    read_user_msg_date,
                                    "yyyyMMdd",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None
                                );
                            DateTime next = dt.AddDays(1);
                            read_user_msg_date = next.ToString("yyyyMMdd");

                            msg_path = @"\DB\ChatRoom\" + room_id + @"\" + read_user_msg_date.Substring(0, 4) + @"\Chat_Room_"
                            + room_id + "_" + read_user_msg_date + ".ndjson";
                        }
                    }

                    if (update)
                    {
                        path.Add(info_path);
                        using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                        {
                            int i = 0;
                            var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                            string line;
                            while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                            {
                                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                {
                                    var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                    if (row != null && row.User_Id == User)
                                    {
                                        edit_index.Add(i);
                                        break;
                                    }
                                }
                                i++;
                            }
                        }
                        list.Add((1, user_info));
                    }

                    if (last_msg_index == new_last_read_index)
                    {
                        send_datas.Add("1");
                        if (edit_index.Count == 0)
                            ans = true;
                    }
                    else
                        send_datas.Add("0");
                }
                catch (Exception e)
                {
                    //error
                    Console.WriteLine($"opcode 37 error: {e.Message}");
                    send_datas.Add("0");
                }
            }
            else if (opcode == 38)      //read my_Chat_Room_List        |   items = {User_id}
            {
                try
                {
                    if (!File.Exists(@"\DB\User_Info.ndjson"))
                    {
                        send_datas.Add("0");
                    }
                    else
                    {
                        User_Info user_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User);
                        if (user_info == default)
                            send_datas.Add("0");
                        else
                        {
                            // 참여 중인 모든 채팅방 목록 (구분자 없이 순수 Room ID만)
                            List<string> chat_room_list = user_info.Chat_Room_List ?? new List<string>();
                            foreach (string room in chat_room_list)
                            {
                                send_datas.Add(room);
                            }

                            send_datas.Add("1");
                        }
                    }
                    ans = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"opcode 38 error: {e.Message}");
                    send_datas = new List<string>() { "0" };
                }
            }
            else if (opcode == 39)      //read Chat_Room_Member_List        |   itmes = {User_id, Room_id}
            {
                string room_id = items[1];

                try
                {
                    if (!File.Exists(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson"))
                    {
                        send_datas.Add("0");
                    }
                    else
                    {
                        using (var src = new StreamReader(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson", Encoding.UTF8))
                        {
                            var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Info));
                            string line;
                            while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                            {
                                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                {
                                    var row = (Chat_Room__Room_Id__Info)ser.ReadObject(ms);
                                    if (row.invite_state == true)
                                    {
                                        send_datas.Add(line);
                                    }
                                }
                            }
                        }
                        send_datas.Add("1");
                        ans = true;
                    }
                }
                catch (Exception e)
                {
                    send_datas = new List<string>() { "0" };
                }
            }
            else
            {
                // 향후 업데이트로 opcode가 추가된다면 추가로 코딩
            }

            var Add_temp = new List<(object, string)>();
            List<string> Edit_str = new List<string>();
            List<object> Edit_obj = new List<object>();
            List<int> Edit_int = new List<int>();
            List<string> Del_str = new List<string>();
            List<int> Del_int = new List<int>();
            int d_i = 0, e_i = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == 0)
                    Add_temp.Add((list[i].Item2, path[i]));
                else if (list[i].Item1 == 1)
                {
                    Edit_str.Add(path[i]);
                    Edit_obj.Add(list[i].Item2);
                    Edit_int.Add(edit_index[e_i++]);
                }
                else if (list[i].Item1 == 2)
                {
                    Del_str.Add(path[i]);
                    Del_int.Add(delete_index[d_i++]);
                }
                else
                    return false;
            }

            if (Add_temp.Count() != 0)
            {
                ans = await DB_IO.SafeBatchAppendAsync(Add_temp);
                if (!ans)
                {
                    //error
                    return false;
                }
            }
            if (Edit_str.Count() != 0)
            {
                ans = await DB_IO.SafeBatchEditAsync(Edit_str, Edit_obj, Edit_int);
                if (!ans)
                {
                    //error
                    return false;
                }
            }
            if (Del_str.Count() != 0)
            {
                ans = await DB_IO.SafeBatchDeleteAsync(Del_str, Del_int);
                if (!ans)
                {
                    //error
                    return false;
                }
            }

            return ans;
        }

        public async static Task<string> Create_Room_Id()      //complete
        {
            string result = "";
            bool exist = true;
            Random rand = new Random();

            while (exist)
            {
                result = rand.Next(0, 1000000).ToString().PadLeft(8, '0');
                exist = await ExistRoomAsync(result);
            }

            return result;
        }

        private static async Task<bool> ExistRoomAsync(string room_id)
        {
            var ser = new DataContractJsonSerializer(typeof(Chat_Room_List));
            bool exist = false;

            string dir = Path.GetDirectoryName(@"\DB\Chat_Room_List.ndjson");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(@"\DB\Chat_Room_List.ndjson"))
            {
                using (File.Create(@"\DB\Chat_Room_List.ndjson"))
                {
                }
            }

            using (var reader = new StreamReader(@"\DB\Chat_Room_List.ndjson", Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null && !exist)
                {
                    // NDJSON → 객체 역직렬화
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var row = (Chat_Room_List)ser.ReadObject(ms);
                        if (row.Room_Id == room_id)
                            exist = true;
                    }
                }
            }

            return exist;
        }

        internal static async Task<Int32> LastMsgmId(string path)
        {
            Int32 ans = -1;

            try
            {
                // 파일이 존재하지 않으면 -1 반환 (새 파일의 경우)
                if (!File.Exists(path))
                {
                    return ans;
                }

                var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id___Date_));

                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        // 빈 줄 무시
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        try
                        {
                            // NDJSON → 객체 역직렬화
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                            {
                                var row = (Chat_Room__Room_Id___Date_)ser.ReadObject(ms);
                                if (row != null && row.Msg_Id > ans)
                                    ans = row.Msg_Id;
                            }
                        }
                        catch (Exception lineEx)
                        {
                            // 개별 라인 파싱 오류는 로그만 남기고 계속 진행
                            Console.WriteLine($"Error parsing line in {path}: {lineEx.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LastMsgmId for path {path}: {ex.Message}");
                // 오류 발생 시 -1 반환
            }

            return ans;
        }
    }
}