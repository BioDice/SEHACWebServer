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
using System.Text.RegularExpressions;

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
                string route = router.CheckRoutes(url, p.stream);
                WritePost(p.stream, route);
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
                //Send404(p);
            }
            m_ServerSemaphore.Release();
        }

        protected override Stream GetStream(TcpClient client)
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
                        GetFormValues(p.stream, url);
                        break;
                    case "LogFiles":
                        OpenLogFile(p.stream);
                        break;
                    case "LoginValues":
                        Authenticate(p.stream, inputData);
                        break;

                    case "AuthenticateRequest":
                        //derp
                        break;
                }
            }
            else
            {
                // handle form post
                PostControlForm(p.stream, inputData, url);
            }
            m_ServerSemaphore.Release();
        }

        /// <param name="inputData">User en Password van form</param>
        public void Authenticate(Stream stream, StreamReader inputData)
        {
            Dictionary<string, string> data = ParsePostData(inputData);
            string user = data.ElementAt(0).Value;
            if (UserAuthentication.Authenticate(user, data.ElementAt(1).Value))
            {
                SessionManager.addSession(user);
                GetLoginAuthentication(stream, true, user);
            }
            else
                GetLoginAuthentication(stream, false, user);
        }

        public void PostControlForm(Stream stream, StreamReader inputData, string url)
        {
            //Console.WriteLine("POST request: {0}", p.http_url);
            Dictionary<string, string> data = ParsePostData(inputData);
            UpdateSettingsModel(data);
            string route = router.CheckRoutes(url, stream);
            WritePost(stream, route);
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

        public void WritePost(Stream stream, string path)
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
            SendContentHandler.SendHeader(header, stream);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void GetFormValues(Stream stream, string url)
        {
            Header header = new ResponseHeader();
            string json = JSONParser.SerializeJSON(settings);
            header.SetHeader("ContentType", "text/html");
            byte[] response = Encoding.ASCII.GetBytes(json);
            header.SetHeader("ContentLength", response.Length.ToString());
            SendContentHandler.SendHeader(header, stream);
            stream.Write(response, 0, response.Length);
        }

        public void GetLoginAuthentication(Stream stream, bool authentication, string user)
        {
            Header header = new ResponseHeader();
            var jSerializer = new JavaScriptSerializer();
            string json = "";
            if (authentication.Equals(true))
            {
                string sessionId = SessionManager.getSessionId(user);
                json = jSerializer.Serialize(new { Authentication = authentication, SessionID = sessionId });

            }
            else
                json = jSerializer.Serialize(new { Authentication = authentication });
            header.SetHeader("ContentType", "text/html");
            byte[] response = Encoding.ASCII.GetBytes(json);
            header.SetHeader("ContentLength", response.Length.ToString());
            SendContentHandler.SendHeader(header, stream);
            stream.Write(response, 0, response.Length);
        }

        public void OpenLogFile(Stream stream)
        {
            Header header = new ResponseHeader();
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(Statics.Root + @"/Logfiles/ControlServer.log.txt"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line + "<br />");
                }
            }
            byte[] response = Encoding.ASCII.GetBytes(sb.ToString());
            header.SetHeader("ContentType", "text/html");
            header.SetHeader("ContentLength", response.Length.ToString());
            SendContentHandler.SendHeader(header, stream);
            stream.Write(response, 0, response.Length);
        }
    }
}
