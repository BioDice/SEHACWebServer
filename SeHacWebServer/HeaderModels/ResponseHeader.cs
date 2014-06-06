using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public class ResponseHeader : Header
    {
        private static Dictionary<int, string> statuses = new Dictionary<int, string>()
        {
            { 200, "OK" },
            { 400, "Bad Request"},
            { 403, "Forbidden" },
            { 404, "Not Found" }
        };

        private string protocol = "HTTP/1.1";

        private int status = 200;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
            
        public ResponseHeader()
        {

        }

        public override string ToString()
        {
            var lines = new List<string>()
            {
                protocol + " " + status + " " + statuses[status]
            };

            foreach (var pair in headers)
            {
                lines.Add(pair.Key + ": " + pair.Value);
            }

            string result = String.Join("\r\n", lines);
            result += "\r\n\r\n";

            Console.WriteLine(result);

            return result;
        }
    }
}
