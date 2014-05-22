namespace PCLCache.UnitTests
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PCLTesting;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public class WebDataCacheTest
    {

        [TestMethod]
        public async Task GetAsyncTest()
        {
            await WebDataCache.GetAsync(new Uri("http://www.google.com/favicon.ico"), true);
        }

        [TestMethod]
        public async Task GetAsyncLongUriTest()
        {
            int length = 500;

            var baseUri = "http://www.google.com/favicon.ico?";
            var q = new string('x', length - baseUri.Length);

            await WebDataCache.GetAsync(new Uri(baseUri + q), true);
        }

        [TestMethod]
        public void AsyncFileValidFilename()
        {
            string key = Extensions.ToCacheKey(new Uri("http://afisha.tut.by/film.php?fid=2710"));

            Assert.IsFalse(key.Contains("/"));
        }

        [TestMethod]
        public void SameHashTest()
        {
            string key1 = Extensions.ToCacheKey(new Uri("http://afisha.tut.by/film.php?fid=2710"));
            string key2 = Extensions.ToCacheKey(new Uri("http://afisha.tut.by/film.php?fid=2710"));

            Assert.AreEqual(key1, key2);

        }

        [TestMethod]
        public void UniqueHashTest()
        {
            string key1 = Extensions.ToCacheKey(new Uri("http://afisha.tut.by/film.php?fid=2710"));

            //Different querystring param
            string key2 = Extensions.ToCacheKey(new Uri("http://afisha.tut.by/film.php?fid=2711"));

#if NETFX_CORE || WINDOWS_PHONE
            Assert.AreNotEqual(key1, key2);
#endif
        }
    }

}
