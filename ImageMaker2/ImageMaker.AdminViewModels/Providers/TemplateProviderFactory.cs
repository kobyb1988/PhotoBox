using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.Data.Repositories;

namespace ImageMaker.AdminViewModels.Providers
{
    public class TemplateProviderFactory
    {
        private readonly IImageRepositoryFactory _imageRepositoryFactory;
        private readonly IMappingEngine _mappingEngine;

        public TemplateProviderFactory(
            IImageRepositoryFactory imageRepositoryFactory, 
            IMappingEngine mappingEngine)
        {
            _imageRepositoryFactory = imageRepositoryFactory;
            _mappingEngine = mappingEngine;
        }

        private TemplateViewModelProvider _templateViewModelProvider;
        private CompositionsViewModelProvider _compositionsProvider;

        public virtual TemplateViewModelProvider TemplateProvider
        {
            get
            {
                return _templateViewModelProvider ??
                       (_templateViewModelProvider = new TemplateViewModelProvider(_imageRepositoryFactory.GetRepository(), _mappingEngine));
            }
        }

        public virtual CompositionsViewModelProvider CompositionsProvider
        {
            get 
            {
                return _compositionsProvider ?? 
                    (_compositionsProvider = new CompositionsViewModelProvider(_imageRepositoryFactory.GetRepository(), _mappingEngine)); 
            }
        }
    }
}
