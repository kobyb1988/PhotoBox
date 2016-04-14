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

        protected  void StartService()
        {
            try
            {
                MonitoringService service = MonitoringService.Create();
                Thread.Sleep(10000);
                _tokenSource = new CancellationTokenSource();
                 service.StartMonitoring(_tokenSource,()=>Stop());
                EventLog.WriteEntry(MonitoringService.EventTarget, "Service start", EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry(MonitoringService.EventTarget, "Service start fail:" + e.Message + "\nStack trace:" + e.StackTrace,EventLogEntryType.Error);
                //File.WriteAllText(path, e.Message);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
