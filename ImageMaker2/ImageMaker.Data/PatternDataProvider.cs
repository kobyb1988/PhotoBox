using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.Common.Extensions;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.Data
{
    public interface IPatternDataProvider
    {
        IEnumerable<Pattern> GetPatterns();
    }

    public class PatternDataProvider : IPatternDataProvider
    {
        private readonly IPatternRepository _patternRepository;

        public PatternDataProvider(IPatternRepository patternRepository)
        {
            _patternRepository = patternRepository;
        }

        public IEnumerable<Pattern> GetPatterns()
        {
            throw new NotImplementedException();
            //const string filePath =
            //    @"C:\Users\phantomer\Documents\Visual Studio 2013\Projects\ImageMaker\Test\5_ozer.png";

            //byte[] fileContent = new byte[0];
            //if (File.Exists(filePath))
            //    fileContent = File.ReadAllBytes(filePath);

            //return
            //    Enum.GetValues(typeof (PatternType))
            //        .OfType<PatternType>()
            //        .Select(x => new Pattern(x.GetDescription(), x, fileContent));
        } 
    }
}
