using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using ChatMoa_DataBaseServer;
using System.IO;
using System.Runtime.Serialization;
using System.CodeDom;
using System.Net.Sockets;

namespace ChatMoa_DataBaseServer
{
    public class SchedulerClass
    {
        public static async Task<bool> SchedulerHandlerAsync(NetworkStream ns, byte opcode, List<string> items, List<string> send_datas)
        {
            bool ans = false;
            string User = items[0];
            int selected_Sche_id = -1;
            string path = "";
            int mode = -1;
            int handle_index = -1;
            List<object> result = new List<object>();

            if (opcode < 68)
                path = @"\DB\Users\" + User + @"\" + User + "_Scheduler.ndjson";
            else
                path = @"\DB\ChatRoom\" + items[1] + @"\Chat_Room_" + items[1] + "_Scheduler.ndjson";
            if (opcode == 64)            //user schedule add         |   items = { User_id, Category, Begin_Date, Finish_Date, Sche_Str, Daily, Weekly, Monthly, Yearly }   | test success
            {
                //"_User_Id__Scheduler" schedule Add
                mode = 0;
                result.Add(new _User_Id__Scheduler
                {
                    Sche_Id = await LastScheId(path, 0) + 1,
                    Category = items[1],
                    Begin_Date = items[2],
                    Finish_Date = items[3],
                    Sche_Str = items[4],
                    Daily = items[5],
                    Weekly = items[6],
                    Monthly = items[7],
                    Yearly = items[8],

                });
                send_datas.Add("1");
            }
            else if (opcode == 65)       //user schedule edit        |   items = { User_id, _User_Id__Scheduler.* }      | test success
            {
                //"_User_Id__Scheduler" schedule Edit
                mode = 1;
                int sche_id = Int32.Parse(items[1]);
                result.Add(new _User_Id__Scheduler
                {
                    Sche_Id = sche_id,
                    Category = items[2],
                    Begin_Date = items[3],
                    Finish_Date = items[4],
                    Sche_Str = items[5],
                    Daily = items[6],                       
                    Weekly = items[7],                      
                    Monthly = items[8],                    
                    Yearly = items[9],                     

                });
                using (var src = new StreamReader(path, Encoding.UTF8))
                {
                    int i = 0;
                    var ser = new DataContractJsonSerializer(typeof(_User_Id__Scheduler));
                    string line;
                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (_User_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id == sche_id)
                            {
                                handle_index = i;
                                break;
                            }
                        }
                        i++;
                    }
                }
                send_datas.Add("1");
            }
            else if (opcode == 66)      //user schedule delete      |   items = { User_id, Sche_id }        | test success
            {
                //"_User_Id__Scheduler" schedule Del
                mode = 2;
                selected_Sche_id = Int32.Parse(items[1]);
                using (var src = new StreamReader(path, Encoding.UTF8))
                {
                    int i = 0;
                    var ser = new DataContractJsonSerializer(typeof(_User_Id__Scheduler));
                    string line;
                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (_User_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id == selected_Sche_id)
                            {
                                handle_index = i;
                                break;
                            }
                        }
                        i++;
                    }
                }
                send_datas.Add("1");
            }
            else if (opcode == 67)      //read all of user_schedule         |   items = { User_id } 
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        send_datas.Add("0");
                    }
                    else
                    {
                        using (var src = new StreamReader(path, Encoding.UTF8))
                        {
                            string line;
                            while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                            {
                                send_datas.Add(line);
                            }
                        }
                        send_datas.Add("1");
                        mode = 3;
                        ans = true;
                    }
                }
                catch (Exception e)
                {
                    send_datas = new List<string>() { "0" };
                }
            }
            else if (opcode == 68)      //chat_room schedule add    |   items = { User_id, Room_id, Chat_Room__Room_Id__Scheduler.* }       | test success(chat_room_info에 개인별 일정 추가 미완)
            {
                //"Chat_Room__Room_Id__Scheduler" schedule Add          
                mode = 0;
                result.Add(new Chat_Room__Room_Id__Scheduler
                {
                    Sche_Id = await LastScheId(path, 1) + 1,
                    User_Id = User,
                    Category = items[2],
                    Begin_Date = items[3],
                    Finish_Date = items[4],
                    Sche_Str = items[5],
                    Daily = items[6],                     
                    Weekly = items[7],                    
                    Monthly = items[8],                     
                    Yearly = items[9],                      
                });
                send_datas.Add("1");
            }
            else if (opcode == 69)      //chat_room schedule edit   |   items = { User_id, Room_id, Chat_Room__Room_Id__Scheduler.* }       | test success
            {
                //"Chat_Room__Room_Id__Scheduler" schedule Edit
                mode = 1;
                int sche_id = Int32.Parse(items[2]);
                result.Add(new Chat_Room__Room_Id__Scheduler
                {
                    Sche_Id = sche_id,
                    User_Id = User,
                    Category = items[3],
                    Begin_Date = items[4],
                    Finish_Date = items[5],
                    Sche_Str = items[6],
                    Daily = items[7],                       
                    Weekly = items[8],                       
                    Monthly = items[9],                     
                    Yearly = items[10],                      
                });
                using (var src = new StreamReader(path, Encoding.UTF8))
                {
                    int i = 0;
                    var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Scheduler));
                    string line;
                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (Chat_Room__Room_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id == sche_id)
                            {
                                handle_index = i;
                                break;
                            }
                        }
                        i++;
                    }
                }
                send_datas.Add("1");
            }
            else if (opcode == 70)      //chat_room schedule delete |   items = { User_id, Room_id, Sche_id }           | test success
            {
                //"Chat_Room__Room_Id__Scheduler" schedule Del
                mode = 2;
                selected_Sche_id = Int32.Parse(items[2]);
                using (var src = new StreamReader(path, Encoding.UTF8))
                {
                    int i = 0;
                    var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Scheduler));
                    string line;
                    while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (Chat_Room__Room_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id == selected_Sche_id)
                            {
                                handle_index = i;
                                break;
                            }
                        }
                        i++;
                    }
                }
                send_datas.Add("1");
            }
            else if (opcode == 71)      //read all of user_schedule
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        send_datas.Add("0");
                    }
                    else
                    {
                        using (var src = new StreamReader(path, Encoding.UTF8))
                        {
                            string line;
                            while ((line = await src.ReadLineAsync().ConfigureAwait(false)) != null)
                            {
                                send_datas.Add(line);
                            }
                        }
                        send_datas.Add("1");
                        mode = 3;
                        ans = true;
                    }
                }
                catch (Exception e)
                {
                    send_datas = new List<string>() { "0" };
                }
            }
            else        // 채팅방 알림 읽기
            {
                // 향후 업데이트로 opcode가 추가된다면 추가로 코딩
            }


            if (mode == 0)
            {

                IEnumerable<(object, string)> temp = new List<(object, string)>
                {
                    (result[0], path)
                };
                ans = await DB_IO.SafeBatchAppendAsync(temp);
            }
            else if (mode == 1)
            {
                Console.WriteLine("진입");
                ans = await DB_IO.SafeBatchEditAsync(new List<string>() { path }, new List<object>() { result[0] }, new List<int>() { handle_index });
            }
            else if (mode == 2)
            {
                ans = await DB_IO.SafeBatchDeleteAsync(new List<string>() { path }, new List<int>() { handle_index });
            }
            else if (mode == 3)
            {
                return ans;
            }
            else
            {
                // error
                Console.WriteLine(User + ": Scheduler Error");
            }

            return ans;
        }

        private static async Task<Int32> LastScheId(string path, int mode)
        {
            Int32 ans = -1;

            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // 더 안전한 파일 생성 방식: 파일이 없으면 만들고 즉시 닫습니다.
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "", Encoding.UTF8);
                }

                // --- 파일 접근 충돌을 해결하기 위한 재시도 로직 ---
                string[] lines = null;
                for (int i = 0; i < 3; i++) // 최대 3번까지 재시도
                {
                    try
                    {
                        lines = File.ReadAllLines(path, Encoding.UTF8);
                        break; // 파일 읽기 성공 시 루프 탈출
                    }
                    catch (IOException) // 파일이 사용 중일 경우
                    {
                        if (i == 2) throw; // 3번 모두 실패하면 오류 발생시킴
                        await Task.Delay(100); // 0.1초 대기 후 다시 시도
                    }
                }

                if (lines == null) return -1;

                if (mode == 0) // User_Scheduler
                {
                    var ser = new DataContractJsonSerializer(typeof(_User_Id__Scheduler));
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (_User_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id > ans)
                                ans = row.Sche_Id;
                        }
                    }
                }
                else if (mode == 1) // Chat_Room_Scheduler
                {
                    var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Scheduler));
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (Chat_Room__Room_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id > ans)
                                ans = row.Sche_Id;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL ERROR] in LastScheId for path {path}: {ex.ToString()}");
                return -1;
            }
            return ans;
        }
    }
}