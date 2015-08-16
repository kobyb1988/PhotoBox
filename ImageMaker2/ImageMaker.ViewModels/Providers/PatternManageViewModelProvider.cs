using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImageMaker.Data.Repositories;
using ImageMaker.ViewModels.ViewModels;
using ImageMaker.ViewModels.ViewModels.Patterns;

namespace ImageMaker.ViewModels.Providers
{
    public class PatternManageViewModelProvider
    {
        private readonly IMappingEngine _mappingEngine;

        private readonly IPatternRepository _patternsRepository;

        public PatternManageViewModelProvider(
            IPatternRepository patternsRepository, 
            IMappingEngine mappingEngine)
        {
            _patternsRepository = patternsRepository;
            _mappingEngine = mappingEngine;
        }

        public IEnumerable<PatternManageViewModel> Items
        {
            get { return _patternsRepository.GetPatterns()
                .Select(_mappingEngine.Map<PatternViewModel>)
                .Select(x => new PatternManageViewModel(x, _patternsRepository, _mappingEngine)); }
        } 
    }
}
