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


namespace ChatMoa_DataBaseServer
{
    internal class ChatRoomClass
    {
        internal static async Task<bool> ChatHandlerAsync(NetworkStream ns, byte opcode, List<string> items)  
        {
            bool ans = false;
            string User = items[0];
            string User_Nickname = (await DB_IO.SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == User)).Nickname;
            List<string> path = new List<string>();
            List<int> delete_index = new List<int>();
            List<int> edit_index = new List<int>();
            var list = new List<(int, object)>();           //int >> 0 == Add | 1 == Edit | 2 == Delete


            if (opcode == 32)           //make chat_room    |   items = {User_id, Friend_id_1, Friend_id_2, ... , Friend_id_n}
            {
                //"Chat_Room_List" newChatRoom Add + "Chat_Room__Room_Id__Info.ndjson"  Add(not empty) + All "_User_Id__Inform_Box" ChatInviteNotification Add
                // + "Chat_Room__Room_Id___Date_" Add + "User_Info.ndjson" Chat_Room_List Edit + "Chat_Room__Room_Id__Scheduler" .ndjson Add(can empty)
                string new_room_id = await Create_Room_Id();
                path.Add(@"\DB\Chat_Room_List.ndjson");
                list.Add((0, new Chat_Room_List{
                    Room_Id = new_room_id,
                    Users_Num = 1  
                }));

                path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Info.ndjson");
                path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Info.ndjson");
                list.Add((0, new Chat_Room__Room_Id__Info{  //ChatRoom Program manager Info Add
                    User_Id = "000000",
                    Read_Msg_Num = 0,
                    Read_Last_Date = DateTime.Now.ToString("yyyyMMdd"),
                    Sche_List = "",
                    invite_state = true
                }));
                list.Add((0, new Chat_Room__Room_Id__Info{
                    User_Id = User,
                    Read_Msg_Num = 0,
                    Read_Last_Date = DateTime.Now.ToString("yyyyMMdd"),
                    Sche_List = "",
                    invite_state = true
                }));

                for (int i = 1; i < items.Count(); i++)
                {
                    string friend = items[i];
                    path.Add(@"\DB\Users\" + friend + @"\" + friend + "_Inform_Box.ndjson");
                    list.Add((0, new _User_Id__Inform_Box{
                        Inform_Id = (await DB_IO.LastInformId(path.Last())) + 1,    
                        Inform_Kind = "invite",
                        Inform_Date = DateTime.Now.ToString("yyyyMMdd"),
                        Inform_Str = User_Nickname + "님이 채팅방 초대를 보냈습니다.",
                        need_items = new List<string> { new_room_id },
                        Inform_Checked = false
                    }));
                    path.Add(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Info.ndjson");
                    list.Add((0, new Chat_Room__Room_Id__Info
                    {
                        User_Id = friend,
                        Read_Msg_Num = 0,
                        Read_Last_Date = DateTime.Now.ToString("yyyyMMdd"),
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

                path.Add(@"\DB\ChatRoom\" + new_room_id + @"\" + DateTime.Now.ToString("yyyy") + @"\Chat_Room_" 
                            + new_room_id + "_" + DateTime.Now.ToString("yyyyMMdd"));
                string dir = Path.GetDirectoryName(path.Last());
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                list.Add((0, new Chat_Room__Room_Id___Date_{
                    Msg_Id = 0,
                    User_Id = "000000",
                    Msg_Kind = 0,
                    Date = DateTime.Now.ToString("yyyyMMddHHmmss"),
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
                dir = Path.GetDirectoryName(@"\DB\ChatRoom\"+ new_room_id + @"\Chat_Room_" + new_room_id + "_Scheduler.ndjson");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (!File.Exists(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Scheduler.ndjson"))
                {
                    using (File.Create(@"\DB\ChatRoom\" + new_room_id + @"\Chat_Room_" + new_room_id + "_Scheduler.ndjson"))
                    {
                    }
                }

            }
            else if (opcode == 33)      //invite chat_room  |   itmes = {User_id, Room_id, Friend_id}
            {
                //"_User_Id__Inform_Box" ChatInviteNotification Add + "Chat_Room__Room_Id__Info.ndjson" new user(false) Add
                // + "User_Info.ndjson" friend.Waiting_Chat_Room Edit
                string room_id = items[1];
                string friend = items[2];

                path.Add(@"\DB\Users\" + friend + @"\" + friend + "_Inform_Box.ndjson");
                list.Add((0, new _User_Id__Inform_Box
                {
                    Inform_Id = (await DB_IO.LastInformId(path.Last())) + 1,
                    Inform_Kind = "invite",
                    Inform_Date = DateTime.Now.ToString("yyyyMMdd"),
                    Inform_Str = User_Nickname + "님이 채팅방 초대를 보냈습니다.",
                    need_items = new List<string> { room_id },
                    Inform_Checked = false
                }));
                path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                list.Add((0, new Chat_Room__Room_Id__Info
                {
                    User_Id = friend,
                    Read_Msg_Num = 0,
                    Read_Last_Date = DateTime.Now.ToString("yyyyMMdd"),
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
            }
            else if (opcode == 34)      //exit chat_room    |   itmes = {User_id, Room_id}
            {
                //"Chat_Room_List" Users_Num Edit + "Chat_Room__Room_Id__Info" row Del + "Chat_Room__Room_Id___Date_" exit Msg Add(by admin) + "User_Info" Chat_Room_List Edit
                // if(user is last member)
                //      "Chat_Room_List" row Del + "Chat_Room__Room_Id__Info.ndjson" Del + "Chat_Room__Room_Id__Scheduler.ndjson" Del
                string room_id = items[1];

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
                    Users_Num = (await DB_IO.SearchAsync<Chat_Room_List>(path.Last(),r => r.Room_Id == room_id)).Users_Num - 1
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
                list.Add((2, new Chat_Room__Room_Id__Info{User_Id = User}));

                path.Add(@"\DB\ChatRoom\" + room_id + @"\" + DateTime.Now.ToString("yyyy") + @"\Chat_Room_"
                           + room_id + "_" + DateTime.Now.ToString("yyyyMMdd"));
                string dir = Path.GetDirectoryName(path.Last());
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                list.Add((0, new Chat_Room__Room_Id___Date_
                {
                    Msg_Id = (await LastMsgmId(path.Last())) + 1,
                    User_Id = "000000",
                    Msg_Kind = 0,
                    Date = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Msg_Str = User_Nickname + "님이 채팅방에서 나갔습니다"
                }));

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
            }
            else if (opcode == 35)      //enter chat_room   |   items = {User_id, Room_id, Inform_id}
            {
                //"Chat_Room_List" Users_Num Edit + "Chat_Room__Room_Id__Info" invite_state Edit + "User_Info" Chat_Room_List Edit
                // + "Chat_Room__Room_Id___Date_" enter Msg Add(manager) + "_User_Id__Inform_Box" ChatInviteNotification Del
                string room_id = items[1];

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
                    Read_Msg_Num = 0,
                    Read_Last_Date = DateTime.Now.ToString("yyyyMMdd"),
                    Sche_List = "",
                    invite_state = true
                }));

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

                path.Add(@"\DB\ChatRoom\" + room_id + @"\" + DateTime.Now.ToString("yyyy") + @"\Chat_Room_"
                           + room_id + "_" + DateTime.Now.ToString("yyyyMMdd"));
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
            }
            else if (opcode == 36)      //write chat        |   itmes = {User_id, Room_id, Chat_msg}
            {
                //"Chat_Room__Room_Id__Info" row(manager, user) Edit + "Chat_Room__Room_Id___Date_" Msg Add(by user)
                string room_id = items[1];
                string today = DateTime.Now.ToString("yyyyMMddHHmmss");

                path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                Chat_Room__Room_Id__Info manager_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(path.Last(),r => r.User_Id == "000000");
                manager_info.Read_Msg_Num++;
                manager_info.Read_Last_Date = today;
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
                user_info.Read_Last_Date = today;
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

                path.Add(@"\DB\ChatRoom\" + room_id + @"\" + DateTime.Now.ToString("yyyy") + @"\Chat_Room_"
                        + room_id + "_" + DateTime.Now.ToString("yyyyMMdd"));
                string dir = Path.GetDirectoryName(path.Last());
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                list.Add((0, new Chat_Room__Room_Id___Date_
                {
                    Msg_Id = (await LastMsgmId(path.Last())) + 1,
                    User_Id = User,
                    Msg_Kind = 1,
                    Date = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Msg_Str = items[2]
                }));

            }
            else if (opcode == 37)      //read chat         |   itmes = {User_id, Room_id}
            {
                // Sending Msg with SendAckAsync() + "Chat_Room__Room_Id__Info" row Edit +  
                string room_id = items[1];
                string info_path = @"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson";
                int last_msg_index = (await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == "000000")).Read_Msg_Num;
                Chat_Room__Room_Id__Info user_info = await DB_IO.SearchAsync<Chat_Room__Room_Id__Info>(info_path, r => r.User_Id == User);
                int read_user_msg_index = user_info.Read_Msg_Num;
                string read_user_msg_date = user_info.Read_Last_Date;
                string msg_path = @"\DB\ChatRoom\" + room_id + @"\" + read_user_msg_date + @"\Chat_Room_"
                        + room_id + "_" + read_user_msg_date;

                while (last_msg_index != read_user_msg_index)
                {
                    read_user_msg_index++;
                    Chat_Room__Room_Id___Date_ new_msg = await DB_IO.SearchAsync<Chat_Room__Room_Id___Date_>(msg_path, r => r.Msg_Id == read_user_msg_index);
                    while(new_msg == default)
                    {
                        DateTime dt = DateTime.ParseExact(
                            read_user_msg_date,
                            "yyyyMMdd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None
                        );
                        DateTime next = dt.AddDays(1);
                        read_user_msg_date = next.ToString("yyyyMMdd");

                        msg_path = @"\DB\ChatRoom\" + room_id + @"\" + read_user_msg_date + @"\Chat_Room_"
                        + room_id + "_" + read_user_msg_date;
                        new_msg = await DB_IO.SearchAsync<Chat_Room__Room_Id___Date_>(msg_path, r => r.Msg_Id == read_user_msg_index);
                    }

                    int[] send_lengths = new int[5];
                    //read_msg_opcode는 고정적으로 앞에 1byte를 차지함
                    send_lengths[0] = 1;    //new_msg.Msg_Id
                    send_lengths[1] = new_msg.User_Id.Length;
                    send_lengths[2] = 1;    //new_msg.Msg_Kind
                    send_lengths[3] = new_msg.Date.Length;
                    send_lengths[4] = new_msg.Msg_Str.Length;

                    byte[] msg_id = BitConverter.GetBytes(new_msg.Msg_Id);
                    byte[] user_id = Encoding.UTF8.GetBytes(new_msg.User_Id);
                    byte[] msg_kind = BitConverter.GetBytes(new_msg.Msg_Kind);
                    byte[] date = Encoding.UTF8.GetBytes(new_msg.Date);
                    byte[] msg_str = Encoding.UTF8.GetBytes(new_msg.Msg_Str);
                    if (BitConverter.IsLittleEndian) Array.Reverse(msg_id);
                    if (BitConverter.IsLittleEndian) Array.Reverse(msg_kind);

                    int total_len = 1;
                    int now_index = 0;
                    foreach (int l in send_lengths)
                        total_len += l;

                    byte[] packet = new byte[total_len];

                    packet[0] = opcode; now_index += 1;
                    Buffer.BlockCopy(msg_id, 0, packet, now_index, send_lengths[0]); now_index += send_lengths[0];
                    Buffer.BlockCopy(user_id, 0, packet, now_index, send_lengths[1]); now_index += send_lengths[1];
                    Buffer.BlockCopy(msg_kind, 0, packet, now_index, send_lengths[2]); now_index += send_lengths[2];
                    Buffer.BlockCopy(date, 0, packet, now_index, send_lengths[3]); now_index += send_lengths[3];
                    Buffer.BlockCopy(msg_str, 0, packet, now_index, send_lengths[4]);

                    try
                    {
                        await ns.WriteAsync(packet, 0, packet.Length)
                            .ConfigureAwait(false);
                    }
                    catch(Exception e)
                    {
                        //error
                        read_user_msg_index--;
                        break;
                    }

                    try
                    {
                        byte[] send_success = new byte[1];
                        await DB_IO.ReadExact(ns, send_success, 1);
                    }
                    catch (Exception e)
                    {
                        //error
                        read_user_msg_index--;
                        break;
                    }
                }

                path.Add(info_path);
                user_info.Read_Msg_Num = read_user_msg_index;
                user_info.Read_Last_Date = read_user_msg_date;
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
            else
            {
                // 향후 업데이트로 opcode가 추가된다면 추가로 코딩
            }

            IEnumerable<(object, string)> Add_temp = new List<(object, string)>();
            List<string> Edit_str = new List<string>();
            List<object> Edit_obj = new List<object>();
            List<int> Edit_int = new List<int>();
            List<string> Del_str = new List<string>();
            List<int> Del_int = new List<int>();
            int d_i = 0, e_i = 0;
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == 0)
                    Add_temp.Append((list[i].Item2, path[i]));
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
