using System;
using System.Linq;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels.Ninject;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.WebBrowsing;
using InstagramImagePrinter.Ninject;
using Ninject;

namespace InstagramImagePrinter
{
    public class MonitoringService
    {
        private readonly MessageAdapter _messageAdapter;
        private readonly InstagramExplorer _instagramExplorer;
        private readonly string _hashTag;
        private readonly DateTime _endTime;
        private readonly string _printerName;

        public MonitoringService(
            SettingsProvider settingsProvider, 
            MessageAdapter messageAdapter, 
            InstagramExplorer instagramExplorer)
        {
            _messageAdapter = messageAdapter;
            _instagramExplorer = instagramExplorer;
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
            await Task.Run(async () =>
            {
                string minTagId = null;
                while (!tokenSource.IsCancellationRequested)
                {
                    if (DateTime.Now.Ticks >= endDate.Ticks)
                        break;

                    Task.Delay(TimeSpan.FromSeconds(30)).Wait();
                    ImageResponse result = await _instagramExplorer.GetImagesByHashTag(hashTag, minTagId);
                    minTagId = result.Return(x => x.MinTagId, null);

                    await _messageAdapter.ProcessImages(result.Return(x => x.Images, Enumerable.Empty<Image>()), _printerName);
                }

            }, tokenSource.Token);
        }
    }
}
