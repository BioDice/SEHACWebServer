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
    public abstract class HttpServer
    {
        protected int port;
        private bool is_active = true;
        private TcpListener listener;
        private SettingsModel settings;
        private Thread thread;

        public HttpServer(int port)
        {
            this.port = port;
            settings = XMLParser.DeserializeXML();
        }

        public void StartServer()
        {
            Console.WriteLine("Server listening on port: " + settings.webPort);
            thread = new Thread(new ThreadStart(Listen));
            thread.Start();
        }

        public void Listen()
        {
            listener = new TcpListener(port);
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

        public abstract void handleGETRequest(RequestHandler p);
        public abstract void handlePOSTRequest(RequestHandler p, StreamReader inputData);
    } 
}
