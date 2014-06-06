using SeHacWebServer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeHacWebServer
{
    class XMLParser
    {
        public static void SerializeXML(SettingsModel settings)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(SettingsModel));
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            TextWriter WriteFileStream = new StreamWriter(root + "/settings.xml");
            SerializerObj.Serialize(WriteFileStream, settings);
            WriteFileStream.Close();
        }

        public static SettingsModel DeserializeXML()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(SettingsModel));
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            FileStream ReadFileStream = new FileStream(root + "/XML/settings.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            SettingsModel LoadedObj = (SettingsModel)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();

            return LoadedObj;
        }
    }
}
