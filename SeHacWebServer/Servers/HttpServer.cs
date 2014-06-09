using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public class HttpServer : Server
    {
        public HttpServer(SettingsModel settings) : base(settings.webPort)
        {
            serverName = "HttpServer";
            this.settings = settings;
            router = new ClientRouter(this);
        }

        public override void handleGETRequest(RequestHandler p, string url) 
        {
            try
            {
                string path = router.CheckRoutes(url, p.stream);
                if (path != null)
                {
                    if (path.Contains('?'))
                    {
                        Dictionary<string, string> data = ParseGetData(url);
                        WritePost(data, path.Split('?')[0], p.stream);
                    }
                    else
                        WritePost(p.stream, path);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
                errorHandler.SendErrorPage(p.stream, 404);
            }
            m_ServerSemaphore.Release();
        }

        public override Stream GetStream(TcpClient client)
        {
            return client.GetStream();
        }

        public override void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url) 
        {
            
            string path = router.CheckRoutes(url, p.stream);
            //Console.WriteLine("POST request: {0}", p.http_url);
            Dictionary<string, string> data = ParsePostData(inputData);
            WritePost(data, path, p.stream);
            m_ServerSemaphore.Release();
        }

        public void WritePost(Dictionary<string, string> data, string path, Stream stream)
        {
            Header header = new ResponseHeader();
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            foreach (KeyValuePair<string, string> entry in data)
            {
                sb.Replace("{{ "+ entry.Key +" }}", entry.Value);
            }
            
            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
            header.SetHeader("ContentLength", bytes.Length.ToString());
            header.SetHeader("ContentType", "text/html");
            Statics.SendHeader(header, stream);
            stream.Write(bytes, 0, bytes.Length);
        }

        protected void WritePost(Stream stream, string path)
        {
            Header header = new ResponseHeader();
            const int chunkSize = 1024;
            using (var file = File.OpenRead(path))
            {
                header.SetHeader("ContentLength", file.Length.ToString());
                header.SetHeader("ContentType", "text/html");
                Statics.SendHeader(header, stream);
                int bytesRead;
                var buffer = new byte[chunkSize];
                while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                {
                    SendChunk(bytesRead, buffer, stream);
                }
            }
        }

        private void SendChunk(int bytesRead, byte[] buffer, Stream stream)
        {
            stream.Write(buffer, 0, bytesRead);
        }
    }
}
