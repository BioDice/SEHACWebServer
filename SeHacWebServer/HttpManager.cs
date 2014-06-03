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

namespace SeHacWebServer
{
    public abstract class HttpManager
    {
        protected int port;
        private bool is_active = true;
        private TcpListener listener;
        public SettingsModel settings { get; set; }
        private Thread thread;
        protected string serverName { get; set; }

        public HttpManager(int port)
        {
            this.port = port;
            
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
                RequestHandler newRequest = new RequestHandler(client,this);
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

        public abstract void handleGETRequest(RequestHandler p, string url);
        public abstract void handlePOSTRequest(RequestHandler p, StreamReader inputData);
    } 
}
