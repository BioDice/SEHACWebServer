using SeHacWebServer.Model;
using SeHacWebServer.XMLModels;
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
        public static void SerializeSettingsXML(SettingsModel settings)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(SettingsModel));
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            TextWriter WriteFileStream = new StreamWriter(root + "/XML/settings.xml");
            SerializerObj.Serialize(WriteFileStream, settings);
            WriteFileStream.Close();
        }

        public static SettingsModel DeserializeSettingsXML()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(SettingsModel));
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            FileStream ReadFileStream = new FileStream(root + "/XML/settings.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            SettingsModel LoadedObj = (SettingsModel)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();

            return LoadedObj;
        }

        public static void SerializeExtensionsXML(ExtensionsModel settings)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(ExtensionsModel));
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            TextWriter WriteFileStream = new StreamWriter(root + "/XML/extensions2.xml");
            SerializerObj.Serialize(WriteFileStream, settings);
            WriteFileStream.Close();
        }

        public static ExtensionsModel DeserializeExtensionXML()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(ExtensionsModel));
            string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            FileStream ReadFileStream = new FileStream(root + "/XML/extensions.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            ExtensionsModel LoadedObj = (ExtensionsModel)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();

            return LoadedObj;
        }
    }
}
