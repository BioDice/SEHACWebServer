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
        private ErrorPageHandler errorHandler { get; set; }

        public ClientRouter(Server server) : base(server)
        {
            this.server = server;
            this.errorHandler = new ErrorPageHandler();
        }

        public override string CheckRoutes(string url, RequestHandler r)
        {
            string[] segments = url.Split('?');
            string path = server.settings.webRoot + segments[0];
            if (url == "/")
            {
                if (Boolean.Parse(server.settings.dirListing))
                    SendContentHandler.SendDirectories(r.stream, DirectoryListing.Generate(path, server.settings.webRoot));
                else
                    return server.settings.webRoot + "/" + server.settings.defaultPage;
            }
            else if (File.Exists(path))
            {
                return server.settings.webRoot + url;
            }
            else if (Directory.Exists(path))
            {
                if (Boolean.Parse(server.settings.dirListing))
                    SendContentHandler.SendDirectories(r.stream, DirectoryListing.Generate(path, server.settings.webRoot));
                else
                    errorHandler.SendErrorPage(r.stream, 403);
            }
            else
            {
                errorHandler.SendErrorPage(r.stream, 404);
            }
            return null;
        }
    }
}
