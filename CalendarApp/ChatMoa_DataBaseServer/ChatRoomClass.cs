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
                    dir = Path.GetDirectoryName(@"\DB\ChatRoom\" + new_room_id + @"\Image_Info.ndjson");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    if (!File.Exists(@"\DB\ChatRoom\" + new_room_id + @"\Image_Info.ndjson"))
                    {
                        using (File.Create(@"\DB\ChatRoom\" + new_room_id + @"\Image_Info.ndjson"))
                        {
                        }
                    }

                    Image_Info make_image_info = new Image_Info()
                    {
                        Changed_User_Id = User,
                        Changed_Date = "00000000000000"
                    };
                    path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Image_Info.ndjson");
                    list.Add((0, make_image_info));

                    send_datas.Add(new_room_id);
                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
                    send_datas.Add("0");
                }
            }
            else if (opcode == 33)      //invite chat_room  |   itmes = {User_id, Room_id, Friend_id}       | test success
            {
                //"_User_Id__Inform_Box" ChatInviteNotification Add + "Chat_Room__Room_Id__Info.ndjson" new user(false) Add
                // + "User_Info.ndjson" friend.Waiting_Chat_Room Edit
                string room_id = items[1];
                string friend = items[2];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");

                try
                {
                    path.Add(@"\DB\Users\" + friend + @"\" + friend + "_Inform_Box.ndjson");
                    list.Add((0, new _User_Id__Inform_Box
                    {
                        Inform_Id = (await DB_IO.LastInformId(path.Last())) + 1,
                        Inform_Kind = "invite",
                        Inform_Date = today.Substring(0, 8),
                        Inform_Str = User_Nickname + "님이 채팅방 초대를 보냈습니다.",
                        need_items = new List<string> { room_id },
                        Inform_Checked = false
                    }));
                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
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
                    friend_info_list.Add(room_id);
                    friend_info.Waiting_Chat_Room_List = friend_info_list;
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
                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
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

                try
                {
                    Chat_Room__Room_Id__Info manager_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(path.Last(), r => r.User_Id == "000000");
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
                        Users_Num = (await DB_IO.SearchAsync<Chat_Room_List>(path.Last(), r => r.Room_Id == room_id)).Users_Num + 1
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
                        Sche_List = "",
                        invite_state = true
                    }));

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
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

                    User_Info user_info = await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User);
                    List<string> user_info_list = user_info.Chat_Room_List;
                    user_info_list.Add(room_id);
                    user_info.Chat_Room_List = user_info_list;
                    List<string> user_info_waiting_list = user_info.Waiting_Chat_Room_List;
                    user_info_waiting_list.Remove(room_id);
                    user_info.Waiting_Chat_Room_List = user_info_waiting_list;
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

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                               + room_id + "_" + today.Substring(0, 8) + ".ndjson");
                    string dir = Path.GetDirectoryName(path.Last());
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    list.Add((0, new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = (await LastMsgmId(path.Last())) + 1,
                        User_Id = "000000",
                        Msg_Kind = 0,
                        Date = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        Msg_Str = User_Nickname + "님이 채팅방에 입장했습니다"
                    }));

                    path.Add(@"\DB\Users\" + User + @"\" + User + "_Inform_Box.ndjson");
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
                                if (row.Inform_Id == Int32.Parse(items[2]))
                                {
                                    delete_index.Add(i);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    list.Add((2, null));
                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
                    send_datas.Add("0");
                }
            }
            else if (opcode == 36)      //write chat        |   itmes = {User_id, Room_id, Chat_Category ,Chat_msg}        | test success
            {
                //"Chat_Room__Room_Id__Info" row(manager, user) Edit + "Chat_Room__Room_Id___Date_" Msg Add(by user)
                string room_id = items[1];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");

                try
                {
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

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                    Chat_Room__Room_Id__Info user_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(path.Last(), r => r.User_Id == User);
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

                    path.Add(@"\DB\ChatRoom\" + room_id + @"\" + today.Substring(0, 4) + @"\Chat_Room_"
                            + room_id + "_" + today.Substring(0, 8) + ".ndjson");
                    string dir = Path.GetDirectoryName(path.Last());
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    list.Add((0, new Chat_Room__Room_Id___Date_
                    {
                        Msg_Id = (await LastMsgmId(path.Last())) + 1,
                        User_Id = User,
                        Msg_Kind = int.Parse(items[2]),
                        Date = today,
                        Msg_Str = items[3]
                    }));
                    send_datas.Add("1");
                }
                catch (Exception e)
                {
                    //error
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
                    int last_msg_index = (await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == "000000")).Read_Msg_Num;
                    Chat_Room__Room_Id__Info user_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == User);
                    int read_user_msg_index = user_info.Read_Msg_Num;
                    string read_user_msg_date = user_info.Read_Last_Date.Substring(0, 8);

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
                                            send_datas.Add(line);
                                            new_last_read_index = row.Msg_Id;
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
                                            if (row.Msg_Id == read_user_msg_index)
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
                    send_datas.Add("0");
                }
            }
            else if (opcode == 38)      //read my_Chat_Room_List        |   itmes = {User_id}
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
                            List<string> f_list = user_info.Chat_Room_List;
                            foreach (string room in f_list)
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
            if (Edit_str.Count() != 0)
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
            var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id___Date_));


            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // NDJSON → 객체 역직렬화
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var row = (Chat_Room__Room_Id___Date_)ser.ReadObject(ms);
                        if (row.Msg_Id > ans)
                            ans = row.Msg_Id;
                    }
                }
            }

            return ans;
        }
    }
}