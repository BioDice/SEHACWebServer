using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeHacWebServer.Model
{
    [Serializable]
    [XmlRoot[""]]
    public class SettingsModel
    {
        public int webPort { get; set; }
        public int controlPort { get; set; }
        public string webRoot { get; set; }
        public string defaultPage { get; set; }
        public string dirListing { get; set; }
    }
}
