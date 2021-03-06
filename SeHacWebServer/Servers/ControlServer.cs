﻿using SeHacWebServer.Model;
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

        /// <summary>
        /// Handles the Get Request  and releases thread when done
        /// </summary>
        /// <param name="p">The current RequestHandler</param>
        /// <param name="url">Given Get url</param>
        public override void handleGETRequest(RequestHandler p, string url)
        {
            try
            {
                string route = router.CheckRoutes(url, p);
                if (route != null)
                    WritePost(p.stream, route);
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
            }
            m_ServerSemaphore.Release();
        }

        /// <summary>
        /// Gets the SSL stream from the connected client
        /// </summary>
        /// <param name="client">Connected client</param>
        /// <returns>stream from client</returns>
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

        /// <summary>
        /// Handles the post request
        /// </summary>
        /// <param name="p">Current RequestHandler</param>
        /// <param name="inputData"></param>
        /// <param name="url"></param>
        public override void handlePOSTRequest(RequestHandler p, StreamReader inputData, string url)
        {
            string bs = "";
            p.requestHeader.Headers.TryGetValue("Cookie", out bs);
            // handle ajax calls
            if (router.CheckAjaxRoutes(url) != null)
            {
                switch (router.CheckAjaxRoutes(url))
                {
                    case "FormValues":
                        if (SessionManager.SessionExists(bs,p.http_clientIp))
                            GetFormValues(p.stream, url);
                        break;
                    case "LogFiles":
                        if (SessionManager.SessionExists(bs,p.http_clientIp)&&SessionManager.isAdmin(bs))
                            OpenLogFile(p.stream);
                        break;
                    case "LoginValues":
                        Authenticate(p, inputData);
                        break;
                    case "LogoutValues":
                        doLogout(p.stream,inputData);
                        break;
                }
            }
            else
            {
                if (SessionManager.SessionExists(bs,p.http_clientIp) && SessionManager.isAdmin(bs))
                {
                    PostControlForm(p, inputData, url);
                }
                else
                {
                    string route = router.CheckRoutes(url, p);
                    WritePost(p.stream, route);
                }
                // handle form post
            }
            m_ServerSemaphore.Release();
        }

        /// <param name="inputData">User en Password van form</param>
        public void Authenticate(RequestHandler r, StreamReader inputData)
        {
            Dictionary<string, string> data = ParsePostData(inputData);
            string user = data.ElementAt(0).Value;
            if (UserAuthentication.Authenticate(user, data.ElementAt(1).Value))
            {
                SessionManager.addSession(user,r.http_clientIp);
                GetLoginAuthentication(r.stream, true, user);
            }
            else
                GetLoginAuthentication(r.stream, false, user);
        }

        public void doLogout(Stream stream,StreamReader inputData)
        {
            Dictionary<string, string> data = ParsePostData(inputData);
            SessionManager.deleteSession(data.ElementAt(0).Value);

            Header header = new ResponseHeader();
            var jSerializer = new JavaScriptSerializer();
            string json = jSerializer.Serialize(new { Success = true });
            header.SetHeader("ContentType", "text/html");
            byte[] response = Encoding.ASCII.GetBytes(json);
            header.SetHeader("ContentLength", response.Length.ToString());
            SendContentHandler.SendHeader(header, stream);
            stream.Write(response, 0, response.Length);
        }

        public void PostControlForm(RequestHandler r, StreamReader inputData, string url)
        {
            //Console.WriteLine("POST request: {0}", p.http_url);
            Dictionary<string, string> data = ParsePostData(inputData);
            UpdateSettingsModel(data);
            string route = router.CheckRoutes(url, r);
            WritePost(r.stream, route);
        }

        public void UpdateSettingsModel(Dictionary<string, string> dict)
        {
            SettingsModel settings = new SettingsModel();
            foreach (KeyValuePair<string, string> entry in dict)
            {
                //if (!new Regex(@"^\w+$").IsMatch(entry.Value)) return;
                switch (entry.Key)
                {
                    case "controlPort":
                        int cport = 0;
                        if (!int.TryParse(entry.Value, out cport)) return;
                        settings.controlPort = cport;
                        break;
                    case "defaultPage":
                        settings.defaultPage = entry.Value;
                        break;
                    case "dirListing":
                        settings.dirListing = entry.Value == "on" ? settings.dirListing = "true" : settings.dirListing = "false";
                        break;
                    case "webPort":
                        int wport = 0;
                        if (!int.TryParse(entry.Value, out wport)) return;
                        settings.webPort = wport;
                        break;
                    case "webRoot":
                        settings.webRoot = System.Net.WebUtility.UrlDecode(entry.Value);
                        break;
                    default:
                        settings.dirListing = entry.Value == "on" ? settings.dirListing = "true" : settings.dirListing = "false";
                        break;
                }
            }
            XMLParser.SerializeSettingsXML(settings);
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
            string extension = GetFileExtensionFromString(path);
            header.SetHeader("ContentLength", bytes.Length.ToString());
            header.SetHeader("ContentType", ext.extensions.Where(x => x.ext == extension).FirstOrDefault().content + "; charset=UTF-8 ");
            SendContentHandler.SendHeader(header, stream);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void GetFormValues(Stream stream, string url)
        {
            Header header = new ResponseHeader();
            string json = JSONParser.SerializeJSON(settings);
            byte[] response = Encoding.ASCII.GetBytes(json);
            header.SetHeader("ContentType", "text/html; charset=UTF-8 ");
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
