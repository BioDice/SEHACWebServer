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
                string path = router.CheckRoutes(url);
                if (!Directory.Exists(path))
                    WritePost(p, path);
                else if (Boolean.Parse(settings.dirListing))
                    SendDirectories(p, DirectoryListing.Generate(path, settings.webRoot));
                else
                    SendErrorPage(p);
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
                SendErrorPage(p);
            }
            m_ServerSemaphore.Release();
        }

        public override Stream GetStream(TcpClient client)
        {
            return client.GetStream();
        }
    
        public override void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url) 
        {
            Header header = new ResponseHeader();
            string path = router.CheckRoutes(url);
            Console.WriteLine("POST request: {0}", p.http_url);
            Dictionary<string, string> data = ParsePostData(inputData);
            byte[] bytes = WritePost(data, path);
            header.SetHeader("ContentLength", bytes.Length.ToString());
            header.SetHeader("ContentType", p.requestHeader.Headers["Accept"]);

            p.SendHeader(header);
            p.stream.Write(bytes, 0, bytes.Length);
            m_ServerSemaphore.Release();
        }

        public byte[] WritePost(Dictionary<string, string> data, string path)
        {
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
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        public void WritePost(RequestHandler p, string path)
        {
            Header header = new ResponseHeader();
            string sResponse = "";
            int iTotBytes = 0;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(fs);
            byte[] bytes = new byte[fs.Length];
            int read;
            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
            {
                sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);
                iTotBytes = iTotBytes + read;
            }
            reader.Close();
            fs.Close();

            header.SetHeader("ContentLength", bytes.Length.ToString());
            header.SetHeader("ContentType", p.requestHeader.Headers["Accept"]);

            p.SendHeader(header);

            p.stream.Write(bytes, 0, bytes.Length);
        }

        public void SendDirectories(RequestHandler p, string dirs)
        {
            Header header = new ResponseHeader();
            byte[] response = Encoding.ASCII.GetBytes(dirs);
            header.SetHeader("ContentLength", response.Length.ToString());
            header.SetHeader("ContentType", p.requestHeader.Headers["Accept"]);
            p.SendHeader(header);
            p.stream.Write(response, 0, response.Length);
        }

        public void SendErrorPage(RequestHandler p)
        {
            ResponseHeader header = new ResponseHeader();
            string content = "<html><head><title>404 Not Found</title></head><body><h1>404 - Page Not Found</body></html>";
            byte[] response = Encoding.ASCII.GetBytes(content);
            header.Status = 404;
            header.SetHeader("ContentLength", response.Length.ToString());
            p.SendHeader(header);
            p.stream.Write(response, 0, response.Length);
        }
    }
}
