using Newtonsoft.Json;
using PCLCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace PCLCache.SampleApi.Flickr
{
    public class LoadFromCacheThenRefreshFlickrRecentPhotos : FlickrRecentPhotos, ILoadFromCacheThenRefreshFlickrRecentPhotos
    {
        public const int DefaultCacheExpiryminutes = 30;

        DataLoader _dataLoader = new DataLoader();

        public LoadFromCacheThenRefreshFlickrRecentPhotos()
        {
            CacheExpiryMinutes = DefaultCacheExpiryminutes;
        }

        public int CacheExpiryMinutes
        {
            get;
            set;
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

        public Action<Exception> ErrorCallback
        {
            get;
            set;
        }

        public HashFunction ComputeHash
        {
            get;
            set;
        }

        private string CalculateHash(string input)
        {

            if (ComputeHash == null)
            {
                throw new InvalidOperationException("ComputeHash is null, you must initialize ComputeHash with your desired hash function before call this method.  e.g. FlickrRecentPhotos.HashFunction = /* your computeHash code */");
            }

            return ComputeHash(input);
        }

        public override async Task GetRecentPhotos(Action<RootObject> callback, bool forceRefresh = false)
        {
            string hash = CalculateHash(RecentPhotosUrl);
            bool refresh = forceRefresh;
            Func<Task<RootObject>> cacheLoad = async () =>
            {
                var result = await DataCache.GetFromCache<RootObject>(hash);
                return result;
            };
            Func<Task<RootObject>> liveDataLoad = async () =>
            {
                var photosResult = await GetRecentPhotos(refresh);
                if (photosResult != null)
                    await DataCache.Set<RootObject>(hash, photosResult, expireDate: DateTime.Now.AddMinutes(CacheExpiryMinutes));
                return photosResult;
            };
            await Loader.LoadCacheThenRefreshAsync(
                cacheLoad,
                liveDataLoad,
                (result) => callback(result), 
                ErrorCallback );
        }

        public void ClearCache()
        {
            DataCache.ClearAll();
        }
    }
}
