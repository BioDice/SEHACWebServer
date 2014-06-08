using SeHacWebServer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SeHacWebServer
{
    public class RequestHandler
    {
        public Server srv;
        private Stopwatch requestTimer;
        public Stream stream;
        //public StreamWriter outputStream;

        public String http_method { get; set; }
        public String http_url { get; set; }
        public String http_protocol_versionstring { get; set; }
        public String http_host { get; set; }
        public String http_clientIp { get; set; }
        public RequestHeader requestHeader { get; set; }
        private static int MAX_POST_SIZE = 10 * 1024 * 1024;

        public RequestHandler(String ip,Server server, Stream stream)
        {
            this.http_clientIp = ip;
            this.requestTimer = new Stopwatch();
            this.stream = stream;
            this.srv = server;
            requestHeader = new RequestHeader();
        }

        public void Process()
        {
            try
            {
                requestTimer.Start();
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                    handleGETRequest();
                else if (http_method.Equals("POST"))
                    handlePOSTRequest();
                LogRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
            stream.Flush();
            stream.Close();
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this, http_url);
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        public void parseRequest()
        {
            String request = streamReadLine(stream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            Console.WriteLine("Reading headers...");
            String line;
            while ((line = streamReadLine(stream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                    throw new Exception("invalid http header line: " + line);
                
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                requestHeader.SetHeader(name, value);
            }
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.requestHeader.Headers.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.requestHeader.Headers["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.stream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            srv.handlePOSTRequest(this, new StreamReader(ms), http_url);
        }

        public void SendHeader(Header header)
        {
            string sBuffer = "";
            sBuffer = header.ToString();
            stream.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
            stream.Flush();
        }
        public void LogRequest()
        {
            requestTimer.Stop();
            Logger.addLog(http_clientIp + " - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - "
                + requestTimer.ElapsedMilliseconds + "ms" + ": " + http_method + " " + http_url);

        }
    }
}