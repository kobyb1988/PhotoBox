using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ImageMaker.Utils.Serialization
{
    public class ObjectSerializer
    {
        public static string Serialize<TObject>(TObject source) where TObject : class
        {
            string result = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TObject));
                StringBuilder stringBuilder = new StringBuilder();

                using (var writer = XmlWriter.Create(stringBuilder))
                {
                    serializer.Serialize(writer, source);
                }

                result = stringBuilder.ToString();
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static TObject Deserialize<TObject>(string source) where TObject : class
        {
            TObject result = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TObject));

                using (var reader = XmlReader.Create(new StringReader(source)))
                {
                    result = serializer.Deserialize(reader) as TObject;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
