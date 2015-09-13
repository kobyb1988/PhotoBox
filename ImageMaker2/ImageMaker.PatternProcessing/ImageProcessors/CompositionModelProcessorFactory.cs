using ImageMaker.Camera;
using ImageMaker.Entities;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class CompositionModelProcessorFactory
    {
        private readonly ImageProcessor _imageProcessor;

        public CompositionModelProcessorFactory(ImageProcessor imageProcessor)
        {
            _imageProcessor = imageProcessor;
        }

        public CompositionModelProcessor Create(Template composition)
        {
            return new CompositionModelProcessor(composition, _imageProcessor);
        }
    }
}
