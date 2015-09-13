using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Effort.Provider;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Data.Repositories;
using ImageMaker.DataContext.Contexts;
using ImageMaker.Entities;
using NUnit.Framework;

namespace ImageMaker.CommonViewModelsTests
{
    [TestFixture]
    public class ImageServiceTests
    {
        private ImageContext _context;

        [TestFixtureSetUp]
        public void ImageServiceFixtureSetup()
        {
            //NinjectBindings.Configure(new IntegrationTestNinjectModule());
            var dirInfo = new DirectoryInfo("Images");
            if (dirInfo.Exists)
                dirInfo.Delete(true);

            EffortProviderConfiguration.RegisterProvider();
            _context = new ImageContext(Effort.DbConnectionFactory.CreateTransient());
            _context.Database.CreateIfNotExists();
           // _context.Database.Initialize(true);
        }

        [Test]
        public void SavesImage_ValidImaeg_DoesNotThrow()
        {
            string filePath = @"C:\Users\Дмитрий\Desktop\India\индия\агра и варанаси\DSC_0002.jpg";
            byte[] data = File.ReadAllBytes(filePath);

            ImageRepository rep = new ImageRepository(_context);
            Session session = new Session() { StartTime = DateTime.Now };
            _context.Sessions.Add(session);
            _context.SaveChanges();

            var imageService = new ImageService(rep);
            
            imageService.SaveImage(new ImageViewModel(data));

            Assert.IsNotEmpty(Directory.EnumerateFiles("Images", "*.png"));
            Assert.IsNotNull(_context.Images.FirstOrDefault());
        }
    }
}
