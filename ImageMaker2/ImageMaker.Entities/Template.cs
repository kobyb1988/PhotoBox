﻿using System;
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

        public ICollection<TemplateImage> Images { get; set; }
    }
}
