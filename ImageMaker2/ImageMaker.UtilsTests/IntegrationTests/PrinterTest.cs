using System.IO;
using ImageMaker.Utils.Services;
using NUnit.Framework;

namespace ImageMaker.UtilsTests.IntegrationTests
{
    [TestFixture]
    public class PrinterTest
    {
        [Test]
        public void Print_AnyState_PdfDocOnExit()
        {
            var printer = new ImagePrinter();

            const string filePath =
                @"C:\Users\phantomer\Documents\Visual Studio 2013\Projects\ImageMaker\Test\5_ozer.png";

            byte[] fileContent = new byte[0];
            if (File.Exists(filePath))
                fileContent = File.ReadAllBytes(filePath);

            printer.Print(fileContent);
        }
    }
}
