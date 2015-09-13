using System;
using System.IO;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.CommonViewModels.Services
{
    public class ImageService
    {
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public void SaveImage(ImageViewModel image)
        {
            Session session = _imageRepository.GetActiveSession();

            try
            {
                DirectoryInfo info = !Directory.Exists("Images")
                ? Directory.CreateDirectory("Images")
                : new DirectoryInfo("Images");

                string path = Path.Combine(info.FullName, string.Format("{0}.png", Guid.NewGuid()));

                File.WriteAllBytes(path, image.Data);

                Image imageDb = new Image()
                {
                    Session = session,
                    Name = image.Name,
                    Path = path
                };

                imageDb.Session = session;
                _imageRepository.AddImage(imageDb);
                _imageRepository.Commit();
            }
            catch (Exception)
            {
            }
        }
    }
}
