using System;
using System.Linq;
using System.Threading;
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
            var images = explorer.GetImagesByHashTag("wedding", null, new CancellationToken(),15).Result;
            Assert.IsTrue(images != null && images.Images != null && images.Images.Any() && images.Images.All(x => x.Data != null));
        }

        [Test]
        public void TestGetImagesByUserName_UserExists_ReturnsValidImages()
        {
            var explorer = new InstagramExplorer();
            var images = explorer.GetImagesByUserName("jlo", null, new CancellationToken()).Result;
            Assert.IsTrue(images != null && images.Images != null && images.Images.Any() && images.Images.All(x => x.Data != null));
        }
    }
}
