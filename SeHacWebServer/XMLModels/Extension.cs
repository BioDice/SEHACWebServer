using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeHacWebServer.Model
{
    [Serializable]
    //[XmlElement("extension")]
    public class Extension
    {
        [XmlAttribute("ext")]
        public string ext { get; set; }
        [XmlAttribute("content")]
        public string content { get; set; }

        public Extension()
        {
            this.ext = "1";
            this.content = "2";
        }
    }
}
