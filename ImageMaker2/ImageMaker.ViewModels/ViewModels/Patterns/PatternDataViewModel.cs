using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.Entities;

namespace ImageMaker.ViewModels.ViewModels.Patterns
{
    public class PatternDataViewModel : BaseViewModel
    {
        private readonly string _name;
        private readonly byte[] _data;

        public PatternDataViewModel(string name, int patternType, byte[] data)
        {
            PatternType = patternType;
            _name = name;
            _data = data;
        }

        public int Id { get; set; }
        public int PatternType { get; private set; }

        public string Name
        {
            get { return _name; }
        }

        public byte[] Data
        {
            get { return _data; }
        }
    }
}
