using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    class ControlServer : Server
    {
        public ControlServer(SettingsModel settings)
            : base(settings.controlPort)
        {
            serverName = "ControlServer";
            this.settings = settings;
            router = new AdminRouter(this);
        }

        public override void handleGETRequest(RequestHandler p, string url)
        {
            try
            {
                string route = router.CheckRoutes(url);
                WritePost(p, route);
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
                //Send404(p);
            }
        }

        public override Stream GetStream(TcpClient client)
        {
            SslStream stream = new SslStream(client.GetStream(), false);

            X509Certificate certificate = new X509Certificate("Certificate\\Certificate.pfx", "KTYy77216");
            stream.AuthenticateAsServer(certificate, false, SslProtocols.Ssl3, false);

            return stream;
        }

        public override void handlePOSTRequest(RequestHandler p, System.IO.StreamReader inputData, string url)
        {
            Header header = new ResponseHeader();
            if (p.requestHeader.Headers["Accept"].Contains("json"))
            {
                string json = router.CheckAjaxRoutes(url);
                header.SetHeader("ContentType",p.requestHeader.Headers["Accept"]);
                byte[] response = Encoding.ASCII.GetBytes(json);
                header.SetHeader("ContentLength", response.Length.ToString());
                p.SendHeader(header);
                p.stream.Write(response, 0, response.Length);
            }
            else
            {
                Console.WriteLine("POST request: {0}", p.http_url);
                List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
                string[] str = inputData.ReadLine().Split('&');
                for (int i = 0; i < str.Length; i++)
                {
                    string[] temp = str[i].Split('=');
                    data.Add(new KeyValuePair<string, string>(temp[0], temp[1]));
                }
                
                header.SetHeader("ContentType", "text/html");
                p.SendHeader(header);

            }
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
            header.SetHeader("ContentType", "text/html");
            p.SendHeader(header);
            p.stream.Write(bytes, 0, bytes.Length);
        }

        public string ShowLog()
        {
            return null;
        }
    }
}
