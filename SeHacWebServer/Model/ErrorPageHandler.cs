using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public class ErrorPageHandler
    {
        public ErrorPageHandler()
        {

        }

        public void SendErrorPage(Stream stream, int code)
        {
            ResponseHeader header = new ResponseHeader();
            using (StreamReader sr = new StreamReader(Statics.Root + @"/Errorpages/" + code + ".html"))
            {
                String line;
                String content = "";
                while ((line = sr.ReadLine()) != null)
                {
                    content += line;
                }
                byte[] response = Encoding.ASCII.GetBytes(content);
                header.Status = code;
                header.SetHeader("ContentLength", response.Length.ToString());
                SendContentHandler.SendHeader(header, stream);
                stream.Write(response, 0, response.Length);
            }
        }
    }
}
