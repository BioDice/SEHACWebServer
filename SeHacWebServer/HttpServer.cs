﻿using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public class HttpServer : HttpManager
    {
        public HttpServer(int port) : base(port)
        {

        }

        public override void handleGETRequest(RequestHandler p, string url) 
        {
            HttpHeaderModel header = new HttpHeaderModel();
            try
            {
                string sResponse = "";
                int iTotBytes = 0;
                string path = "";
                if (url == "/")
                    path = settings.webRoot + "/" + settings.defaultPage;
                else
                    path = settings.webRoot + url;
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                // Create a reader that can read bytes from the FileStream.


                BinaryReader reader = new BinaryReader(fs);
                byte[] bytes = new byte[fs.Length];
                int read;
                while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Read from the file and write the data to the network
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
            catch (IOException ex)
            {
                Console.WriteLine("File not Found");
                Send404(p);
            }
        }
    
        public override void handlePOSTRequest(RequestHandler p, StreamReader inputData) {
            /*Console.WriteLine("POST request: {0}", p.http_url);
            p.writeSuccess();
            string data = inputData.ReadToEnd();
            string content = "";
            content += "<html><body><h1>test server</h1>";
            content += "<a href=/test>return</a><p>";
            content += "postbody: <pre>"+data+"</pre>";
            content += "</p></body></html>";*/
            //p.outputStream.WriteLine(content);
        }

        public void Send404(RequestHandler p)
        {
            HttpHeaderModel header = new HttpHeaderModel();
            string content = "<html><head><title>404 Not Found</title></head><body><h1>404 - Page Not Found</body></html>";
            byte[] response = Encoding.ASCII.GetBytes(content);
            header.Protocol = "HTTP/1.1";
            header.ResponseCode = "404 Not Found";
            header.ContentLength = response.Length;
            p.SendHeader(header);
            p.stream.Write(response, 0, response.Length);
        }
    }
}
