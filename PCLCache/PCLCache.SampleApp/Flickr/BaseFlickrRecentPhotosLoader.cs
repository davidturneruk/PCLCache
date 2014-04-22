using PortableCacheLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace PCLCache.SampleApp.Flickr
{
    abstract class BaseFlickrRecentPhotosLoader : IFlickrRecentPhotosLoader
    {
        public const string FlickrKey = "2d91c9ee34a385c43a4187f9c31f0f3a";
        public const string RecentPhotosUrlFormat = "http://api.flickr.com/services/rest/?method=flickr.photos.getRecent&api_key={0}&format=json";
    

        public BaseFlickrRecentPhotosLoader()
        {

        }

        public abstract Task GetRecentPhotos(string key, int expiryMinutes, Action<RootObject> resultsCallback = null, bool forceRefresh = false);

        public string CalculateMD5Hash(string input)
        {
            var md5h = HashAlgorithmProvider.OpenAlgorithm("MD5").CreateHash();
            var buff = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf16BE);
            md5h.Append(buff);
            var buffHash = md5h.GetValueAndReset();
            return CryptographicBuffer.EncodeToBase64String(buffHash);
        }

        public void ClearCache()
        {
            DataCache.ClearAll();
        }

    }
}
