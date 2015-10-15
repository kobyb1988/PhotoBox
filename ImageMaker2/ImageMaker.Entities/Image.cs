using System.ComponentModel.DataAnnotations.Schema;

namespace ImageMaker.Entities
{
    public class Image
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        //public virtual FileData Data { get; set; }

        //public int FileDataId { get; set; }

        public virtual Session Session { get; set; }

        public int SessionId { get; set; }
    }
}
