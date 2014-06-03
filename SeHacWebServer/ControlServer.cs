using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    class ControlServer : HttpManager
    {
        public ControlServer(int port)
            : base(port)
        {
            serverName = "ControlServer";
        }

        public override void handleGETRequest(RequestHandler p, string url)
        {/*
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);

            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
            p.outputStream.WriteLine("</form></body></html>");*/
        }

        public override void handlePOSTRequest(RequestHandler p, System.IO.StreamReader inputData)
        {
            throw new NotImplementedException();
        }
    }
}
