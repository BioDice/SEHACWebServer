﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    class SendContentHandler
    {
        public SendContentHandler()
        {

        }

        public static void SendContent(byte[] bytes, Stream stream)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void SendChunk(int bytesRead, byte[] buffer, Stream stream)
        {
            stream.Write(buffer, 0, bytesRead);
        }

        public static void SendHeader(Header header, Stream stream)
        {
            string sBuffer = "";
            sBuffer = header.ToString();
            stream.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
            stream.Flush();
        }

        public static void SendDirectories(Stream stream, string dirs)
        {
            Header header = new ResponseHeader();
            byte[] response = Encoding.ASCII.GetBytes(dirs);
            header.SetHeader("ContentLength", response.Length.ToString());
            header.SetHeader("ContentType", @"text\html");
            SendContentHandler.SendHeader(header, stream);
            stream.Write(response, 0, response.Length);
        }
    }
}
