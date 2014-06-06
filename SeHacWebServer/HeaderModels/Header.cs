using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    public abstract class Header
    {
        protected Dictionary<string, string> headers;
        public Dictionary<string, string> Headers
        {
            get { return headers; }
        }

        public Header()
        {
            headers = new Dictionary<string, string>();
        }

        public void SetHeader(string key, string value)
        {
            if (headers.ContainsKey(key))
            {
                headers.Remove(key);
            }

            headers.Add(key, value);
        }
    }
}
