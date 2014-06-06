using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeHacWebServer.Model
{
    [Serializable]
    [XmlRoot("extension")]
    public class Extension
    {
        [XmlAttribute("ext")]
        public string ext { get; set; }
        [XmlAttribute("content")]
        public string content { get; set; }

        public Extension(string ext, string content)
        {
            this.ext = ext;
            this.content = content;
        }
    }
}
