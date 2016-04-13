using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.CommonViewModels.Commands
{
    public interface INotifyPropertyChangedWithRaise : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }
}
