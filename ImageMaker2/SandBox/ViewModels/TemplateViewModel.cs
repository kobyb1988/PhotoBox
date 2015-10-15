using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBox.ViewModels
{
    public class TemplateViewModel
    {
        private ObservableCollection<TemplateImageViewModel> _children;

        public TemplateViewModel()
        {
            Width = 500;
            Height = 500;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public ObservableCollection<TemplateImageViewModel> Children
        {
            get { return _children ?? (_children = new ObservableCollection<TemplateImageViewModel>(InitChildren())); }
        }

        private IEnumerable<TemplateImageViewModel> InitChildren()
        {
            var rand = new Random(DateTime.Now.Second);
            return
                Enumerable.Range(0, 5)
                    .Select(
                        x => new TemplateImageViewModel(rand.NextDouble() * 0.5, rand.NextDouble() * 0.5, 0.1,
                            0.1, x, Width, Height) {Index = x});
        } 
    }
}
