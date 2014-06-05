using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    class ControlServer : HttpManager
    {
        public ControlServer(int port)
            : base(port)
        {
            serverName = "ControlServer";
        }

        public override void handleGETRequest(RequestHandler p, string url)
        {
            try
            {
                string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                string path = root + @"/controlserver_files/main.html";
                WritePost(p, path);

            }
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
                //Send404(p);
            }
        }

        public override void handlePOSTRequest(RequestHandler p, System.IO.StreamReader inputData)
        {
            throw new NotImplementedException();
        }

        public void WritePost(RequestHandler p, string path)
        {
            HttpHeaderModel header = new HttpHeaderModel();
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

            header.ContentLength = bytes.Length;
            header.ContentType = "text/html";
            header.Protocol = "HTTP/1.1";
            header.ResponseCode = "200 OK";

            p.SendHeader(header);

            p.stream.Write(bytes, 0, bytes.Length);
        }
    }
}
