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
            HttpManager httpServer = new HttpServer(settings.webPort);
            httpServer.settings = settings;
            httpServer.StartServer();
            HttpManager controlServer = new ControlServer(settings.controlPort);
            controlServer.settings = settings;
            controlServer.StartServer();
        }
    }
}
