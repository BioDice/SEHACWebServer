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

        //protected int port;
        bool is_active = true;
        TcpListener_notused listener;
        SettingsParser parser;
        List<KeyValuePair<string, string>> settings;

        public HttpServer()
        {
            parser = new SettingsParser();
            settings = new List<KeyValuePair<string, string>>();
        }

        public void listen()
        {
            //listener = new TcpListener(int.Parse(settings.ElementAt(1).Value));
            listener = new TcpListener_notused(3030);
            while (is_active)
            {
                /*TcpClient s = listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);*/
            }
        }
    } 
}
