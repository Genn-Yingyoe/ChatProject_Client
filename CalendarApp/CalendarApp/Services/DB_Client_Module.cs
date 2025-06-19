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
    public class DCM
    {
        private string user_id;
        private Dictionary<int, List<string>> received_data;

        public DCM()
        {
            user_id = "";
            received_data = new Dictionary<int, List<string>>();
        }

        public async Task<KeyValuePair<bool, (int, List<int>)>> db_request_data(byte opcode, List<string> items)
        {
            bool success = false;
            List<int> received_data_index = new List<int>();
            int n = -1;

            using (var client = new TcpClient())
            {
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
            byte opByte = opcode;
            List<string> parts = bodyStr;
            Encoding utf8 = Encoding.UTF8;
            byte[][] data = parts.Select(p => utf8.GetBytes(p)).ToArray();

            int len = 1 + 6 + 1 + (parts.Count * sizeof(int));
            len += data.Sum(b => b.Length);

            byte[] packet = new byte[len];
            int pos = 0;

            packet[pos++] = opcode;
            Buffer.BlockCopy(userBytes, 0, packet, pos, 6);
            pos += 6;
            packet[pos++] = (byte)parts.Count;

            foreach (var b in data)
            {
                byte[] tmp = BitConverter.GetBytes(b.Length);
                Buffer.BlockCopy(tmp, 0, packet, pos, sizeof(int));
                pos += sizeof(int);
            }

            foreach (var b in data)
            {
                Buffer.BlockCopy(b, 0, packet, pos, b.Length);
                pos += b.Length;
            }

            await ns.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);

            List<int> result = new List<int>();
            if (opcode == 15)
            {
                try
                {
                    byte[] lenBuf = await ReadExact(ns, 4);
                    if (BitConverter.IsLittleEndian) Array.Reverse(lenBuf);
                    uint i_len = BitConverter.ToUInt32(lenBuf, 0);

                    byte[] i_body = await ReadExact(ns, (int)i_len);

                    string path = bodyStr[2];
                    File.WriteAllBytes(path, i_body);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else if (opcode == 16)
            {
                string dir = bodyStr[2];

                try
                {
                    byte[] img = System.IO.File.ReadAllBytes(dir);

                    byte[] lenBuf = BitConverter.GetBytes((uint)img.Length);
                    if (BitConverter.IsLittleEndian) Array.Reverse(lenBuf);

                    await ns.WriteAsync(lenBuf, 0, 4);   // 헤더
                    await ns.WriteAsync(img, 0, img.Length); // 바디
                    Console.WriteLine($"송신 완료 ({img.Length} B)");
                }
                catch (Exception e)
                {
                    return null;
                }
            }

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
            }
            catch (Exception e)
            {
                //error
                return null;
            }

            return result;
        }

        private async Task<(int, int)> ReadAckAsync(NetworkStream ns, int num)          // item1: 0 == error | 1 == success + end | 2 == success + additional execution      || item2: receive_data_index
        {
            byte[] state_buf = new byte[1];
            byte[] receive_buf_len = new byte[sizeof(int)];
            byte[] receive_buf;
            int result = -1;
            int n = await ns.ReadAsync(state_buf, 0, 1).ConfigureAwait(false);
            if (state_buf[0] != 0 && n != 0)
            {
                n = await ns.ReadAsync(receive_buf_len, 0, sizeof(int)).ConfigureAwait(false);
                int item_len = BitConverter.ToInt32(receive_buf_len, 0);
                receive_buf = new byte[item_len];
                n = await ns.ReadAsync(receive_buf, 0, receive_buf_len[0]).ConfigureAwait(false);

                received_data[num].Add(Encoding.UTF8.GetString(receive_buf));           // login 등 성공여부만을 전달받는 경우에는 >> "1" : 성공 | "0" : 실패
                result = received_data[num].Count - 1;
            }

            return (state_buf[0], result);
        }

        public bool Clear_receive_data(int num) { return received_data.Remove(num); }

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

        public static string SerializeJson(object obj)
        {
            // DataContractJsonSerializer는 '직렬화할 형식'을 생성자에 넘겨야 합니다.
            var ser = new DataContractJsonSerializer(obj.GetType());

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);                       // 객체 → JSON → 메모리스트림
                return Encoding.UTF8.GetString(ms.ToArray());   // UTF-8 문자열로 변환
            }
        }

        private static async Task<byte[]> ReadExact(Stream s, int len)
        {
            byte[] buf = new byte[len];
            int read = 0;
            while (read < len)
            {
                int n = await s.ReadAsync(buf, read, len - read);
                if (n == 0) throw new IOException("remote closed");
                read += n;
            }
            return buf;
        }

        public void Login(string user_id)
        {
            this.user_id = user_id;
        }

        public void Logout()
        {
            this.user_id = "";
            this.received_data = new Dictionary<int, List<string>>();
        }

        public string my_User_Id()
        {
            return this.user_id;
        }
    }
}
