using Newtonsoft.Json;
using PortableCacheLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace PCLCache.SampleApp.Flickr
{
    class LoadFromCacheThenRefreshLoader : BaseFlickrRecentPhotosLoader
    {
        DataLoader _dataLoader = new DataLoader();

        public LoadFromCacheThenRefreshLoader()
        {

        }

        public DataLoader Loader
        {
            get
            {
                return _dataLoader;
            }
            set
            {
                _dataLoader = value;
            }
        }


        public override async Task GetRecentPhotos(string key, int expiryMinutes, Action<RootObject> resultsCallback = null, bool forceRefresh = false)
        {
            string recentPhotosUrl = string.Format(RecentPhotosUrlFormat, key);
            string hash = CalculateMD5Hash(recentPhotosUrl);
            Func<Task<RootObject>> cacheLoad = async () =>
            {
                var result = await DataCache.GetFromCache<RootObject>(hash);
                return result;
            };
            Func<Task<RootObject>> liveDataLoad = async () =>
            {
                HttpClient client = new HttpClient();
                //TODO disable httpclient caching behviour
                var tempResult = await client.GetStringAsync(new Uri(recentPhotosUrl, UriKind.Absolute));
                var resultSubstring = tempResult.Substring(14, tempResult.Length - 15);
                var photosResult = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RootObject>(resultSubstring));
                if (photosResult != null)
                    await DataCache.Set<RootObject>(hash, photosResult);
                return photosResult;
            };
            await Loader.LoadCacheThenRefreshAsync(
                forceRefresh ? liveDataLoad : cacheLoad,
                liveDataLoad,
                (result) => resultsCallback(result),
                async (exc) =>
                {
                    var msg = new MessageDialog(exc.Message, "Error");
                    await msg.ShowAsync();
                });
        }
    }
}
