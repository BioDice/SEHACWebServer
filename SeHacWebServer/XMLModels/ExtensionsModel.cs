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
    [XmlRoot("validextensions")]
    public class ExtensionsModel
    {
        public List<Extension> extensions;

        public ExtensionsModel()
        {
            extensions = new List<Extension>();
        }
    }
}
