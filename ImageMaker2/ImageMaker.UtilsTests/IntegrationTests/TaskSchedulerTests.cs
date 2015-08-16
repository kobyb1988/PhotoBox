using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using ImageMaker.Utils.Services;
using NUnit.Framework;

namespace ImageMaker.UtilsTests.IntegrationTests
{
    [TestFixture]
    public class TaskSchedulerTests
    {
        [Test]
        public void CreateTask_AnyState_TaskExecuted()
        {
            string path = @"C:\Users\phantomer\Desktop\Stuff\empty.txt";
            File.WriteAllText(path, string.Empty);
           
            var timer = new Timer(7000);
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();

            timer.Elapsed += (sender, args) =>
            {
                source.TrySetResult(true);
            };

            var scheduler = new SchedulerService();
            scheduler.StartInstagramMonitoring(DateTime.Now + TimeSpan.FromSeconds(3));

            timer.Start();

            source.Task.Wait();
            string result = File.ReadAllText(path);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
