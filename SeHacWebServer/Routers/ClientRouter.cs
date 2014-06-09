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

        public override string CheckRoutes(string url, Stream stream)
        {
            string[] segments = url.Split('?');
            string path = server.settings.webRoot + segments[0];
            if (url == "/")
            {
                if (Boolean.Parse(server.settings.dirListing))
                    SendDirectories(stream, DirectoryListing.Generate(path, server.settings.webRoot));
                else
                    path = server.settings.webRoot + "/" + server.settings.defaultPage;
            }
            else if (File.Exists(path))
            {
                return server.settings.webRoot + url;
            }
            else if (Directory.Exists(path))
            {
                SendDirectories(stream, DirectoryListing.Generate(path, server.settings.webRoot));
            }
            else
            {
                errorHandler.SendErrorPage(stream, 404);
            }
            return null;
        }

        public void SendDirectories(Stream stream, string dirs)
        {
            Header header = new ResponseHeader();
            byte[] response = Encoding.ASCII.GetBytes(dirs);
            header.SetHeader("ContentLength", response.Length.ToString());
            header.SetHeader("ContentType", @"text\html");
            Statics.SendHeader(header, stream);
            stream.Write(response, 0, response.Length);
        }
    }
}
