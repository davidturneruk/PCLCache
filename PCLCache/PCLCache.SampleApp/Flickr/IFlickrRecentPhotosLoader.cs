using System;
using System.Threading.Tasks;

namespace PCLCache.SampleApp.Flickr
{
    interface IFlickrRecentPhotosLoader
    {
        string CalculateMD5Hash(string input);
        void ClearCache();
        Task GetRecentPhotos(string key, int expiryMinutes, Action<RootObject> resultsCallback = null, bool forceRefresh = false);
    }
}
