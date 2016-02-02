using System;
using System.Linq;
using ImageMaker.WebBrowsing;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ImageMaker.WebBrowsingTests.IntegrationTests
{
    [TestFixture]
    public class InstagramExplorerTest
    {
        [Test]
        public void TestGetImagesByHashTag_AnyState_ReturnsValidImages()
        {
            var explorer = new InstagramExplorer();
            var images = explorer.GetImagesByHashTag("wedding", null,15).Result;
            Assert.IsTrue(images != null && images.Images != null && images.Images.Any() && images.Images.All(x => x.Data != null));
        }

        [Test]
        public void TestGetImagesByUserName_UserExists_ReturnsValidImages()
        {
            var explorer = new InstagramExplorer();
            var images = explorer.GetImagesByUserName("jlo", null).Result;
            Assert.IsTrue(images != null && images.Images != null && images.Images.Any() && images.Images.All(x => x.Data != null));
        }
    }
}
