using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public class ClientRouter : Router
    {
        private Server server { get; set; }

        public ClientRouter(Server server) : base(server)
        {
            this.server = server;
        }

        public override string CheckRoutes(string url, string host)
        {
            string path = server.settings.webRoot + url;
            if (url == "/")
            {
                path = server.settings.webRoot + "/" + server.settings.defaultPage;
                return path;
            }
            else if (File.Exists(path))
            {
                return path;
            }
            return path;
        }
    }
}
