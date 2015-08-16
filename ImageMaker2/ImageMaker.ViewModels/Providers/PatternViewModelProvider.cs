using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.Data;
using ImageMaker.Data.Repositories;
using ImageMaker.ViewModels.ViewModels.Images;
using ImageMaker.ViewModels.ViewModels.Patterns;

namespace ImageMaker.ViewModels.Providers
{
    public class PatternViewModelProvider
    {
        private readonly IImageRepository _imageDataProvider;
        private readonly IMappingEngine _mappingEngine;

        public PatternViewModelProvider(IImageRepository imageDataProvider, IMappingEngine mappingEngine)
        {
            _imageDataProvider = imageDataProvider;
            _mappingEngine = mappingEngine;
        }

        public IEnumerable<CompositionViewModel> GetPatterns()
        {
            return _imageDataProvider.GetCompositions().Select(_mappingEngine.Map<CompositionViewModel>);
        }

        public async Task<IEnumerable<CompositionViewModel>> GetPatternsAsync()
        {
            var result = await _imageDataProvider.GetCompositionsAsync();
            return result.Select(_mappingEngine.Map<CompositionViewModel>);
        } 
    }
}
