using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    class ControlServer : HttpServer
    {
        public ControlServer(int port)
            : base(port)
        {

        }

        public override void handleGETRequest(RequestHandler p)
        {
            throw new NotImplementedException();
        }

        public override void handlePOSTRequest(RequestHandler p, System.IO.StreamReader inputData)
        {
            throw new NotImplementedException();
        }
    }
}
