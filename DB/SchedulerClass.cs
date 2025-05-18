using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using ChatMoa_DataBaseServer;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.CodeDom;
using System.Net.Sockets;

namespace ChatMoa_DataBaseServer
{
    internal class SchedulerClass
    {
        internal static async Task<bool> SchedulerHandlerAsync(NetworkStream ns, byte opcode, List<string> items)
        {
            bool ans = false;
            string User = items[0];
            int selected_Sche_id = -1;
            string path = "";
            int mode = -1;
            int handle_index = -1;
            List<object> result = new List<object>();

            if (opcode < 67)
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
                    Yearly = items[8]
                });
            }
            else if(opcode == 65)       //user schedule edit        |   items = { User_id, _User_Id__Scheduler.* }      | test success
            {
                //"_User_Id__Scheduler" schedule Edit
                mode = 1;
                int sche_id = Int32.Parse(items[1]);
                result.Add (new _User_Id__Scheduler
                {
                    Sche_Id = sche_id,
                    Category = items[2],
                    Begin_Date = items[3],
                    Finish_Date = items[4],
                    Sche_Str = items[5],
                    Daily = items[6],
                    Weekly = items[7],
                    Monthly = items[8],
                    Yearly = items[9]
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
            }
            else if (opcode == 67)      //chat_room schedule add    |   items = { User_id, Room_id, Chat_Room__Room_Id__Scheduler.* }       | test success(chat_room_info에 개인별 일정 추가 미완)
            {
                //"Chat_Room__Room_Id__Scheduler" schedule Add          
                mode = 0;
                result.Add( new Chat_Room__Room_Id__Scheduler
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
                    Yearly = items[9]           
                });
            }
            else if (opcode == 68)      //chat_room schedule edit   |   items = { User_id, Room_id, Chat_Room__Room_Id__Scheduler.* }       | test success
            {
                //"Chat_Room__Room_Id__Scheduler" schedule Edit
                mode = 1;
                int sche_id = Int32.Parse(items[2]);
                result.Add( new Chat_Room__Room_Id__Scheduler
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
                    Yearly = items[10]
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
            }
            else if (opcode == 69)      //chat_room schedule delete |   items = { User_id, Room_id, Sche_id }           | test success
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
            }
            else
            {
                // 향후 업데이트로 opcode가 추가된다면 추가로 코딩
            }


            if(mode == 0)
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
            else
            {
                // error
                Console.WriteLine(User+": Scheduler Error");
            }
            
            return ans;
        }

        private static async Task<Int32> LastScheId(string path, int mode)
        {
            Int32 ans = -1;
            bool exist = false;

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(path))
            {
                using (File.Create(path))
                {
                }
            }

            if (mode == 0)           //User_Scheduler
            {
                var ser = new DataContractJsonSerializer(typeof(_User_Id__Scheduler));


                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null && !exist)
                    {
                        // NDJSON → 객체 역직렬화
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (_User_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id > ans)
                                ans = row.Sche_Id;
                        }
                    }
                }
            }
            else if(mode == 1)      //Chat_Room_Scheduler
            {
                var ser = new DataContractJsonSerializer(typeof(Chat_Room__Room_Id__Scheduler));


                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null && !exist)
                    {
                        // NDJSON → 객체 역직렬화
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            var row = (Chat_Room__Room_Id__Scheduler)ser.ReadObject(ms);
                            if (row.Sche_Id > ans)
                                ans = row.Sche_Id;
                        }
                    }
                }
            }
            else
            {
                //error
            }

            return ans;
        }
    }
}