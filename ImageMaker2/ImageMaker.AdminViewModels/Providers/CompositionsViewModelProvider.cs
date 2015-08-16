using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.AdminViewModels.Providers
{
    public class CompositionsViewModelProvider
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMappingEngine _mappingEngine;

        public CompositionsViewModelProvider(
            IImageRepository imageRepository, 
            IMappingEngine mappingEngine)
        {
            _imageRepository = imageRepository;
            _mappingEngine = mappingEngine;
        }


        public IEnumerable<CompositionViewModel> GetCompositions()
        {
            return _imageRepository
                .GetCompositions()
                .Select(_mappingEngine.Map<CompositionViewModel>);
        }

        public async Task<IEnumerable<CompositionViewModel>> GetCompositionsAsync()
        {
            var result =  await _imageRepository
                .GetCompositionsAsync();
            
            return result.Select(_mappingEngine.Map<CompositionViewModel>);
        }

        public void SaveChanges()
        {
            _imageRepository.Commit();
        }
        public void UpdateCompositions(IEnumerable<CompositionViewModel> compositions)
        {
            _imageRepository.UpdateCompositions(compositions.Select(_mappingEngine.Map<Composition>));
        }

        public void AddCompositions(IEnumerable<CompositionViewModel> compositions)
        {
            _imageRepository.AddCompositions(compositions.Select(_mappingEngine.Map<Composition>));
        }

        public void RemoveCompositions(IEnumerable<CompositionViewModel> compositions)
        {
            _imageRepository.RemoveCompositions(compositions.Select(_mappingEngine.Map<Composition>));
        }
    }
}
