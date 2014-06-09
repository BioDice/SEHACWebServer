using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public class Statics
    {
        public static string Root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

        public static void SendHeader(Header header, Stream stream)
        {
            string sBuffer = "";
            sBuffer = header.ToString();
            stream.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
            stream.Flush();
        }
    }
}
