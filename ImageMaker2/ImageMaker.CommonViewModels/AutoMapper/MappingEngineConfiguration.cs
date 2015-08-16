using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;

namespace ImageMaker.CommonViewModels.AutoMapper
{
    public class MappingEngineConfiguration
    {
        public static IMappingEngine CreateEngine(Profile mappingProfile)
        {
            ConfigurationStore store = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            store.AssertConfigurationIsValid();
            MappingEngine engine = new MappingEngine(store);

            store.AddProfile(mappingProfile);
            return engine;
        }
    }
}
