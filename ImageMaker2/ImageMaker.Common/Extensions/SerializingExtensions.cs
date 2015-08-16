using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImageMaker.Common.Extensions
{
    public static class SerializingExtensions
    {
        public static TObject Deserialize<TObject>(this byte[] source)
        {
            return source.Return(src =>
            {
                var serializer = new XmlSerializer(typeof (TObject));
                TObject result = default(TObject);
                using (MemoryStream stream = new MemoryStream(source))
                {
                    try
                    {
                        result = (TObject)serializer.Deserialize(stream);
                    }
                    catch (Exception)
                    {
                    }
                }

                return result;
            }, default(TObject));
        }

        public static byte[] Serialize<TObject>(this TObject source) where TObject : class
        {
            return source.Return(src =>
            {
                var serializer = new XmlSerializer(typeof(TObject));
                byte[] buffer = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    try
                    {
                        serializer.Serialize(stream, source);
                        buffer = stream.ToArray();
                    }
                    catch (Exception)
                    {
                    }
                }

                return buffer;
            }, null);
        }
    }
}
