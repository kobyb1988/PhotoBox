﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] AppSettings { get; set; }

        public byte[] CameraSettings { get; set; }

        public byte[] ThemeSettings { get; set; }

        public byte[] AvailableModules { get; set; }

        public byte MaxPrinterCopies { get; set; }

        public string Password { get; set; }
    }
}
