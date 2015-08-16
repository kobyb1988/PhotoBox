using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using AutoMapper;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.AdminViewModels.Providers
{
    public class TemplateViewModelProvider
    {
        private readonly IImageRepository _imageDataProvider;
        private readonly IMappingEngine _mappingEngine;

        public TemplateViewModelProvider(
            IImageRepository imageDataProvider,
            IMappingEngine mappingEngine
            )
        {
            _imageDataProvider = imageDataProvider;
            _mappingEngine = mappingEngine;
        }

        public IEnumerable<TemplateViewModel> GetTemplates()
        {
            return _imageDataProvider
                .GetTemplates()
                .Select(_mappingEngine.Map<TemplateViewModel>);
        }

        public async Task<IEnumerable<TemplateViewModel>> GetTemplatesAsync()
        {
            var result = await _imageDataProvider
                .GetTemplatesAsync();

            return result.Select(_mappingEngine.Map<TemplateViewModel>);
        }

        public void SaveChanges()
        {
            _imageDataProvider.Commit();
        }
        public void UpdateTemplates(IEnumerable<TemplateViewModel> templates)
        {
            _imageDataProvider.UpdateTemplates(templates.Select(_mappingEngine.Map<Template>));
        }

        public void AddTemplates(IEnumerable<TemplateViewModel> templates)
        {
            _imageDataProvider.AddTemplates(templates.Select(_mappingEngine.Map<Template>));
        }

        public void RemoveTemplates(IEnumerable<TemplateViewModel> templates)
        {
            _imageDataProvider.RemoveTemplates(templates.Select(_mappingEngine.Map<Template>));
        }
    }
}
