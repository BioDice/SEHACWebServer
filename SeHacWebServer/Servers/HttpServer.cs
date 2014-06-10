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
                string path = router.CheckRoutes(url, p);
                if (path != null)
                {
                    if (path.Contains('?'))
                    {
                        Header header = new ResponseHeader();
                        Dictionary<string, string> data = ParseGetData(url);
                        byte[] bytes = WritePost(data, path.Split('?')[0], p.stream);
                        string extension = GetFileExtensionFromString(path);
                        header.SetHeader("ContentLength", bytes.Length.ToString());
                        header.SetHeader("ContentType", ext.extensions.Where(x => x.ext == extension).FirstOrDefault().content);
                        SendContentHandler.SendHeader(header, p.stream);
                        SendContentHandler.SendContent(bytes, p.stream);
                    }
                    else
                        WritePost(p.stream, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("File not Found");
                errorHandler.SendErrorPage(p.stream, 404);
            }
            m_ServerSemaphore.Release();
        }

        public override void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url) 
        {
            Header header = new ResponseHeader();
            string path = router.CheckRoutes(url, p);
            //Console.WriteLine("POST request: {0}", p.http_url);
            Dictionary<string, string> data = ParsePostData(inputData);
            byte[] bytes = WritePost(data, path, p.stream);
            string extension = GetFileExtensionFromString(url);
            header.SetHeader("ContentLength", bytes.Length.ToString());
            header.SetHeader("ContentType", ext.extensions.Where(x => x.ext == extension).FirstOrDefault().content);
            SendContentHandler.SendHeader(header, p.stream);
            SendContentHandler.SendContent(bytes, p.stream);
            m_ServerSemaphore.Release();
        }

        protected override Stream GetStream(TcpClient client)
        {
            return client.GetStream();
        }

        // writes content for a form that is posted
        private byte[] WritePost(Dictionary<string, string> data, string path, Stream stream)
        {
            
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                foreach (KeyValuePair<string, string> entry in data)
                {
                    sb.Replace("{{ " + entry.Key + " }}", entry.Value);
                }
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        // writes content for just a request
        private void WritePost(Stream stream, string path)
        {
            Header header = new ResponseHeader();
            const int chunkSize = 1024;
            using (var file = File.OpenRead(path))
            {
                string extension = Path.GetExtension(file.Name).Replace(".", "");
                header.SetHeader("ContentLength", file.Length.ToString());
                header.SetHeader("ContentType", ext.extensions.Where(x => x.ext == extension).FirstOrDefault().content);
                SendContentHandler.SendHeader(header, stream);
                int bytesRead;
                var buffer = new byte[chunkSize];
                while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                {
                    SendContentHandler.SendChunk(bytesRead, buffer, stream);
                }
            }
        }
    }
}
