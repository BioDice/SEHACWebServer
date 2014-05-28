using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public abstract class HttpServer
    {

        protected int port;
        bool is_active = true;
        TcpListener listener;
        SettingsModel settings;

        public HttpServer(int port)
        {
            this.port = port;
            settings = XMLParser.DeserializeXML();
        }

        public void StartServer()
        {
            Console.WriteLine("Server starting on port: " + settings.webPort);
            Thread thread = new Thread(new ThreadStart(Listen));
            thread.Start();
        }

        public void Listen()
        {
            listener = new TcpListener(port);
            listener.Start();
            while (is_active)
            {
                TcpClient s = listener.AcceptTcpClient();
                /*HttpProcessor processor = new HttpProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);*/
            }
        }
    } 
}
