using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageMaker.Entities
{
    public class Pattern
    {
        public Pattern(string name, PatternType patternType)
        {
            Name = name;
            PatternType = patternType;
            Children = new HashSet<PatternData>();
        }

        public Pattern()
        {
            Children = new HashSet<PatternData>();
        }

        public virtual ICollection<PatternData> Children { get; set; }

        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public PatternType PatternType { get; set; }
    }
}
