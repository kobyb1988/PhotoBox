using System.IO;
using ImageMaker.Utils.Services;
using NUnit.Framework;

namespace ImageMaker.UtilsTests.IntegrationTests
{
    [TestFixture]
    public class SmtpClient
    {
        [Test]
        public void SendEmail_ValidEmail_Ok()
        {
            SmtpService smtpService = new SmtpService();

            const string filePath =
                @"C:\Users\phantomer\Documents\Visual Studio 2013\Projects\ImageMaker\Test\5_ozer.png";

            byte[] fileContent = new byte[0];
            if (File.Exists(filePath))
                fileContent = File.ReadAllBytes(filePath);

            bool result = smtpService.SendEmail(fileContent);
            Assert.IsTrue(result);
        }
    }
}
