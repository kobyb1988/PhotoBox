using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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

        public MessageAdapter(
            IMappingEngine mappingEngine, 
            QueueUtilizerFactory queueUtilizerFactory, 
            ImagePrinter imagePrinter)
        {
            _mappingEngine = mappingEngine;
            _queueUtilizer = queueUtilizerFactory.CreateQueue<InstagramMessageDto>();
            _imagePrinter = imagePrinter;
        }

        public async Task ProcessImages(IEnumerable<Image> images, string printerName)
        {
            foreach (var image in images)
            {
                InstagramMessageDto imageDto = _mappingEngine.Map<InstagramMessageDto>(image);
                _imagePrinter.PrintAsync(imageDto.Data, printerName);
                await _queueUtilizer.SendMessage(imageDto);
            }
        }
    }
}
