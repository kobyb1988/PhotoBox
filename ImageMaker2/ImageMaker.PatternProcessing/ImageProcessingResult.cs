using ImageMaker.Entities;

namespace ImageMaker.PatternProcessing
{
    public class CompositionProcessingResult
    {
        public CompositionProcessingResult(Template pattern, byte[] result)
        {
            SelectedComposition = pattern;
            ImageResult = result;
        }

        public Template SelectedComposition { get; private set; }

        public byte[] ImageResult { get; private set; }
    }
}
