using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class InstagramImageViewModel : BaseViewModel
    {
        public InstagramImageViewModel(byte[] data, int width, int height, string name)
        {
            Name = name;
            Height = height;
            Width = width;
            Data = data;
        }

        public byte[] Data { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public string Name { get; private set; }
    }
}
