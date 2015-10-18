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

        public virtual void SaveImage(ImageViewModel image)
        {
            try
            {
                Session session = _imageRepository.GetActiveSession() ?? _imageRepository.GetLastSession();

                if (session == null)
                {
                    if (_imageRepository.StartSession())
                        session = _imageRepository.GetActiveSession();
                    else
                    {
                        throw new Exception("Cannot start session");
                    }
                }

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                DirectoryInfo info = !Directory.Exists(Path.Combine(baseDir, "Images"))
                ? Directory.CreateDirectory(Path.Combine(baseDir, "Images"))
                : new DirectoryInfo(Path.Combine(baseDir, "Images"));

                string currentSessionStartTime = session.StartTime.ToString("dd_MM_yyyy");
                string subDirectoryPath = Path.Combine(info.FullName, string.Format("{0}_{1}", currentSessionStartTime, session.Id));

                DirectoryInfo subDirectory = !Directory.Exists(subDirectoryPath)
                    ? info.CreateSubdirectory(string.Format("{0}_{1}", currentSessionStartTime, session.Id))
                    : new DirectoryInfo(subDirectoryPath);

                string path = Path.Combine(subDirectory.FullName, string.Format("{0}.png", Guid.NewGuid()));

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
