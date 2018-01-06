using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    static class xmlIO
    {
        public static void saveObject(object obj,string filename)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, obj);
        }

        public static object loadObject(string filename,Type t)
        {
            //Type t = Type.GetType(typename);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(t);
            System.IO.StreamReader sr = new System.IO.StreamReader(filename, new System.Text.UTF8Encoding(false));
            object output = serializer.Deserialize(sr);

            sr.Close();
            return output;
        }
    }
}
