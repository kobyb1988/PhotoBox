using System;
using System.Windows.Controls;
using ImageMaker.PatternProcessing.Extensions;

namespace ImageMaker.PatternProcessing.ProcessingViews
{
    /// <summary>
    /// Interaction logic for InstagramDefaultCtrl.xaml
    /// </summary>
    public partial class InstagramDefaultCtrl : UserControl
    {
        public InstagramDefaultCtrl()
        {
            InitializeComponent();
        }

        public void FillData(byte[] data, string publishAuthorName, DateTime timeUpdate, byte[] profilePictureData)
        {
            BodyImage.Source = data.ToImage();
            AvatarImg.Source = profilePictureData.ToImage();
            PublishAuthorTxt.Text = publishAuthorName;
            TimeUpdateTxt.Text = timeUpdate.ToShortDateString();
        }
    }
}
