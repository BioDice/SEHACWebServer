using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SeHacWebServer.Model
{
    class JSONParser
    {
        public static string SerializeJSON(SettingsModel settings)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SettingsModel));
            ser.WriteObject(stream, settings);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string str = sr.ReadToEnd();
            return str;
        }

        public static SettingsModel DeserializeJSON()
        {
            return null;
        }
    }
}
