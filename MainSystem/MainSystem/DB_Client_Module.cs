using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Json;

//추후 잘못된 opcode에 대해 state_buf를 0으로 반환하도록 수정

namespace MainSystem
{
    internal class DCM
    {
        private string user_id;

        private Dictionary<int, List<string>> received_data;

        internal DCM()
        {
            user_id = "";
            received_data = new Dictionary<int, List<string>>();
        }

        internal async Task<KeyValuePair<bool, (int, List<int>)>> db_request_data(byte opcode, List<string> items)      //Key : request 성공 여부  |  Value.item1 : received_data의 key  |
                                                                                                                        //Value.item2 : 방금 받아온 data(string)들을 저장한 received_data의 Value(List<string>)의 각 index_List
        {
            bool success = false;
            List<int> received_data_index = new List<int>();
            int n = -1;

            using (var client = new TcpClient())
            {
                await client.ConnectAsync("127.0.0.1", 5000);   // 서버 IP·포트
                NetworkStream ns = client.GetStream();

                n = received_data.Count;

                while (received_data.ContainsKey(n)) n++;

                received_data[n] = new List<string>();

                received_data_index = await SendAsync(ns, n, opcode, items);
            }

            if (received_data_index != null)
                success = true;
            else
                Clear_receive_data(n);          //request가 실패 했을 때, 미리 추가해두었던 receive_data의 공간(key : n)을 반환

            return new KeyValuePair<bool, (int, List<int>)>(success, (n, received_data_index));
        }

        private async Task<List<int>> SendAsync(NetworkStream ns, int num, byte opcode, List<string> bodyStr)
        {
            byte[] userBytes = Encoding.ASCII.GetBytes(user_id.PadLeft(6, '0'));
            byte opByte = opcode;
            List<string> parts = bodyStr;
            Encoding utf8 = Encoding.UTF8;
            byte[][] data = parts.Select(p => utf8.GetBytes(p)).ToArray();

            int len = 1 + 6 + 1 + (parts.Count * 1);        // 각 body의 길이 정보를 1byte로 보내기에  "* 1"
            len += data.Sum(b => b.Length);


            byte[] packet = new byte[len];
            int pos = 0;

            packet[pos++] = opcode;

            Buffer.BlockCopy(userBytes, 0, packet, pos, 6);
            pos += 6;

            packet[pos++] = (byte)parts.Count;

            foreach (var b in data)
                packet[pos++] = (byte)b.Length;

            foreach (var b in data)
            {
                Buffer.BlockCopy(b, 0, packet, pos, b.Length);
                pos += b.Length;
            }

            await ns.WriteAsync(packet, 0, packet.Length)
                    .ConfigureAwait(false);

            List<int> result = new List<int>();

            try
            {
                var receive = await ReadAckAsync(ns, num);
                while (receive.Item1 == 2)
                {
                    result.Add(receive.Item2);
                    receive = await ReadAckAsync(ns, num);
                }

                if (receive.Item1 == 1)
                {
                    result.Add(receive.Item2);
                }
                else
                    throw new Exception("Error");
            } catch (Exception e)
            {
                //error
                return null;
            }

            return result;
        }

        private async Task<(int, int)> ReadAckAsync(NetworkStream ns, int num)          // item1: 0 == error | 1 == success + end | 2 == success + additional execution      || item2: receive_data_index
        {
            byte[] state_buf = new byte[1];
            byte[] receive_buf_len = new byte[1];
            byte[] receive_buf;
            int result = -1;
            int n = await ns.ReadAsync(state_buf, 0, 1).ConfigureAwait(false);
            if (state_buf[0] != 0 && n != 0)
            {
                n = await ns.ReadAsync(receive_buf_len, 0, 1).ConfigureAwait(false);
                receive_buf = new byte[receive_buf_len[0]];
                n = await ns.ReadAsync(receive_buf, 0, receive_buf_len[0]).ConfigureAwait(false);

                received_data[num].Add(Encoding.UTF8.GetString(receive_buf));           // login 등 성공여부만을 전달받는 경우에는 >> "1" : 성공 | "0" : 실패
                result = received_data[num].Count - 1;
            }

            return (state_buf[0], result);
        }

        private bool Clear_receive_data(int num) { return received_data.Remove(num); }

        private T DeSerializeJson<T>(int num, int index)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            string s = (received_data[num])[index];

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(s)))
            {
                var obj = (T)ser.ReadObject(ms);
                return obj;
            }
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

        private void Login(string user_id)
        {
            this.user_id = user_id;
        }

        private void Logdout()
        {
            this.user_id = "";
            this.received_data = new Dictionary<int, List<string>>();
        }

        private string my_User_Id(){ return this.user_id; }
    }
}
