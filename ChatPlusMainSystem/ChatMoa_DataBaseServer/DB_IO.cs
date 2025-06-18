using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Services;
using System.Windows.Forms;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.ConstrainedExecution;
using System.Data;
using System.Security.Cryptography;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.IO.Ports;

// idea : TCP 요청을 받아올 때, opcode를 지정 / opcode 별로 지정된 dictionary or POCO 객체 생성
// ==> 이를 Data_Add,Edit,Del 에게 넘겨서 저장후 반환
// 1. 1차적으로 Client로부터 요청이 들어오면 들어온 buffer를 알맞게 파싱
// 2. 파싱한 opcode로부터 알맞은 Add, Edit, Del 호출
// 3. Data를 최종 처리 후 결과(success or fail)를 다시 Client에게 반환
// (3-1. 추후 Client 쪽 모듈에서 일정 시간안에 회신이 오지 않은 경우, 실패로 간주)

namespace ChatMoa_DataBaseServer
{
    [DataContract]
    class LogText
    {
        [DataMember] public string ts;
        [DataMember] public string user;
        [DataMember] public string msg;
    }
    internal class DB_IO
    {
        private const int Port = 5000;            // 열 포트 번호
        private const int HeaderSize = 1 + 6 + 1;               // [0] = opcode, [1..6] = User_Id(string, Big-Endian), [7] = num of request contents
                                                                // private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<TcpClient, bool> _clients = new ConcurrentDictionary<TcpClient, bool>();
        private static string[] used_User_Id = new string[100];
        private static int used_User_Id_Index = 0;

