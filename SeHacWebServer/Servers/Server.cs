using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SeHacWebServer
{
    public abstract class Server
    {
        protected int port;
        private bool is_active = true;
        private TcpListener listener;
        public SettingsModel settings { get; set; }
        private Thread thread;
        protected string serverName { get; set; }
        protected Router router { get; set; }
        protected Semaphore m_ServerSemaphore;
        protected string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

        public Server(int port)
        {
            this.port = port;
            m_ServerSemaphore = new Semaphore(20,20);
        }

        public void StartServer()
        {
            Console.WriteLine(serverName + " listening on port: " + port);
            thread = new Thread(new ThreadStart(Listen));
            thread.Start();
        }

        public void Listen()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();
            while (is_active)
            {
                Console.WriteLine("Waiting for connection...");
                TcpClient client = listener.AcceptTcpClient();

                m_ServerSemaphore.WaitOne();
                Stream stream = GetStream(client); // http stream
                String ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                RequestHandler newRequest = new RequestHandler(ip,this, stream);
                Thread Thread = new Thread(new ThreadStart(newRequest.Process));
                Thread.Name = "HTTP Request";
                Thread.Start();
            }
        }

        public void StopServer()
        {
            listener.Stop();
            thread.Abort();
        }

        public abstract Stream GetStream(TcpClient client);

        public abstract void handleGETRequest(RequestHandler p, string url);
        public abstract void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url);

        public Dictionary<string, string> ParsePostData(StreamReader inputData)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string[] str = inputData.ReadLine().Split('&');
            for (int i = 0; i < str.Length; i++)
            {
                string[] temp = str[i].Split('=');
                data.Add(temp[0], temp[1]);
            }
            return data;
        }
    } 
}
