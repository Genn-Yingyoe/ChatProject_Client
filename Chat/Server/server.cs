using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class server : Form
    {
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();
        private Thread listenThread;

        public server()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            listenThread = new Thread(StartListening);
            listenThread.IsBackground = true;
            listenThread.Start();
            Log("서버 시작됨...");
        }

        private void StartListening()
        {
            listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                lock (clients) clients.Add(client);
                Thread t = new Thread(() => HandleClient(client));
                t.IsBackground = true;
                t.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int len = ns.Read(buffer, 0, buffer.Length);
                    if (len == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, len);
                    Broadcast(msg, client);
                    Log($"수신: {msg}");
                }
            }
            catch { }
            finally
            {
                lock (clients) clients.Remove(client);
                client.Close();
            }
        }

        private void Broadcast(string msg, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            lock (clients)
            {
                foreach (var c in clients)
                {
                    if (c != sender)
                    {
                        try { c.GetStream().Write(data, 0, data.Length); } catch { }
                    }
                }
            }
        }

        private void Log(string text)
        {
            if (InvokeRequired)
                Invoke(new Action(() => txtLog.AppendText(text + Environment.NewLine)));
            else
                txtLog.AppendText(text + Environment.NewLine);
        }
    }
}
