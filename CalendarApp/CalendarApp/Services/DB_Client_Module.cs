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

namespace ChatMoa_DataBaseServer
{
    // internal -> public 으로 변경
    public class DCM
    {
        private string user_id;
        private Dictionary<int, List<string>> received_data;

        public DCM()
        {
            user_id = "";
            received_data = new Dictionary<int, List<string>>();
        }

        // internal -> public 으로 변경
        public async Task<KeyValuePair<bool, (int, List<int>)>> db_request_data(byte opcode, List<string> items)
        {
            bool success = false;
            List<int> received_data_index = new List<int>();
            int n = -1;

            using (var client = new TcpClient())
            {
                // IP 주소는 실제 서버 환경에 맞게 변경해야 할 수 있습니다.
                await client.ConnectAsync("127.0.0.1", 5000);
                NetworkStream ns = client.GetStream();

                n = received_data.Count;
                while (received_data.ContainsKey(n)) n++;

                received_data[n] = new List<string>();
                received_data_index = await SendAsync(ns, n, opcode, items);
            }

            if (received_data_index != null && received_data_index.Count > 0)
                success = true;
            else
                Clear_receive_data(n);

            return new KeyValuePair<bool, (int, List<int>)>(success, (n, received_data_index));
        }

        private async Task<List<int>> SendAsync(NetworkStream ns, int num, byte opcode, List<string> bodyStr)
        {
            byte[] userBytes = Encoding.ASCII.GetBytes(user_id.PadLeft(6, '0'));
            List<string> parts = bodyStr;
            Encoding utf8 = Encoding.UTF8;
            byte[][] data = parts.Select(p => utf8.GetBytes(p)).ToArray();

            int len = 1 + 6 + 1 + (parts.Count * 1);
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

            await ns.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);

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
                else if (result.Count == 0 && receive.Item1 == 0) // 받은 데이터가 아예 없는 실패의 경우
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }

        private async Task<(int, int)> ReadAckAsync(NetworkStream ns, int num)
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

                received_data[num].Add(Encoding.UTF8.GetString(receive_buf));
                result = received_data[num].Count - 1;
            }

            return (state_buf[0], result);
        }

        // private -> public 으로 변경
        public bool Clear_receive_data(int num)
        {
            if (received_data.ContainsKey(num))
            {
                return received_data.Remove(num);
            }
            return false;
        }

        // private -> public 으로 변경
        public T DeSerializeJson<T>(int num, int index)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            string s = (received_data[num])[index];

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(s)))
            {
                var obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }

        // private -> public 으로 변경
        public static string SerializeJson(object obj)
        {
            var ser = new DataContractJsonSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        // private -> public 으로 변경
        public void Login(string user_id)
        {
            this.user_id = user_id;
        }

        // private -> public 으로 변경
        public void Logout()
        {
            this.user_id = "";
            this.received_data.Clear();
        }

        // private -> public 으로 변경
        public string my_User_Id()
        {
            return this.user_id;
        }
    }
}
