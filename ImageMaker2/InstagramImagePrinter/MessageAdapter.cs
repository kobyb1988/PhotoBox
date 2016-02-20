using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.MessageQueueing.Dto;
using ImageMaker.MessageQueueing.MessageQueueing;
using ImageMaker.Utils.Services;
using ImageMaker.WebBrowsing;

namespace InstagramImagePrinter
{
    public class MessageAdapter
    {
        private readonly IMappingEngine _mappingEngine;
        private readonly QueueUtilizer<InstagramMessageDto> _queueUtilizer;
        private readonly ImagePrinter _imagePrinter;
        private readonly ImageService _imageService;

        public MessageAdapter(
            IMappingEngine mappingEngine, 
            QueueUtilizerFactory queueUtilizerFactory, 
            ImagePrinter imagePrinter,
            ImageService imageService)
        {
            _mappingEngine = mappingEngine;
            _queueUtilizer = queueUtilizerFactory.CreateQueue<InstagramMessageDto>();
            _imagePrinter = imagePrinter;
            _imageService = imageService;
        }

        public void ProcessImages(IEnumerable<Image> images, string printerName)
        {
            foreach (var image in images)
            {
                InstagramMessageDto imageDto = _mappingEngine.Map<InstagramMessageDto>(image);
                _imageService.SaveImage(new ImageViewModel(image.Data));
                _imagePrinter.Print(imageDto.Data, printerName);

                //await _queueUtilizer.SendMessage(imageDto);
            }
        }
    }
}
