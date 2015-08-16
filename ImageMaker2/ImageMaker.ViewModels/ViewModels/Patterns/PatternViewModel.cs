using System.Collections.Generic;
using System.Collections.ObjectModel;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.ViewModels.ViewModels.Patterns
{
    public class PatternViewModel : BaseViewModel
    {
        private string _name;
        private ObservableCollection<PatternDataViewModel> _children;

        public PatternViewModel(string name, int patternType)
        {
            PatternType = patternType;
            Name = name;
            _children = new ObservableCollection<PatternDataViewModel>();
        }

        public PatternViewModel(string name, int patternType, IEnumerable<PatternDataViewModel> children)
        {
            PatternType = patternType;
            Name = name;
            _children = new ObservableCollection<PatternDataViewModel>(children);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public int PatternType { get; private set; }

        public ObservableCollection<PatternDataViewModel> Children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
}
