using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageMaker.Entities
{
    public class Session
    {
        public Session()
        {
            Images = new HashSet<Image>();   
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public virtual ICollection<Image> Images { get; set; }
    }
}
