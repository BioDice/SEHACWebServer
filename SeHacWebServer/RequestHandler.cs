﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public class RequestHandler
    {
        public Server srv;

        public Stream stream;
        //public StreamWriter outputStream;

        public String http_method { get; set; }
        public String http_url { get; set; }
        public String http_protocol_versionstring { get; set; }
        public String http_host { get; set; }
        public Hashtable httpHeaders = new Hashtable();
        private static int MAX_POST_SIZE = 10 * 1024 * 1024;

        public RequestHandler(Server server, Stream stream)
        {
            this.stream = stream;
            this.srv = server;
        }

        public void Process()
        {
            //outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));

            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
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
                //if (next_char == -1) { Thread.Sleep(1); continue; };
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
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                http_host = line.Contains("Host") ? http_host = line : http_host = null;
                httpHeaders[name] = value;
            }
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
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
            srv.handlePOSTRequest(this, new StreamReader(ms));
        }

        public void SendHeader(HttpHeaderModel header)
        {
            string sBuffer = "";
            sBuffer = sBuffer + header.Protocol + " " + header.ResponseCode + "\r\n";
            //sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + header.ContentType + "\r\n";
            //sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + header.ContentLength + "\r\n";
            //sBuffer = sBuffer + "Host: "+header.Host+"\r\n";
            sBuffer = sBuffer + "Connection: close \r\n\r\n";
            stream.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
            stream.Flush();
        }
    }
}