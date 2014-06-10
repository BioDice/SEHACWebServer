using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeHacWebServer.XMLModels
{
    [Serializable]
    [XmlRoot("ValidExtensions")]
    public class ExtensionsModel
    {
        [XmlArray("Extensions")]
        public List<Extension> extensions = new List<Extension>();
    }
}
