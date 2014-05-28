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
            HttpServer httpServer = new HttpManager(8080);
            httpServer.StartServer();
        }
    }
}
