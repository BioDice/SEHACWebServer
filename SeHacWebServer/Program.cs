using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SettingsModel settings = XMLParser.DeserializeXML();
            Server httpServer = new HttpServer(settings);
            httpServer.StartServer();
            Server controlServer = new ControlServer(settings);
            controlServer.StartServer();
        }
    }
}
