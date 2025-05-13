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

/*  ndjson 사용 예시
[DataContract]
internal class Person
{
    [DataMember]
    internal string name;

    [DataMember]
    internal int age;
}

namespace testndjsonandTCP
{
    internal class Program
    {
        public static void Main()
        {
            var p1 = new Person();
            var p2 = new Person();
            var l = new List<Person>();
            p1.name = "John";
            p1.age = 42;
            p2.name = "Bob";
            p2.age = 1;
            l.Add(p1);
            l.Add(p2);

            

            
            
            using (var writer = new StreamWriter("output.ndjson"))
            {
                foreach (var p in l)
                {
                    var ms = new MemoryStream();
                    var ser = new DataContractJsonSerializer(typeof(Person));
                    ser.WriteObject(ms, p);

                    byte[] json = ms.ToArray();
                    ms.Close();

                    string j = Encoding.UTF8.GetString(json, 0, json.Length);

                    Console.WriteLine(j);
                    writer.WriteLine(j);
                }
            }

            using (var reader = new StreamReader("output.ndjson"))
            {
                var ser = new DataContractJsonSerializer(typeof(Person));
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(line);
                    using (var ms = new MemoryStream(bytes))
                    {
                        var p = (Person)ser.ReadObject(ms);

                        Console.Write("한줄: ");
                        Console.WriteLine($"name = {p.name}, age={p.age}");
                    }
                }
            }
        }
    }
}
 */

