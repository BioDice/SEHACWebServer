using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public class HttpHeaderModel
    {
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }
        public string ResponseCode { get; set; }

        public HttpHeaderModel()
        {

        }
    }
}
