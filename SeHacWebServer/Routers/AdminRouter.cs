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
        private string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        private Server server { get; set; }
        private Dictionary<string, string> ajaxCalls;

        public AdminRouter(Server server) : base(server)
        {
            this.server = server;
            ajaxCalls = new Dictionary<string, string>();
            ajaxCalls.Add("/getFormValues", "FormValues");
            ajaxCalls.Add("/getLogFiles", "LogFiles");
        }

        public override string CheckRoutes(string url)
        {
            return root + @"/controlserver_files/main.html";
        }

        public override string CheckAjaxRoutes(string url)
        {
            return ajaxCalls[url];

                return JSONParser.SerializeJSON(server.settings);
        }

        
    }
}
