using ImageMaker.Entities;

namespace ImageMaker.PatternProcessing
{
    public class CompositionProcessingResult
    {
        public CompositionProcessingResult(Composition pattern, byte[] result)
        {
            SelectedComposition = pattern;
            ImageResult = result;
        }

        public Composition SelectedComposition { get; private set; }

        public byte[] ImageResult { get; private set; }
    }
}
