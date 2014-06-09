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
using System.Net;
using SeHacWebServer.Database;
using System.Web.Script.Serialization;

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
            m_ServerSemaphore.Release();
        }

        public override Stream GetStream(TcpClient client)
        {
            SslStream stream = null;
            try
            {
                stream = new SslStream(client.GetStream(), false);

                X509Certificate certificate = new X509Certificate("Certificate\\Certificate.pfx", "KTYy77216");
                stream.AuthenticateAsServer(certificate, false, SslProtocols.Ssl3, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return stream;
        }

        public override void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url)
        {
            // handle ajax calls
            if (router.CheckAjaxRoutes(url) != null)
            {
                switch (router.CheckAjaxRoutes(url))
                {
                    case "FormValues":
                        GetFormValues(p, url);
                        break;
                    case "LogFiles":
                        OpenLogFile(p);
                        
                        break;
                    case "LoginValues":
                        Authenticate(p, inputData);
                        break;
                }
            }
            else
            {
                // handle form post
                PostControlForm(p, inputData, url);
            }
            m_ServerSemaphore.Release();
        }

        /// <summary>
        /// Authenticate de huidige gebruiker
        /// Als het goed is gegaan moet je redirecten naar de main.html
        /// </summary>
        /// <param name="rHandler">De huidige requesthandler</param>
        public void Authenticate(RequestHandler rHandler, StreamReader inputData)
        {
            Dictionary<string, string> data = ParsePostData(inputData);
            if (UserAuthentication.Authenticate(data.ElementAt(0).Value, data.ElementAt(1).Value))
                GetLoginAuthentication(rHandler, true);
            else
                GetLoginAuthentication(rHandler, false);
        }

        public void PostControlForm(RequestHandler p, StreamReader inputData, string url)
        {
            Console.WriteLine("POST request: {0}", p.http_url);
            Dictionary<string, string> data = ParsePostData(inputData);
            UpdateSettingsModel(data);
            string route = router.CheckRoutes(url);
            WritePost(p, route);
        }

        public void UpdateSettingsModel(Dictionary<string, string> dict)
        {
            SettingsModel settings = new SettingsModel();
            foreach (KeyValuePair<string, string> entry in dict)
            {
                switch (entry.Key)
                {
                    case "controlPort":
                        settings.controlPort = int.Parse(entry.Value);
                        break;
                    case "defaultPage":
                        settings.defaultPage = entry.Value;
                        break;
                    case "dirListing":
                        settings.dirListing = entry.Value == "on" ? settings.dirListing = "true" : settings.dirListing = "false";
                        break;
                    case "webPort":
                        settings.webPort = int.Parse(entry.Value);
                        break;
                    case "webRoot":
                        settings.webRoot = System.Net.WebUtility.UrlDecode(entry.Value);
                        break;
                    default:
                        settings.dirListing = entry.Value == "on" ? settings.dirListing = "true" : settings.dirListing = "false";
                        break;
                }
            }
            XMLParser.SerializeXML(settings);
            this.settings = settings;
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

        public void GetFormValues(RequestHandler p, string url)
        {
            Header header = new ResponseHeader();
            string json = JSONParser.SerializeJSON(settings);
            header.SetHeader("ContentType", p.requestHeader.Headers["Accept"]);
            byte[] response = Encoding.ASCII.GetBytes(json);
            header.SetHeader("ContentLength", response.Length.ToString());
            p.SendHeader(header);
            p.stream.Write(response, 0, response.Length);
        }

        public void GetLoginAuthentication(RequestHandler p, bool authentication)
        {
            Header header = new ResponseHeader();
            var jSerializer = new JavaScriptSerializer();
            string json = jSerializer.Serialize(new { Authentication = authentication });
            header.SetHeader("ContentType", p.requestHeader.Headers["Accept"]);
            byte[] response = Encoding.ASCII.GetBytes(json);
            header.SetHeader("ContentLength", response.Length.ToString());
            p.SendHeader(header);
            p.stream.Write(response, 0, response.Length);
        }

        public void OpenLogFile(RequestHandler p)
        {
            Header header = new ResponseHeader();
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(root + @"/Logfiles/ControlServer.log.txt"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line + "<br />");
                }
            }
            byte[] response = Encoding.ASCII.GetBytes(sb.ToString());
            header.SetHeader("ContentType", p.requestHeader.Headers["Accept"]);
            header.SetHeader("ContentLength", response.Length.ToString());
            p.SendHeader(header);
            p.stream.Write(response, 0, response.Length);
        }
    }
}
