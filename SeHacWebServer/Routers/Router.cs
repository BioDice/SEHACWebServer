using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public abstract class Router
    {
        private Header response { get; set; }
        private Server server;

        public Router(Server server)
        {
            this.server = server;
        }

        public abstract string CheckRoutes(string url, Stream stream);
        // deze methode moet weg zodra er een visitor is gemaakt
        public virtual string CheckAjaxRoutes(string url)
        {
            return null;
        }
    }
}
