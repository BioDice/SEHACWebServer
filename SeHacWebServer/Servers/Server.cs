using Microsoft.Security.Application;
using SeHacWebServer.Model;
using SeHacWebServer.XMLModels;
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
        
        private bool is_active = true;
        private TcpListener listener;
        private Thread thread;
        public SettingsModel settings { get; set; }
        protected int port;
        protected Semaphore m_ServerSemaphore;
        protected string serverName { get; set; }
        protected Router router { get; set; }
        public ErrorPageHandler errorHandler { get; set; }
        protected ExtensionsModel ext { get; set; }

        public Server(int port)
        {
            this.port = port;
            m_ServerSemaphore = new Semaphore(20,20);
            errorHandler = new ErrorPageHandler();
            ext = XMLParser.DeserializeExtensionXML();
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
                Stream stream = GetStream(client);

                String ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                RequestHandler newRequest = new RequestHandler(ip, this, stream);
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

        protected abstract Stream GetStream(TcpClient client);

        public abstract void handleGETRequest(RequestHandler p, string url);
        public abstract void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url);

        protected Dictionary<string, string> ParsePostData(StreamReader inputData)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string[] str = inputData.ReadLine().Split('&');
            for (int i = 0; i < str.Length; i++)
            {
                string[] temp = str[i].Split('=');
                string postValue = System.Net.WebUtility.UrlDecode(temp[1]);
                data.Add(temp[0], System.Net.WebUtility.HtmlEncode(postValue));
                
            }
            return data;
        }

        protected Dictionary<string, string> ParseGetData(string url)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string[] tempo = url.Split('?');
            string[] str = tempo[1].Split('&');
            for (int i = 0; i < str.Length; i++)
            {
                string[] temp = str[i].Split('=');
                string postValue = System.Net.WebUtility.UrlDecode(temp[1]);
                data.Add(temp[0], System.Net.WebUtility.HtmlEncode(postValue));
            }
            return data;
        }

        protected string GetFileExtensionFromString(string path)
        {
            string[] str = path.Split('.');
            return str[1].Split('?')[0];
        }
    } 
}