        internal static async Task RunAsync()
        {
            DB_setup();
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine($"[LISTEN] {Port}");

            while (true)                        // 여러 클라이언트 수락
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleAsync(client));
            }
        }

        private static async void DB_setup()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string dbDir = Path.Combine(basePath, @"\DB");
            Console.WriteLine("DBSETUP");
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
                bool setup1 = false;
                bool setup2 = false;
                string fileName = @"\DB\User_Table.ndjson";
                if (!File.Exists(fileName))
                {
                    using (File.Create(fileName))
                    {
                    }
                    User_Table manager = new User_Table
                    {
                        User_Id = "000000",
                        Id = "m1m2m3m4m5m5m6m7m8m9m10",
                        Password = "mm2mm4mm6mm8mm10",
                        Ps_Question_Index = -1,
                        Ps_Answer = "",
                    };
                    IEnumerable<(object, string)> temp = new List<(object, string)>
                    {
                        (manager, fileName)
                    };
                    setup1 = await SafeBatchAppendAsync(temp);
                }


                fileName = @"\DB\User_Info.ndjson";
                if (!File.Exists(fileName))
                {
                    using (File.Create(fileName))
                    {
                    }
                    User_Info manager_2 = new User_Info
                    {
                        User_Id = "000000",
                        Name = "관리자",
                        Nickname = "관리자",
                        Profile_Image_Path = "",
                        Chat_Room_List = new List<string>(),
                        Waiting_Chat_Room_List = new List<string>(),
                    };
                    IEnumerable<(object, string)> temp = new List<(object, string)>
                    {
                        (manager_2, fileName)
                    };
                    setup2 = await SafeBatchAppendAsync(temp);
                }


                if (!(setup1 && setup2))
                    Console.WriteLine("DBsetup Fail: need to reload");
            }
        }
        public static async Task<bool> ExistUserAsync(string user_id)
        {
            var ser = new DataContractJsonSerializer(typeof(User_Table));
            bool exist = false;

            foreach (string s in used_User_Id)
            {
                if (s == user_id)
                    return true;
            }

            using (var reader = new StreamReader(@"\DB\User_Table.ndjson", Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null && !exist)
                {
                    // NDJSON → 객체 역직렬화
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var row = (User_Table)ser.ReadObject(ms);
                        if (row.User_Id == user_id)
                            exist = true;
                    }
                }
            }

            if (exist)
            {
                used_User_Id[used_User_Id_Index++] = user_id;
                used_User_Id_Index %= 100;
                return true;
            }

            return exist;
        }


        private static async Task HandleAsync(TcpClient client)
        {
            var ep = client.Client.RemoteEndPoint;
            Console.WriteLine($"[JOIN] {ep}");

            try
            {
                using (client)
                using (NetworkStream ns = client.GetStream())
                {
                    byte[] header = new byte[HeaderSize];

                    while (true)                // 한 연결에서 계속 수신
                    {
                        await ReadExact(ns, header, HeaderSize);

                        string user_id = Encoding.ASCII.GetString(header, 1, 6);
                        Console.WriteLine("Access: " + user_id);
                        byte opcode = header[0];

                        // Big-Endian → Int32
                        int bode_num = header[7];

                        var body_lengths = new int[bode_num];

                        for (int i = 0; i < bode_num; i++)
                        {
                            int b = ns.ReadByte();
                            if (b == -1) throw new IOException("EOF");
                            body_lengths[i] = b;
                        }

                        var body = new string[bode_num];

                        for (int i = 0; i < bode_num; i++)
                        {
                            byte[] buf = new byte[body_lengths[i]];
                            await ReadExact(ns, buf, buf.Length);
                            body[i] = Encoding.UTF8.GetString(buf);
                            Console.WriteLine(body[i]);
                        }

                        bool exist_user_id = await ExistUserAsync(user_id);

                        if (!exist_user_id)
                        {
                            Console.WriteLine("존재하지 않는 usercode 입니다");
                            //존재하지 않는 user_id
                            break;
                        }

                        bool opcode_success = false;
                        List<string> items = new List<string>() { user_id };
                        foreach (string s in body)
                            items.Add(s);

                        List<string> send_datas = new List<string>();
                        List<string> path = new List<string>();
                        List<int> delete_index = new List<int>();
                        List<int> edit_index = new List<int>();
                        var list = new List<(int, object)>();           //int >> 0 == Add | 1 == Edit | 2 == Delete

                        if (opcode == 0)            //test              |   items = 
                        {
                            send_datas.Add("1");
                            opcode_success = true;
                        }
                        else if (opcode == 1)       //register user     |   items = (User_id(not use), Id,Ps,Ps_question_index,Ps_ans,Name,Nickname)    | test success
                        {
                            //"User_Table.ndjson" user Add + "User_Info" user Add
                            // + "_User_Id__Inform_Box" and "_User_Id__Friend_List" and "_User_Id__Setting_Info" empty file Add
                            string new_user_id = await Create_User_Id();
                            if (await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.Id == items[1]) != default)
                            {
                                //duplication id
                                opcode_success = true;
                                send_datas.Add("0");
                            }
                            else
                            {
                                User_Table make_user = new User_Table();
                                make_user.User_Id = new_user_id;
                                make_user.Id = items[1];
                                make_user.Password = items[2];
                                make_user.Ps_Question_Index = Int32.Parse(items[3]);
                                make_user.Ps_Answer = items[4];
                                Console.WriteLine("진입");
                                path.Add(@"\DB\User_Table.ndjson");
                                list.Add((0, make_user));

                                User_Info make_user_info = new User_Info();
                                make_user_info.User_Id = new_user_id;
                                make_user_info.Name = items[5];
                                make_user_info.Nickname = items[6];
                                make_user_info.Profile_Image_Path = "";
                                make_user_info.Chat_Room_List = new List<string>();
                                make_user_info.Waiting_Chat_Room_List = new List<string>();
                                path.Add(@"\DB\User_Info.ndjson");
                                list.Add((0, make_user_info));

                                string dir = Path.GetDirectoryName(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Inform_Box.ndjson");
                                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                                if (!File.Exists(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Inform_Box.ndjson"))
                                {
                                    using (File.Create(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Inform_Box.ndjson"))
                                    {
                                    }
                                }
                                dir = Path.GetDirectoryName(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Friend_List.ndjson");
                                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                                if (!File.Exists(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Friend_List.ndjson"))
                                {
                                    using (File.Create(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Friend_List.ndjson"))
                                    {
                                    }
                                }

                                _User_Id__Setting_Info make_user_setting_info1 = new _User_Id__Setting_Info()
                                {
                                    Info_Id = "",
                                    Info_Str = ""
                                };
                                path.Add(@"\DB\Users\" + new_user_id + @"\" + new_user_id + "_Setting_Info.ndjson");
                                list.Add((0, make_user_setting_info1));

                                send_datas.Add("1");
                            }
                        }
                        else if (opcode == 2)       //try login         |   items = (User_id(not use),Id,Ps)        | test success
                        {
                            User_Table user = await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.Id == items[1] && r.Password == items[2]);
                            if (user != default)
                            {
                                //login success
                                User_Info send_info = new User_Info() { User_Id = user.User_Id, Name = (await SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == user.User_Id)).Name };
                                send_datas.Add(SerializeJson(send_info));
                                send_datas.Add("1");
                            }
                            else
                            {
                                //login failure
                                send_datas.Add("0");
                            }

                            opcode_success = true;
                            Console.WriteLine("Login 성공 여부: " + send_datas[0]);
                        }
                        else if (opcode == 3)       //find password     |   items = (User_id(not use),Id,Ps_index,Ps_ans,New_ps)        | test success
                        {
                            //if info same, "User_Table" Password Edit
                            User_Table user = await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.Id == items[1]);
                            if (user == default)
                            {
                                //not found id
                                opcode_success = true;
                                send_datas.Add("0");
                                Console.WriteLine("not found id");
                            }
                            else
                            {
                                if (user.Ps_Question_Index == Int32.Parse(items[2]) && user.Ps_Answer == items[3])
                                {
                                    //Ps_question success
                                    user.Password = items[4];
                                    path.Add(@"\DB\User_Table.ndjson");
                                    using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                                    {
                                        int i = 0;
                                        var ser = new DataContractJsonSerializer(typeof(User_Table));
                                        string line;
                                        while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                                        {
                                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                            {
                                                var row = (User_Table)ser.ReadObject(ms);
                                                if (row.Id == items[1])
                                                {
                                                    edit_index.Add(i);
                                                    break;
                                                }
                                            }
                                            i++;
                                        }
                                    }
                                    list.Add((1, user));
                                    send_datas.Add("1");
                                }
                                else
                                {
                                    //Ps_question failure
                                    opcode_success = true;
                                    send_datas.Add("0");
                                    Console.WriteLine("Ps_question failure");
                                }
                            }
                        }
                        else if (opcode == 4)       //change Nickname   |   items = (User_id, Nickname)     | test success
                        {
                            //
                            User_Info user = await SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == user_id);
                            user.Nickname = items[1];
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
                                        if (row.User_Id == user_id)
                                        {
                                            edit_index.Add(i);
                                            break;
                                        }
                                    }
                                    i++;
                                }
                            }
                            list.Add((1, user));
                            send_datas.Add("1");
                        }
                        else if (opcode == 5)       //change setting    |   items = (User_id, Info_id_1, Info_str_1, Info_id_2, Info_str_2, ... , Info_id_n, Info_str_n)        | test success
                        {
                            for (int i = 1; i <= items.Count / 2; i++)
                            {
                                _User_Id__Setting_Info setting_info = new _User_Id__Setting_Info
                                {
                                    Info_Id = items[2 * i - 1],
                                    Info_Str = items[2 * i]
                                };
                                path.Add(@"\DB\Users\" + user_id + @"\" + user_id + "_Setting_Info.ndjson");
                                using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                                {
                                    int index = 0;
                                    var ser = new DataContractJsonSerializer(typeof(_User_Id__Setting_Info));
                                    string line;
                                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                                    {
                                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                        {
                                            var row = (_User_Id__Setting_Info)ser.ReadObject(ms);
                                            if (row.Info_Id == setting_info.Info_Id)
                                            {
                                                edit_index.Add(index);
                                                Console.WriteLine("Store: " + index);
                                                break;
                                            }
                                        }
                                        index++;
                                    }
                                }
                                list.Add((1, setting_info));
                                send_datas.Add("1");
                            }
                        }
                        else if (opcode == 6)       //friend request    |   items = (User_id, friend_Id)        | test success
                        {

                            string friend = items[1];
                            User_Table friend_info = await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.User_Id == friend);

                            if (friend_info != default)
                            {
                                path.Add(@"\DB\Users\" + friend_info.User_Id + @"\" + friend_info.User_Id + "_Inform_Box.ndjson");
                                list.Add((0, new _User_Id__Inform_Box
                                {
                                    Inform_Id = (await LastInformId(path.Last())) + 1,
                                    Inform_Kind = "friend_request",
                                    Inform_Date = DateTime.Now.ToString("yyyyMMdd"),
                                    Inform_Str = (await SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == user_id)).Nickname + "님이 친구 요청을 보냈습니다.",
                                    need_items = new List<string> { user_id },
                                    Inform_Checked = false
                                }));
                                send_datas.Add("1");
                            }
                            else
                            {
                                opcode_success = true;
                                Console.WriteLine("존재하지 않는 User_ID입니다");
                                send_datas.Add("0");
                            }

                        }
                        else if (opcode == 7)       //friend delete     |   items = (User_id, friend_Id)        | test success
                        {
                            string friend = items[1];
                            User_Table friend_info = await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.User_Id == friend);
                            User_Table my_info = await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.User_Id == user_id);

                            if ((friend_info == default) || (my_info == default))
                            {
                                Console.WriteLine("Read Error");
                                send_datas.Add("0");
                                throw new Exception();
                            }

                            path.Add(@"\DB\Users\" + user_id + @"\" + user_id + "_Friend_List.ndjson");
                            using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                            {
                                int i = 0;
                                var ser = new DataContractJsonSerializer(typeof(_User_Id__Friend_List));
                                string line;
                                while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                                {
                                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                    {
                                        var row = (_User_Id__Friend_List)ser.ReadObject(ms);
                                        if (row.Friend_Id == friend)
                                        {
                                            delete_index.Add(i);
                                            break;
                                        }
                                    }
                                    i++;
                                }
                            }
                            list.Add((2, null));

                            path.Add(@"\DB\Users\" + friend_info.User_Id + @"\" + friend_info.User_Id + "_Friend_List.ndjson");
                            using (var src = new StreamReader(path.Last(), Encoding.UTF8))
                            {
                                int i = 0;
                                var ser = new DataContractJsonSerializer(typeof(_User_Id__Friend_List));
                                string line;
                                while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                                {
                                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                    {
                                        var row = (_User_Id__Friend_List)ser.ReadObject(ms);
                                        if (row.Friend_Id == my_info.User_Id)
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
                        else if (opcode == 8)       //check notify      |   items = (User_id, Inform_id, check_state)           | test success
                                                    //>> check_state == 0 : 취소(or 의미없는 확인) / check_state == 1 : 수락
                        {
                            int inform_id = Int32.Parse(items[1]);
                            if (items[2] == "1")
                            {
                                _User_Id__Inform_Box user_inform_box = await SearchAsync<_User_Id__Inform_Box>(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson",
                                    r => r.Inform_Id == inform_id);
                                path.Add(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson");
                                user_inform_box.Inform_Checked = true;
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
                                            if (row.Inform_Id == inform_id)
                                            {
                                                edit_index.Add(i);
                                                break;
                                            }
                                        }
                                        i++;
                                    }
                                }
                                list.Add((1, user_inform_box));
                                if (user_inform_box.Inform_Kind == "friend_request")
                                {
                                    string friend = user_inform_box.need_items.First();     //User_Id

                                    User_Info friend_info = await SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == friend);
                                    User_Info my_info = await SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == user_id);

                                    path.Add(@"\DB\Users\" + user_id + @"\" + user_id + "_Friend_List.ndjson");
                                    list.Add((0, new _User_Id__Friend_List() { Friend_Id = friend, Nickname = friend_info.Nickname }));

                                    path.Add(@"\DB\Users\" + friend + @"\" + friend + "_Friend_List.ndjson");
                                    list.Add((0, new _User_Id__Friend_List() { Friend_Id = user_id, Nickname = my_info.Nickname }));

                                }
                                else if (user_inform_box.Inform_Kind == "invite")
                                {
                                    string room_id = user_inform_box.need_items.First();
                                    List<string> new_items = new List<string>();
                                    new_items.Add(user_id);
                                    new_items.Add(room_id);
                                    new_items.Add(inform_id.ToString());
                                    opcode_success = await ChatRoomClass.ChatHandlerAsync(ns, 35, new_items, send_datas);
                                }
                            }
                            else if (items[2] == "0")
                            {
                                //not execution
                                _User_Id__Inform_Box user_inform_box = await SearchAsync<_User_Id__Inform_Box>(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson",
                                    r => r.Inform_Id == inform_id);
                                string room_id = user_inform_box.need_items.First();
                                path.Add(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson");
                                user_inform_box.Inform_Checked = true;

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
                                            if (row.Inform_Id == inform_id)
                                            {
                                                edit_index.Add(i);
                                                break;
                                            }
                                        }
                                        i++;
                                    }
                                }
                                list.Add((1, user_inform_box));


                                if (user_inform_box.Inform_Kind == "invite")
                                {
                                    path.Add(@"\DB\ChatRoom\" + room_id + @"\Chat_Room_" + room_id + "_Info.ndjson");
                                    Chat_Room__Room_Id__Info room_info = await SearchAsync<Chat_Room__Room_Id__Info>(path.Last(), r => r.User_Id == user_id);
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
                                                if (row.User_Id == user_id)
                                                {
                                                    delete_index.Add(i);
                                                    break;
                                                }
                                            }
                                            i++;
                                        }
                                    }
                                    list.Add((2, null));

                                    path.Add(@"\DB\User_Info.ndjson");
                                    User_Info user = await SearchAsync<User_Info>(path.Last(), r => r.User_Id == user_id);
                                    List<string> temp_waiting_list = user.Waiting_Chat_Room_List;
                                    temp_waiting_list.Remove(room_id);
                                    user.Waiting_Chat_Room_List = temp_waiting_list;
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
                                                if (row.User_Id == user_id)
                                                {
                                                    edit_index.Add(i);
                                                    break;
                                                }
                                            }
                                            i++;
                                        }
                                    }
                                    list.Add((1, user));
                                }
                            }
                            send_datas.Add("1");
                        }
                        else if (opcode == 9)      //delete notify     |   items = (User_id, inform_id)     | test success
                        {
                            int inform_id = Int32.Parse(items[1]);

                            path.Add(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson");
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
                                        if (row.Inform_Id == inform_id)
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
                        else if (opcode == 10)      //read friend_list     |   items = (User_id)     
                        {
                            try
                            {
                                if (!File.Exists(@"\DB\Users\" + user_id + @"\" + user_id + "_Friend_List.ndjson"))
                                {
                                    send_datas.Add("0");
                                }
                                else
                                {
                                    List<string> f_list = new List<string>();

                                    using (var reader = new StreamReader(@"\DB\Users\" + user_id + @"\" + user_id + "_Friend_List.ndjson", Encoding.UTF8))
                                    {
                                        string line;
                                        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                                        {
                                            f_list.Add(line);
                                        }
                                    }

                                    opcode_success = true;
                                    if (f_list.Count == 0)
                                        send_datas.Add("1");
                                    else
                                        send_datas = f_list;
                                }
                            }
                            catch (Exception e)
                            {
                                send_datas = new List<string>() { "0" };
                            }

                        }
                        else if (opcode == 11)  //read all of notify        |   items = (User_id)     
                        {
                            try
                            {
                                if (!File.Exists(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson"))
                                {
                                    send_datas.Add("0");
                                }
                                else
                                {
                                    List<string> f_list = new List<string>();

                                    using (var reader = new StreamReader(@"\DB\Users\" + user_id + @"\" + user_id + "_Inform_Box.ndjson", Encoding.UTF8))
                                    {
                                        string line;
                                        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                                        {
                                            f_list.Add(line);
                                        }
                                    }

                                    opcode_success = true;
                                    if (f_list.Count == 0)
                                        send_datas.Add("1");
                                    else
                                        send_datas = f_list;
                                }
                            }
                            catch (Exception e)
                            {
                                send_datas = new List<string>() { "0" };
                            }
                        }
                        else if (opcode == 12)  //read all of that user of setting      |   items = (User_id)
                        {
                            try
                            {
                                if (!File.Exists(@"\DB\Users\" + user_id + @"\" + user_id + "_Setting_Info.ndjson"))
                                {
                                    opcode_success = false;
                                    send_datas.Add("0");
                                }
                                else
                                {
                                    List<string> f_list = new List<string>();

                                    using (var reader = new StreamReader(@"\DB\Users\" + user_id + @"\" + user_id + "_Setting_Info.ndjson", Encoding.UTF8))
                                    {
                                        string line;
                                        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                                        {
                                            f_list.Add(line);
                                        }
                                    }

                                    opcode_success = true;
                                    if (f_list.Count == 0)
                                        send_datas.Add("1");
                                    else
                                        send_datas = f_list;
                                }
                            }
                            catch (Exception e)
                            {
                                send_datas = new List<string>() { "0" };
                            }
                        }
                        else if (opcode == 13)  //user_id_search             |   items = (friend_id)
                        {
                            string search_id = items[1];
                            try
                            {
                                User_Table user = await SearchAsync<User_Table>(@"\DB\User_Table.ndjson", r => r.User_Id == search_id);
                                _User_Id__Friend_List send_info = new _User_Id__Friend_List();
                                if (user != default)
                                {
                                    //search success
                                    send_info.Friend_Id = search_id;
                                    send_info.Nickname = (await SearchAsync<User_Info>(@"\DB\User_Info.ndjson", r => r.User_Id == search_id)).Nickname;

                                }

                                send_datas.Add(SerializeJson(send_info));
                                send_datas.Add("1");
                                opcode_success = true;
                                Console.WriteLine("Search 시도 여부: " + send_datas[0]);
                            }
                            catch (Exception e)
                            {
                                send_datas.Add("0");
                            }
                        }
                        else if (opcode >= 32 && opcode < 64)
                        {
                            opcode_success = await ChatRoomClass.ChatHandlerAsync(ns, opcode, items, send_datas);
                        }
                        else if (opcode >= 64)
                        {
                            opcode_success = await SchedulerClass.SchedulerHandlerAsync(ns, opcode, items, send_datas);
                        }
                        else
                        {
                            // 향후 업데이트로 opcode가 추가된다면 추가로 코딩
                        }
                        Console.WriteLine("list: " + list.Count);
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
                            {
                                // error
                                throw new Exception();
                            }
                        }
                        Console.WriteLine("진입");
                        if (Add_temp.Count() != 0)
                        {
                            opcode_success = await DB_IO.SafeBatchAppendAsync(Add_temp);
                            if (!opcode_success)
                            {
                                //error
                                throw new Exception();
                            }
                        }
                        if (Edit_str.Count() != 0)
                        {
                            opcode_success = await DB_IO.SafeBatchEditAsync(Edit_str, Edit_obj, Edit_int);
                            if (!opcode_success)
                            {
                                //error
                                throw new Exception();
                            }
                        }
                        if (Del_str.Count() != 0)
                        {
                            opcode_success = await DB_IO.SafeBatchDeleteAsync(Del_str, Del_int);
                            if (!opcode_success)
                            {
                                //error
                                throw new Exception();
                            }
                        }


                        try
                        {
                            if (opcode_success)
                            {
                                Console.WriteLine("요청 처리 성공");
                                byte[][] datas_buf = send_datas.Select(p => Encoding.UTF8.GetBytes(p)).ToArray();
                                for (int i = 0; i < send_datas.Count; i++)
                                {
                                    byte[] state_buf = new byte[1] { 2 };
                                    if (i == send_datas.Count - 1) state_buf[0] = 1;

                                    await ns.WriteAsync(state_buf, 0, 1)
                                        .ConfigureAwait(false);

                                    int len = datas_buf[i].Length;
                                    byte[] len_buf = new byte[1] { (byte)len };
                                    await ns.WriteAsync(len_buf, 0, 1)
                                        .ConfigureAwait(false);

                                    await ns.WriteAsync(datas_buf[i], 0, len)
                                        .ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                byte[] state_buf = new byte[1] { 0 };

                                await ns.WriteAsync(state_buf, 0, 1)
                                    .ConfigureAwait(false);

                                byte[] len_buf = new byte[1] { 1 };
                                await ns.WriteAsync(len_buf, 0, 1)
                                    .ConfigureAwait(false);

                                byte[] error_buf = Encoding.UTF8.GetBytes("0");
                                await ns.WriteAsync(error_buf, 0, 1)
                                    .ConfigureAwait(false);
                                Console.WriteLine("요청 처리 실패");
                            }
                        }
                        catch (Exception e)
                        {
                            byte[] state_buf = new byte[1] { 0 };

                            await ns.WriteAsync(state_buf, 0, 1)
                                .ConfigureAwait(false);

                            byte[] len_buf = new byte[1] { 1 };
                            await ns.WriteAsync(len_buf, 0, 1)
                                .ConfigureAwait(false);

                            byte[] error_buf = Encoding.UTF8.GetBytes("0");
                            await ns.WriteAsync(error_buf, 0, 1)
                                .ConfigureAwait(false);
                            Console.WriteLine("요청 처리 실패");
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception e) { /* 연결 종료 */ }
            finally
            {
                Console.WriteLine($"[LEFT] {ep}");
            }
        }

        // 정확히 n바이트를 읽을 때까지 대기
        internal static async Task ReadExact(Stream s, byte[] buf, int len)  //complete
        {
            int read = 0;
            while (read < len)
            {
                int n = await s.ReadAsync(buf, read, len - read);
                if (n == 0) throw new IOException("remote closed");
                read += n;
            }
        }

        internal static string SerializeJson(object obj)
        {
            // DataContractJsonSerializer는 '직렬화할 형식'을 생성자에 넘겨야 합니다.
            var ser = new DataContractJsonSerializer(obj.GetType());

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);                       // 객체 → JSON → 메모리스트림
                return Encoding.UTF8.GetString(ms.ToArray());   // UTF-8 문자열로 변환
            }
        }

        internal static async Task SendAckAsync(NetworkStream ns, bool ok)
        {
            byte[] ack = { ok ? (byte)1 : (byte)0 };   // 길이 = 1
            await ns.WriteAsync(ack, 0, 1)             // 비동기 전송
                     .ConfigureAwait(false);
        }
        /* example
        bool saved = await FileUtil.SafeAppendAsync("table.ndjson", row);
        await SendAckAsync(ns, saved);     // ★ 1바이트 응답
        */

        public static async Task<string> Create_User_Id()      //complete
        {
            string result = "";
            bool exist = true;
            Random rand = new Random();

            while (exist)
            {
                result = rand.Next(0, 1000000).ToString().PadLeft(6, '0');
                exist = await ExistUserAsync(result);
            }

            return result;
        }

        internal static async Task<bool> SafeBatchAppendAsync(IEnumerable<(object row, string path)> objList)
        {
            // path, temp, backup 미리 구성
            Dictionary<string, ItemInfo> bak_map = new Dictionary<string, ItemInfo>();
            Dictionary<string, List<object>> items_map = new Dictionary<string, List<object>>();

            foreach (var x in objList)
            {
                List<object> temp = null;
                string dir = Path.GetDirectoryName(x.path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (!File.Exists(x.path))
                {
                    using (File.Create(x.path))
                    {
                        // 생성만 하고 즉시 닫기
                    }
                }

                if (!items_map.TryGetValue(x.path, out temp))
                {
                    bak_map.Add(x.path, new ItemInfo
                    {
                        Path = x.path,
                        Temp = Path.Combine(dir, Guid.NewGuid() + ".tmp"),
                        Backup = Path.ChangeExtension(x.path, ".bak")
                    });
                    items_map.Add(x.path, new List<Object>{
                        (x.row)
                    });
                }
                else
                {
                    temp.Add(x.row);
                    items_map[x.path] = temp;
                }
            }

            var items = new List<ItemInfo>();

            foreach (var x in bak_map)
                items.Add(x.Value);

            // temp 파일들 모두 작성
            foreach (var x in items_map)
            {
                try
                {
                    await WriteTempAsync(x.Key, x.Value, bak_map[x.Key]).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"temp 실패: {ex.Message}");
                    CleanupTemps(items);
                    return false;
                }
            }

            // 순차 Replace, 실패 시 롤백
            var replaced = new List<ItemInfo>();
            try
            {
                foreach (var it in items)
                {
                    File.Replace(it.Temp, it.Path, it.Backup, true);
                    replaced.Add(it);                   // 교체 성공 목록
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Replace 실패: {ex.Message}");
                Rollback(replaced);                     // 이미 바뀐 것 복구
                return false;
            }
            finally
            {
                CleanupTemps(items);                    // temp 잔재 삭제
            }
        }

        internal static async Task<bool> SafeBatchEditAsync(List<string> paths, List<object> obj, List<int> index)
        {
            Dictionary<string, ItemInfo> bak_map = new Dictionary<string, ItemInfo>();
            Dictionary<string, List<(object, int)>> items_map = new Dictionary<string, List<(object, int)>>();
            int k = 0;
            foreach (var p in paths)
            {
                List<(object, int)> temp = null;
                string dir = Path.GetDirectoryName(p);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (!items_map.TryGetValue(p, out temp))
                {
                    bak_map.Add(p, new ItemInfo
                    {
                        Path = p,
                        Temp = Path.Combine(dir, Guid.NewGuid() + ".tmp"),
                        Backup = Path.ChangeExtension(p, ".bak")
                    });
                    items_map.Add(p, new List<(Object, int)>{
                        (obj[k],index[k])
                    });
                }
                else
                {
                    temp.Add((obj[k], index[k]));
                    items_map[p] = temp;
                }
                k++;
            }

            List<ItemInfo> items = new List<ItemInfo>();

            foreach (var x in bak_map)
                items.Add(x.Value);

            foreach (var x in items_map)
            {
                try
                {
                    await WriteTempEditAsync(x.Key, x.Value, bak_map[x.Key]).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    CleanupTemps(items);
                    return false;
                }
            }

            var replaced = new List<ItemInfo>();
            try
            {
                foreach (var it in items)
                {
                    File.Replace(it.Temp, it.Path, it.Backup, true);
                    replaced.Add(it);
                }
                return true;
            }
            catch
            {
                Rollback(replaced);                                     // 이미 교체된 것 복구
                return false;
            }
            finally
            {
                CleanupTemps(items);                                    // temp 잔재 삭제
            }
        }

        internal static async Task<bool> SafeBatchDeleteAsync(List<string> paths, List<int> index)
        {
            Dictionary<string, ItemInfo> bak_map = new Dictionary<string, ItemInfo>();
            Dictionary<string, List<int>> items_map = new Dictionary<string, List<int>>();
            int k = 0;
            Console.WriteLine("Del: ");
            foreach (var p in paths)
            {
                List<int> temp = null;
                string dir = Path.GetDirectoryName(p);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (!items_map.TryGetValue(p, out temp))
                {
                    bak_map.Add(p, new ItemInfo
                    {
                        Path = p,
                        Temp = Path.Combine(dir, Guid.NewGuid() + ".tmp"),
                        Backup = Path.ChangeExtension(p, ".bak")
                    });
                    items_map.Add(p, new List<int>{
                        index[k]
                    });
                }
                else
                {
                    temp.Add(index[k]);
                    items_map[p] = temp;
                }
                k++;
            }
            Console.WriteLine(bak_map.Count);

            List<ItemInfo> items = new List<ItemInfo>();

            foreach (var x in bak_map)
                items.Add(x.Value);

            foreach (var x in items_map)
            {
                try
                {
                    await WriteTempDeleteAsync(x.Key, x.Value, bak_map[x.Key]).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    CleanupTemps(items);
                    return false;
                }
            }


            var replaced = new List<ItemInfo>();
            try
            {
                foreach (var it in items)
                {
                    File.Replace(it.Temp, it.Path, it.Backup, true);
                    replaced.Add(it);
                }
                return true;
            }
            catch
            {
                Rollback(replaced);                                     // 이미 교체된 것 복구
                return false;
            }
            finally
            {
                CleanupTemps(items);                                    // temp 잔재 삭제
            }
        }

        public class ItemInfo
        {
            public object Row;
            public string Path;
            public string Temp;
            public string Backup;
        }

        private static async Task WriteTempAsync(string path, List<object> items, ItemInfo bak_item)
        {
            Console.WriteLine("Add: " + bak_item.Path);
            using (var dest = new StreamWriter(bak_item.Temp, false, Encoding.UTF8))
            {
                if (File.Exists(bak_item.Path))
                {
                    using (var src = new StreamReader(bak_item.Path, Encoding.UTF8))
                        await dest.WriteAsync(await src.ReadToEndAsync());
                }

                foreach (var it in items)
                    await dest.WriteLineAsync(SerializeJson(it));
            }
        }
        private static async Task WriteTempEditAsync(string path, List<(object, int)> items, ItemInfo bak_item)
        {
            // 원본이 없으면 끝
            items.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            if (!File.Exists(bak_item.Path)) return;
            items.Add((null, -1));       // 주어진 Edit이 끝났어도 overflow로 인한 나머지 부분에 대한 복사를 막기 위해

            int solved = 0;

            Console.WriteLine("Edit: " + bak_item.Path);
            int i = 0;

            using (var src = new StreamReader(bak_item.Path, Encoding.UTF8))
            using (var dest = new StreamWriter(bak_item.Temp, false, Encoding.UTF8))
            {
                string line;
                while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    Console.WriteLine("index: " + i + " / " + line);
                    if (i == items[solved].Item2)
                    {
                        Console.WriteLine("Edit 중");
                        await dest.WriteLineAsync(SerializeJson(items[solved].Item1))
                              .ConfigureAwait(false);
                        solved++;
                    }
                    else
                    {
                        await dest.WriteLineAsync(line)
                              .ConfigureAwait(false);       // 유지 대상 → temp에 기록
                    }
                    i++;
                }
            }
            Console.WriteLine("1차 End");
        }
        private static async Task WriteTempDeleteAsync(string path, List<int> items, ItemInfo bak_item)
        {
            items.Sort();
            // 원본이 없으면 끝
            if (!File.Exists(bak_item.Path)) return;
            Console.WriteLine("Del: " + bak_item.Path);
            items.Add(-1);      //Del이 모두 끝났어도 나머지 부분을 Write하기 위해 overflow 억제용으로 -1을 추가
            int i = 0;
            int solved = 0;

            using (var src = new StreamReader(bak_item.Path, Encoding.UTF8))
            using (var dest = new StreamWriter(bak_item.Temp, false, Encoding.UTF8))
            {
                string line;
                while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    if (i++ == items[solved]) { solved++; continue; }

                    await dest.WriteLineAsync(line)
                              .ConfigureAwait(false);       // 유지 대상 → temp에 기록
                }
            }
        }

        private static void Rollback(List<ItemInfo> list)
        {

            foreach (var it in list.ToList())            // 이미 교체된 것만 역순이면 더 안전
            {
                for (int i = 0; i < 5 && list.Count() != 0; i++)
                {
                    if (File.Exists(it.Backup))
                    {
                        try
                        {
                            File.Replace(it.Backup, it.Path, null, true);
                            list.Remove(it);
                        }
                        catch { /* 최후 수단: 로그만 */ }
                    }
                }
            }
        }

        private static void CleanupTemps(List<ItemInfo> list)
        {

            foreach (var it in list)
            {
                for (int i = 0; i < 5 && list.Count() != 0; i++)
                {
                    if (File.Exists(it.Backup))
                    {
                        if (File.Exists(it.Temp))
                            try { File.Delete(it.Temp); } catch { }
                    }
                }
            }

        }

        internal static async Task<T> SearchAsync<T>(string path, Func<T, bool> predicate)
        {
            if (!File.Exists(path)) return default;  // null

            var ser = new DataContractJsonSerializer(typeof(T));

            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var obj = (T)ser.ReadObject(ms);
                        if (predicate(obj))
                            return obj;
                    }
                }
            }
            return default;
        }

        internal static async Task<Int32> LastInformId(string path)
        {
            Int32 ans = -1;
            var ser = new DataContractJsonSerializer(typeof(_User_Id__Inform_Box));


            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // NDJSON → 객체 역직렬화
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var row = (_User_Id__Inform_Box)ser.ReadObject(ms);
                        if (row.Inform_Id > ans)
                            ans = row.Inform_Id;
                    }
                }
            }
            Console.WriteLine("ans: " + ans);
            return ans;
        }
    }
}