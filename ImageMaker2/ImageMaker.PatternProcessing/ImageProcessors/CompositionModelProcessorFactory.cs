using ImageMaker.Camera;
using ImageMaker.Entities;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class CompositionModelProcessorFactory
    {
        private readonly ImageProcessor _imageProcessor;
        private readonly ImageUtils _imageUtils;

        public CompositionModelProcessorFactory(ImageProcessor imageProcessor,ImageUtils imageUtils)
        {
            _imageProcessor = imageProcessor;
            _imageUtils = imageUtils;
        }

        public CompositionModelProcessor Create(Template composition)
        {
            return new CompositionModelProcessor(composition, _imageProcessor,_imageUtils);
        }
    }
}
