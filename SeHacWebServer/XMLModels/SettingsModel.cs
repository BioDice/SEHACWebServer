using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeHacWebServer.Model
{
    [Serializable]
    [XmlRoot("serverSettings")]
    [DataContract]
    public class SettingsModel
    {
        [DataMember]
        public int webPort { get; set; }
        [DataMember]
        public int controlPort { get; set; }
        [DataMember]
        public string webRoot { get; set; }
        [DataMember]
        public string defaultPage { get; set; }
        [DataMember]
        public string dirListing { get; set; }
    }
}
