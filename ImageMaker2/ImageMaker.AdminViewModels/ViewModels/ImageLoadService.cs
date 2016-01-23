using System;
using System.IO;
using System.Linq;
using ImageMaker.CommonViewModels.ViewModels.Images;
using Microsoft.Win32;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class ImageLoadService
    {
        public ImageViewModel TryLoadImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
                                        {
                                            CheckFileExists = true,
                                            CheckPathExists = true,
                                            Multiselect = true,
                                            Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png"
            };

            Func<string, bool> isValidFile = file =>
                                             {
                                                 string[] extensions =
                                                 {
                                                     ".jpg",
                                                     ".png",
                                                     ".jpeg"
                                                 };

                                                 string ext = Path.GetExtension(file);
                                                 return extensions.Contains(ext);
                                             };

            fileDialog.FileOk += (sender, args) =>
                                 {
                                     if (!fileDialog.FileNames.All(isValidFile))
                                         args.Cancel = true;
                                 };

            bool? dialogResult = fileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                try
                {
                    Stream[] files = fileDialog.OpenFiles();
                    for (int i = 0; i < fileDialog.FileNames.Length; i++)
                    {
                        string filePath = fileDialog.FileNames[i];
                        Stream file = files[i];
                        byte[] fileData = null;

                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            fileData = stream.ToArray();
                        }

                        ImageViewModel viewModel = new ImageViewModel(0, Path.GetFileName(filePath), fileData);

                        return viewModel;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return null;
        }
    }
}