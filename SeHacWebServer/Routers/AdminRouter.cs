using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public class AdminRouter : Router
    {
        private Server server { get; set; }

        public AdminRouter(Server server) : base(server)
        {
            this.server = server;
        }

        public override string CheckRoutes(string url)
        {
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            return root + @"/controlserver_files/main.html";
        }

        public override string CheckAjaxRoutes(string url)
        {
            return JSONParser.SerializeJSON(server.settings);
        }
    }
}
