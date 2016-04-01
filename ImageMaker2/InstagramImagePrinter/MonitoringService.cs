using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels.Ninject;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.WebBrowsing;
using InstagramImagePrinter.Ninject;
using Ninject;
using ImageMaker.Data.Repositories;

namespace InstagramImagePrinter
{
    public class MonitoringService
    {
        private readonly MessageAdapter _messageAdapter;
        private readonly InstagramExplorer _instagramExplorer;
        private readonly ImageUtils _imageUtils;
        private readonly IImageRepository _imageRep;
        private readonly string _hashTag;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;
        private readonly string _printerName;
        private string _lastInstagramImageId;
        private Process screenSaver;

        public MonitoringService(
            SettingsProvider settingsProvider,
            MessageAdapter messageAdapter,
            InstagramExplorer instagramExplorer,
            ImageUtils imageUtils,
            IImageRepository imageRep)
        {

            _messageAdapter = messageAdapter;
            _instagramExplorer = instagramExplorer;
            _imageUtils = imageUtils;
            _imageRep = imageRep;
            AppSettingsDto settings = settingsProvider.GetAppSettings();
            if (settings == null)
                throw new InvalidOperationException();

            _hashTag = settings.HashTag;
            _startTime = imageRep.GetActiveSession(includeImages: false).StartTime;
            _endTime = settings.DateEnd;
            _printerName = settings.PrinterName;
        }

        public static MonitoringService Create()
        {
            var kernel = NinjectBootstrapper.GetKernel(new MainModule());
            return kernel.Get<MonitoringService>();
        }

        public void StartMonitoring(CancellationTokenSource tokenSource, Action stopService)
        {
            StartMonitoring(tokenSource, _startTime, _endTime, _hashTag, stopService);
        }

        //TODO передать токен для отмены ниже по стеку
        public void StartMonitoring(CancellationTokenSource tokenSource, DateTime startDate, DateTime endDate, string hashTag, Action stopService)
        {
            var thread = new Thread(() =>
           {

               var printed = new List<string>();
               string nextUrl = null;
               while (!tokenSource.IsCancellationRequested)
               {
                   if (DateTime.Now.Ticks >= endDate.Ticks)
                       break;

                   Task.Delay(TimeSpan.FromSeconds(10)).Wait();

                   ImageResponse result = string.IsNullOrEmpty(nextUrl)
                        ? _instagramExplorer.GetImagesByHashTag(hashTag, "", tokenSource.Token, 1).Result
                        : _instagramExplorer.GetImagesFromUrl(nextUrl, tokenSource.Token).Result;

                   nextUrl = result.Return(x => x.NextUrl, null);

                   foreach (var image in result.Images)
                   {
                       if (image.CreatedTime < startDate.Ticks)
                       {
                           nextUrl = null;
                           printed.Add(image.Url);
                       }

                       if (printed.Contains(image.Url))
                           continue;

                       printed.Add(image.Url);
                       var imageData = _imageUtils.GetCaptureForInstagramControl(image.Data, image.FullName, DateTime.Now, image.ProfilePictureData);

                       image.Data = imageData;
                       _messageAdapter.ProcessImages(new List<Image> { image }, _printerName);
                   }
               }


               stopService();
           });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
