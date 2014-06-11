using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeHacWebServer.Database;

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
            ajaxCalls.Add("/postLogin","LoginValues");
            ajaxCalls.Add("/postLogout", "LogoutValues");
        }

        public override string CheckRoutes(string url, RequestHandler r)
        {
            string Cookies = "";
            r.requestHeader.Headers.TryGetValue("Cookie", out Cookies);

            if (SessionManager.SessionExists(Cookies,r.http_clientIp))
            {
                if (url.Equals("/"))
                {
                    return root + @"/controlserver_files/login.html";
                }
                else if (url.Equals("/main.html"))
                {
                    return root + @"/controlserver_files/main.html";
                }
                else
                {
                    server.errorHandler.SendErrorPage(r.stream, 404);
                }
            }
            else if (url.Equals("/") || url.Equals("/login.html") || url.Equals("/main.html"))
            {
                return root + @"/controlserver_files/login.html";
            }
            else
            {
                server.errorHandler.SendErrorPage(r.stream, 404);
            }
            return null;
        }

        public override string CheckAjaxRoutes(string url)
        {
            string output = null;
            ajaxCalls.TryGetValue(url, out output);
            return output;
        }
    }
}
