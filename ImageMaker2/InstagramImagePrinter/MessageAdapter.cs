using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.MessageQueueing.Dto;
using ImageMaker.MessageQueueing.MessageQueueing;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.Utils.Services;
using Image = ImageMaker.WebBrowsing.Image;
using DrawingImage = System.Drawing.Image;
using EntityTemplate = ImageMaker.Entities.Template;

namespace InstagramImagePrinter
{
    public class MessageAdapter
    {
        private readonly IMappingEngine _mappingEngine;
        private readonly QueueUtilizer<InstagramMessageDto> _queueUtilizer;
        private readonly ImagePrinter _imagePrinter;
        private readonly ImageService _imageService;
        private readonly PatternViewModelProvider _patternVmProvider;
        private readonly ImageUtils _imageUtils;

        public MessageAdapter(
            IMappingEngine mappingEngine,
            QueueUtilizerFactory queueUtilizerFactory,
            ImagePrinter imagePrinter,
            ImageService imageService,
            PatternViewModelProvider patternViewModelProvider,
             ImageUtils imageUtils)
        {
            _mappingEngine = mappingEngine;
            _queueUtilizer = queueUtilizerFactory.CreateQueue<InstagramMessageDto>();
            _imagePrinter = imagePrinter;
            _imageService = imageService;
            _patternVmProvider = patternViewModelProvider;
            _imageUtils = imageUtils;
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

        public void Print(Image image, string printerName)
        {
            InstagramMessageDto imageDto = _mappingEngine.Map<InstagramMessageDto>(image);

            var result = _patternVmProvider.GetPatterns();

            var instaTemplate = result.SingleOrDefault(x => x.IsInstaPrinterTemplate);
            Action<byte[]> print = null;
            if (!string.IsNullOrEmpty(printerName))
                print = (data) => _imagePrinter.Print(data, printerName);
            else
            {
                print = (data) => _imagePrinter.Print(data);
            }

            byte[] imageData = new byte[] { };
            Size imageStreamSize;

            using (var stream = new MemoryStream(imageDto.Data))
            {
                var img = DrawingImage.FromStream(stream);
                imageStreamSize = img.Size;
            }

            if (instaTemplate != null)
                imageData = _imageUtils.ProcessImages(new List<byte[]> { imageDto.Data }, imageStreamSize,
                    _mappingEngine.Map<EntityTemplate>(instaTemplate));

            else
            {
                //если раземер имени превышает допустимое значение то мы обрезаем его
                var imageName = imageDto.FullName.Length < 23 ? imageDto.FullName : (imageDto.UserName.Length < 23 ? imageDto.UserName : (imageDto.UserName.Substring(0, 20) + "..."));
                imageData = _imageUtils.GetCaptureForInstagramControl(imageDto.Data, imageName, DateTime.Now, imageDto.ProfilePictureData);
            }
            _imageService.SaveImage(new ImageViewModel(imageData));
            print(imageData);
        }
    }
}
