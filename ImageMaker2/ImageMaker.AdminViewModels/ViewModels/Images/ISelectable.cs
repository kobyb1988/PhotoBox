using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }

        event Action<ISelectable> SelectionChanged;

        void SetSelected(bool status);
    }
}
