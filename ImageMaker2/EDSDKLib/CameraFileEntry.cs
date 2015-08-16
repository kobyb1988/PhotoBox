using System.Drawing;

namespace EDSDKLib
{
    public class CameraFileEntry
    {
        public string Name { get; private set; }
        public bool IsFolder { get; private set; }
        public Bitmap Thumbnail { get; private set; }
        public CameraFileEntry[] Entries { get; private set; }

        public CameraFileEntry(string Name, bool IsFolder)
        {
            this.Name = Name;
            this.IsFolder = IsFolder;
        }

        public void AddSubEntries(CameraFileEntry[] Entries)
        {
            this.Entries = Entries;
        }

        public void AddThumb(Bitmap Thumbnail)
        {
            this.Thumbnail = Thumbnail;
        }
    }
}