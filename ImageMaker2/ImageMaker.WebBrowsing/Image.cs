using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageMaker.WebBrowsing
{
    [JsonObject]
    public class Image
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
        
        [JsonProperty("createdtime")]
        public long CreatedTime { get; set; }

        [JsonProperty("data")]
        public byte[] Data { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("profilepictureObject")]
        public string UrlAvatar { get; set; }

        [JsonProperty("profilepicturedata")]
        public byte [] ProfilePictureData { get; set; }
    }
}
