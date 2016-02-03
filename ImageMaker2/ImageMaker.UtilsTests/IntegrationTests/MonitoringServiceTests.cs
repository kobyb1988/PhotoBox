using System;
using System.Threading;
using System.Threading.Tasks;
using ImageMaker.InstagramMonitoring;
using InstagramImagePrinter;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ImageMaker.UtilsTests.IntegrationTests
{
    [TestFixture]
    public class MonitoringServiceTests
    {
        [Test]
        public async Task StartMonitoring_AnyState_Ok()
        {
            MonitoringService svc = MonitoringService.Create();
            bool res = false;
            await Task.Run(() =>
           {
               svc.StartMonitoring(new CancellationTokenSource(), DateTime.Now + TimeSpan.FromMinutes(2), "love", () => { });
               res = true;
           });

            Assert.IsTrue(res);

        }

        [Test]
        public void StartService_AnyState_Ok()
        {
            ServiceTest svc = new ServiceTest();
            svc.Start();
        }
    }

    public class ServiceTest : InstagramService
    {
        public void Start()
        {
            StartService();
        }
    }
}
