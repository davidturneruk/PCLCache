using Newtonsoft.Json;
using PortableCacheLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Web.Http;

namespace PCLCache.SampleApp.Flickr
{
    class SimpleFlickrRecentPhotosLoader : BaseFlickrRecentPhotosLoader
    {

        public SimpleFlickrRecentPhotosLoader()
        {

        }

        public override async Task GetRecentPhotos(string key, int expiryMinutes, Action<RootObject> resultsCallback = null, bool forceRefresh = false)
        {
            var client = new HttpClient();
            string recentPhotosUrl = string.Format(RecentPhotosUrlFormat, key);
            var result = await DataCache.GetAsync<RootObject>(CalculateMD5Hash(recentPhotosUrl),
                async () =>
                {
                    var tempResult = await client.GetStringAsync(new Uri(recentPhotosUrl, UriKind.Absolute));
                    var resultSubstring = tempResult.Substring(14, tempResult.Length - 15);
                    var photosResult = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RootObject>(resultSubstring));
                    return photosResult;
                },
                expireDate: DateTime.Now.AddMinutes(expiryMinutes),
                forceRefresh: forceRefresh);
            resultsCallback(result);
        }


    }
}
