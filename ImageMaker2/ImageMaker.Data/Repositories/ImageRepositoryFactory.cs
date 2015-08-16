using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.DataContext.Contexts;

namespace ImageMaker.Data.Repositories
{
    public interface IImageRepositoryFactory
    {
        IImageRepository GetRepository();
    }

    public class ImageRepositoryFactory : IImageRepositoryFactory
    {
        private readonly ImageContextFactory _contextFactory;

        public ImageRepositoryFactory(ImageContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IImageRepository GetRepository()
        {
            return new ImageRepository(_contextFactory.Create());
        }
    }

    public class ImageContextFactory : IDbContextFactory<ImageContext>
    {
        public ImageContext Create()
        {
            return new ImageContext();
        }
    }
}