/*  비동기 사용법
 private static async Task<bool> TryAppendLineAsync(string path, string line, int retry = 3)
{
    for (int i = 0; i < retry; i++)
    {
        try
        {
            await AppendLineAsync(path, line);   // 비동기 버전 (WriteAsync 등 사용)
            return true;                         // 성공
        }
        catch (IOException)
        {
            await Task.Delay(50);                // 백오프 후 재시도
        }
    }
    return false;                                // 최종 실패
}

bool ok = await TryAppendLineAsync(path, jsonLine);
if (!ok) Console.Error.WriteLine("저장 실패!");
//////////////////////////////////////////////////////////////////
//
                        if (kind == 1)            // 텍스트
                        {
                            string text = Encoding.UTF8.GetString(body);
                            var logObj = new Dictionary<string, object>   // ★ 익명 대신 Dictionary
                            {
                                ["ts"] = DateTime.UtcNow.ToString("o"),
                                ["user"] = ep.ToString(),
                                ["msg"] = text
                            };
                            TryAppendLine(@"Logs\chat.ndjson", SerializeJson(logObj));
                        }
                        else if (kind == 2)       // 이미지
                        {\
                            var logObj = new Dictionary<string, object>
                            {
                                ["ts"] = DateTime.UtcNow.ToString("o"),
                                ["user"] = ep.ToString(),
                                ["img"] = "fileName"
                            };
                            TryAppendLine(@"Logs\chat.ndjson", SerializeJson(logObj));
                        }

                        // 필요하다면 _clients.Keys 반복하며 브로드캐스트 가능
 */

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
        private const int HeaderSize = 8;               // [0] = opcode, [1..6] = User_Id(string, Big-Endian)
        private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<TcpClient, bool> _clients = new ConcurrentDictionary<TcpClient, bool>();
        private string[] used_User_Id = new string[100];
        private int used_User_Id_Index = 0;
        public DB_IO()
        {
            _listener = new TcpListener(IPAddress.Any, Port); // Wi-Fi 내 모든 NIC
        }

        private static string SerializeJson(object obj)
        {
            // DataContractJsonSerializer는 '직렬화할 형식'을 생성자에 넘겨야 합니다.
            var ser = new DataContractJsonSerializer(obj.GetType());

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);                       // 객체 → JSON → 메모리스트림
                return Encoding.UTF8.GetString(ms.ToArray());   // UTF-8 문자열로 변환
            }
        }

        public async Task RunAsync(CancellationToken token)
        {
            _listener.Start();
            Console.WriteLine($"[INFO] LISTEN {Port}");

            while (!token.IsCancellationRequested)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _clients[client] = true;
                //HandleClientAsync가 비동기로 돌아가기에 요청이 들어오는 Client마다 Run이 실행되면서 while은 멈추지않고 계속 새로운 Client 요청을 받아와 실행함
                await Task.Run(() => HandleClientAsync(client));     // 백그라운드 처리      
            }
        }

        public string Create_User_Id()
        {
            string result;
            char[] num = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            HashSet<string> exist_User_Ids = new HashSet<string>();
            HashSet<string> new_id = new HashSet<string>();

            using (var reader = new StreamReader("User_Table.ndjson"))
            {
                var ser = new DataContractJsonSerializer(typeof(User_Table));
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    byte[] temp = Encoding.UTF8.GetBytes(line);
                    using (var ms = new MemoryStream(temp))
                    {
                        var user = (User_Table)ser.ReadObject(ms);
                        exist_User_Ids.Add(user.User_Id);
                    }
                }
            }

            do
            {
                new_id.Clear();
                result = "";
                for(int i=0;i<6;i++){
                    Random rand = new Random();
                    result += num[rand.Next(0, 10)];
                }
                new_id.Add(result);
            }
            while (!exist_User_Ids.SetEquals(new_id));

            return result;
        }
        private async Task<bool> Exist_User_Id(string get_id)
        {
            foreach (string s in used_User_Id)
            {
                if (s == get_id)
                    return true;
            }

            bool exist = false;

            using (var reader = new StreamReader("User_Table.ndjson", Encoding.UTF8))
            {
                var ser = new DataContractJsonSerializer(typeof(User_Table));
                string line;

                while ((line = await reader.ReadLineAsync()) != null && !exist)
                {
                    byte[] temp = Encoding.UTF8.GetBytes(line);
                    using (var ms = new MemoryStream(temp))
                    {
                        var user = (User_Table)ser.ReadObject(ms);
                        if (user.User_Id == get_id) exist = true;
                    }
                }
            }
            if (exist)
            {
                used_User_Id[used_User_Id_Index++] = get_id;
                used_User_Id_Index %= 100;
                return true;
            }

            return false;
        }

        private async Task<bool> Exist_Id(string get_id)
        {
            bool exist = false;

            using (var reader = new StreamReader("User_Table.ndjson", Encoding.UTF8))
            {
                var ser = new DataContractJsonSerializer(typeof(User_Table));
                string line;

                while (!exist && (line = await reader.ReadLineAsync()) != null)   // ★ await
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    {
                        var user = (User_Table)ser.ReadObject(ms);
                        if (user.Id == get_id) exist = true;
                    }
                }
            }

            return exist;
        }

        private async Task HandleClientAsync(TcpClient client)      //예시 코드
        {
            EndPoint ep = client.Client.RemoteEndPoint;
            Console.WriteLine($"[JOIN] {ep}");

            try
            {
                
                using (client)
                using (NetworkStream ns = client.GetStream())
                {
                    ns.ReadTimeout = 1200_000;          // 20분 동안 추가 요청 없을 시 Socket을 닫음
                    byte[] header = new byte[HeaderSize];
                    
                    while (true)        // 연결 유지 루프
                    {
                        await ReadExactAsync(ns, header, HeaderSize);

                        List<string> db_path = new List<string>();
                        List<int> data_modes = new List<int>();     // 0: Add, 1: Edit, 2:Delete, 3:Not work -1: Error
                        byte opcode = header[0];
                        string get_user_id = Encoding.ASCII.GetString(header, 1, 6);
                        int num_of_data = header[7];
                        List<int> len_of_data = new List<int>();

                        if (!await Exist_User_Id(get_user_id))
                        {
                            //send error code: not exist User_Id
                            opcode = 0;
                            data_modes.Add(-1);
                        }
                        else
                        {
                            for (int i = 0; i < num_of_data; i++)
                            {
                                byte[] temp = new byte[1];
                                await ReadExactAsync(ns, temp, 1);
                                len_of_data.Add(BitConverter.ToInt32(temp, 1));
                            }
                        }

                        var request_ser = new DataContractJsonSerializer(null);
                        object request_data;
                        List<string> datas = new List<string>();

                        switch (opcode)
                        {
                            case 0:         //test code
                                break;
                            case 1:         //register user(Id,Ps,Ps_question_index,Ps_ans)
                                request_ser = new DataContractJsonSerializer(typeof(User_Table));
                                foreach(int len in len_of_data)
                                {
                                    byte[] temp = new byte[len];
                                    await ReadExactAsync(ns, temp, len);
                                    datas.Add(Encoding.UTF8.GetString(header, 0, len));
                                }
                                if(!await Exist_Id(datas[0]))
                                {
                                    db_path.Add("User_Table.ndjson");

                                    request_data = new User_Table {
                                        User_Id = Create_User_Id(),
                                        Id = datas[0],
                                        Password = datas[1],
                                        Ps_Answer = datas[3],
                                        Ps_Question_Index = Int32.Parse(datas[2])
                                    };
                                    data_modes.Add(0);
                                }
                                else
                                {
                                    data_modes.Add(-1);
                                    //send error code: Id duplication 
                                }

                                break;
                            case 2:         //try login(Id,Ps)
                                request_ser = new DataContractJsonSerializer(typeof(User_Table));
                                foreach (int len in len_of_data)
                                {
                                    byte[] temp = new byte[len];
                                    await ReadExactAsync(ns, temp, len);
                                    datas.Add(Encoding.UTF8.GetString(header, 0, len));
                                }

                                if (await Exist_Id(datas[0]))
                                {
                                    using (var reader = new StreamReader("User_Table.ndjson"))
                                    {
                                        var ser = new DataContractJsonSerializer(typeof(User_Table));
                                        string line;

                                        while ((line = await reader.ReadLineAsync()) != null)
                                        {
                                            byte[] temp = Encoding.UTF8.GetBytes(line);
                                            using (var ms = new MemoryStream(temp))
                                            {
                                                var user = (User_Table)ser.ReadObject(ms);
                                                if (user.Id == datas[0])
                                                {
                                                    if(user.Password == datas[1])
                                                    {
                                                        //login success
                                                        //
                                                        data_modes.Add(3);
                                                    }
                                                    else
                                                    {
                                                        //login failure
                                                        //send Erro: not same ps
                                                        data_modes.Add(-1);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //send Error: not found Id

                                    data_modes.Add(-1);
                                }
                                break;
                            case 3:         //find password(Id,Ps_index,Ps_ans)
                                request_ser = new DataContractJsonSerializer(typeof(User_Table));
                                foreach (int len in len_of_data)
                                {
                                    byte[] temp = new byte[len];
                                    await ReadExactAsync(ns, temp, len);
                                    datas.Add(Encoding.UTF8.GetString(header, 0, len));
                                }

                                if (await Exist_Id(datas[0]))
                                {
                                    using (var reader = new StreamReader("User_Table.ndjson"))
                                    {
                                        var ser = new DataContractJsonSerializer(typeof(User_Table));
                                        string line;

                                        while ((line = await reader.ReadLineAsync()) != null)
                                        {
                                            byte[] temp = Encoding.UTF8.GetBytes(line);
                                            using (var ms = new MemoryStream(temp))
                                            {
                                                var user = (User_Table)ser.ReadObject(ms);
                                                if (user.Id == datas[0])
                                                {
                                                    if (user.Ps_Question_Index == Int32.Parse(datas[1]) && user.Ps_Answer == datas[2])
                                                    {
                                                        //find success
                                                        //
                                                        data_modes.Add(3);
                                                    }
                                                    else
                                                    {
                                                        //find failure
                                                        //send Erro: not same ps
                                                        data_modes.Add(-1);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //send Error: not found Id

                                    data_modes.Add(-1);
                                }

                                break;
                            case 5:         //change Nickname
                                request_ser = new DataContractJsonSerializer(typeof(User_Info));
                                
                                break;
                            case 6:         //change setting
                            case 7:         //friend request
                            case 8:         //friend delete
                            case 9:         //check notification
                            case 10:        //delete notification
                            case 32:        //make chat_room
                            case 33:        //invite chat_room
                            case 34:        //exit chat_room
                            case 35:        //write chat
                            case 36:        //read chat
                            case 64:        //user schedule add
                            case 65:        //user schedule edit
                            case 66:        //user schedule delete
                            case 67:        //chat_room schedule add
                            case 68:        //chat_room schedule edit
                            case 69:        //chat_room schedule delete
                            default:
                                break;
                        }


                        foreach(int data_mode in data_modes)
                        {
                            if (data_mode == 0)     //Add
                            {
                                if (await Data_Add())
                                {
                                    //success
                                }
                                else
                                {
                                    //fail
                                }
                            }
                            else if (data_mode == 1) //Edit
                            {
                                if (await Data_Edit())
                                {
                                    //success
                                }
                                else
                                {
                                    //fail
                                }
                            }
                            else if (data_mode == 2) //Delete
                            {
                                if (await Data_Edit())
                                {
                                    //success
                                }
                                else
                                {
                                    //fail
                                }
                            }
                            else if (data_mode == 3) //Not work
                            {

                            }
                            else                    //Error
                            {

                            }
                            //loop end
                        }
                    }
                }
            }
            catch (IOException) { /* 연결 끊김 */ }
            finally
            {
                _clients.TryRemove(client, out _);
                Console.WriteLine($"[LEFT] {ep}");
            }
        }

        private static async Task ReadExactAsync(Stream s, byte[] buf, int len)
        {
            int read = 0;
            while (read < len)
            {
                int n = await s.ReadAsync(buf, read, len - read);
                if (n == 0) throw new IOException("remote closed");
                read += n;
            }
        }

        private static readonly object _fileLock = new object();

        // NDJSON 한 줄 안전하게 추가
        private static void AppendLine(string path, string line)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            lock (_fileLock)                       // 파일 무결성을 위한 최소 구간 lock
            {
                File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
            }
        }

        bool TryAppendLine(string path, string line, int retry = 3)
        {
            for (int i = 0; i < retry; i++)
            {
                try
                {
                    AppendLine(path, line);   // 내부는 예외 전파
                    return true;              // 성공
                }
                catch (IOException)
                {
                    Thread.Sleep(50);         // 잠시 기다렸다 재시도
                }
            }
            return false;                     // 최종 실패
        }

        // ------- entry point -------
        private static async Task Main()
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("Press Ctrl+C to stop.");
            await new DB_IO().RunAsync(cts.Token);
        }








        private async Task<bool> Data_Add()
        {
            bool ans = false;

            return ans;
        }

        private async Task<bool> Data_Edit()
        {
            bool ans = false;

            return ans;
        }

        private async Task<bool> Data_Del()
        {
            bool ans = false;

            return ans;
        }
    }
}
