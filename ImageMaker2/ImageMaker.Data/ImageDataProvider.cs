using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.Data
{
    public interface IImageDataProvider
    {
        IEnumerable<Template> GetAll();
    }

    public class ImageDataProvider : IImageDataProvider
    {
        private readonly IImageRepository _imageRepository;

        public ImageDataProvider(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public IEnumerable<Template> GetAll()
        {
            return _imageRepository.GetTemplates();
        }
    }
}
