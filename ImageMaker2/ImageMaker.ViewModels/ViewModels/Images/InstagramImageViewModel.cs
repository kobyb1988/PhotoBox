using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.ViewModels.ViewModels.Images
{
    public class InstagramImageViewModel : BaseViewModel
    {
        private bool _isChecked;

        public InstagramImageViewModel(byte[] data, int width, int height, string name, string fullName, byte[] profilePictureData, string urlAvatar, string userName)
        {
            UserName = userName;
            Name = name;
            Height = height;
            Width = width;
            Data = data;
            FullName = fullName;
            ProfilePictureData = profilePictureData;
            UrlAvatar = urlAvatar;
        }

        public string UserName { set; get; }

        public byte[] Data { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public string Name { get; private set; }

        public string FullName { get; set; }

        public string UrlAvatar { get; set; }

        public byte[] ProfilePictureData { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value) return;

                _isChecked = value;
                RaisePropertyChanged();
            }
        }
    }
}
