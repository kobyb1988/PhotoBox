using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace InstagramImagePrinter
{
    public class MonitoringService
    {
        private readonly MessageAdapter _messageAdapter;
        private readonly InstagramExplorer _instagramExplorer;
        private readonly ImageUtils _imageUtils;
        private readonly string _hashTag;
        private readonly DateTime _endTime;
        private readonly string _printerName;
        private string _lastInstagramImageId;

        public MonitoringService(
            SettingsProvider settingsProvider,
            MessageAdapter messageAdapter,
            InstagramExplorer instagramExplorer,
            ImageUtils imageUtils)
        {
            
            _messageAdapter = messageAdapter;
            _instagramExplorer = instagramExplorer;
            _imageUtils = imageUtils;
            AppSettingsDto settings = settingsProvider.GetAppSettings();
            if (settings == null)
                throw new InvalidOperationException();

            _hashTag = settings.HashTag;
            _endTime = settings.DateEnd;
            _printerName = settings.PrinterName;
        }

        public static MonitoringService Create()
        {
            var kernel = NinjectBootstrapper.GetKernel(new MainModule());
            return kernel.Get<MonitoringService>();
        }

        public async Task StartMonitoring(CancellationTokenSource tokenSource)
        {
            await StartMonitoring(tokenSource, _endTime, _hashTag);
        }

        public async Task StartMonitoring(CancellationTokenSource tokenSource, DateTime endDate, string hashTag)
        {
            if (!Debugger.IsAttached)
                Debugger.Launch();
            //await Task.Factory.StartNew (async () =>
            //{
                string nextUrl = null;
                while (!tokenSource.IsCancellationRequested)
                {
                    if (DateTime.Now.Ticks >= endDate.Ticks)
                        break;


                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                    
                    ImageResponse result = string.IsNullOrEmpty(nextUrl)
                        ? await _instagramExplorer.GetImagesByHashTag(hashTag, _lastInstagramImageId, 1)
                        : await _instagramExplorer.GetImagesFromUrl(nextUrl);

                    nextUrl = result.Return(x => x.NextUrl, null);

                    if (nextUrl == null && _lastInstagramImageId == result.MinTagId)
                        continue;
                    _lastInstagramImageId = result.MinTagId;

                    foreach (var image in result.Images)
                    {
                        
                        var imageData = _imageUtils.GetCaptureForInstagramControl(image.Data, image.FullName, DateTime.Now, image.ProfilePictureData);

                        image.Data = imageData;
                        await _messageAdapter.ProcessImages(new List<Image>() { image }, _printerName);
                    }
                }

           // },  tokenSource.Token,TaskCreationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
