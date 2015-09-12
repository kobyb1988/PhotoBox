using System.Runtime.Serialization;

namespace ImageMaker.AppServer
{
    [DataContract]
    public class Command
    {
        string _data = "empty";

        [DataMember]
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}