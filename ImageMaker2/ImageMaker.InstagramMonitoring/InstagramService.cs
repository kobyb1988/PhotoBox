using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using InstagramImagePrinter;

namespace ImageMaker.InstagramMonitoring
{
    public partial class InstagramService : ServiceBase
    {
        public InstagramService()
        {
            if (!Debugger.IsAttached)
                Debugger.Launch();
            InitializeComponent();
        }

        private CancellationTokenSource _tokenSource;

        protected override void OnStart(string[] args)
        {
            //string hello = "qq from InstaService!";
            //string path = @"C:\Users\phantomer\Desktop\Stuff\empty.txt";
            //File.WriteAllText(path, hello);

            StartService();
            
        }

        protected async void StartService()
        {
            try
            {
                MonitoringService service = MonitoringService.Create();
                _tokenSource = new CancellationTokenSource();
                await service.StartMonitoring(_tokenSource);
            }
            catch (Exception e)
            {
                //File.WriteAllText(path, e.Message);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
