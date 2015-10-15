using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using AutoMapper;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Data.Repositories;

namespace ImageMaker.AdminViewModels.Services
{
    public class SessionService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMappingEngine _mappingEngine;

        public SessionService(IImageRepository imageRepository, IMappingEngine mappingEngine)
        {
            _imageRepository = imageRepository;
            _mappingEngine = mappingEngine;
        }

        public virtual void StartSession()
        {
            bool started = _imageRepository.StartSession();
            if (!started)
                return;

            _imageRepository.Commit();
        }

        public virtual async Task<IEnumerable<ImageViewModel>> GetImagesAsync()
        {
            var session = await _imageRepository.GetActiveSessionAsync(true);
            return session.With(x => x.Images.Select(_mappingEngine.Map<ImageViewModel>));
        } 
    }
}
