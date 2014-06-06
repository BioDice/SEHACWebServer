using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public class Router
    {
        private RequestHandler request;
        private ResponseStatus response;

        public Router(RequestHandler request, ResponseStatus response)
        {
            this.request = request;
            this.response = response;
        }

        public void CheckRoutes(string url)
        {

        }
    }
}
