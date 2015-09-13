using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Entities
{
    public class Template
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        //public virtual Image Background { get; set; }

        //public virtual Image Overlay { get; set; }

        public virtual FileData Background { get; set; }

        public virtual FileData Overlay { get; set; }


        public int? BackgroundId { get; set; }

        public int? OverlayId { get; set; }

        public ICollection<TemplateImage> Images { get; set; }
    }
}
