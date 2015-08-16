using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImageMaker.WebBrowsing
{
    [JsonObject]
    public class ImageResponse
    {
        [JsonProperty("min_tag_id")]
        public string MinTagId { get; set; }

        public IEnumerable<Image> Images { get; set; }
    }
}
