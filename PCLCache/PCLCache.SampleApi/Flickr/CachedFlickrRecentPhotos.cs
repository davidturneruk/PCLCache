using Newtonsoft.Json;
using PCLCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLCache.SampleApi.Flickr
{
    public class CachedFlickrRecentPhotos : FlickrRecentPhotos, ICachedFlickrRecentPhotos
    {
        public const int DefaultCacheExpiryminutes = 30;

        public CachedFlickrRecentPhotos()
            : base()
        {
            CacheExpiryMinutes = DefaultCacheExpiryminutes;
        }

        public int CacheExpiryMinutes
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
                throw new InvalidOperationException("ComputeHash is null, you must initialize ComputeHash with your desired hash function before call this method.  e.g. BaseFlickrRecentPhotosLoader.HashFunction = /* your computeHash code */");
            }

            return ComputeHash(input);
        }
        
        public override async Task<RootObject> GetRecentPhotos(bool forceRefresh = false)
        {
            string hash = CalculateHash(RecentPhotosUrl);
            var result = await DataCache.GetAsync<RootObject>(hash,
                async () =>
                {
                    return await base.GetRecentPhotos(true);
                },
                expireDate: DateTime.Now.AddMinutes(CacheExpiryMinutes),
                forceRefresh: forceRefresh);
            return result;
        }

        public void ClearCache()
        {
            DataCache.ClearAll();
        }


    }
}
