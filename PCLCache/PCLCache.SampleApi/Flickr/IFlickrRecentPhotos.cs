using System;
using System.Threading.Tasks;

namespace PCLCache.SampleApi.Flickr
{
    public interface IFlickrRecentPhotos
    {
        string FlickrKey { get; set; }
        Task<RootObject> GetRecentPhotos(bool forceRefresh);
        Task GetRecentPhotos(Action<RootObject> callback, bool forceRefresh);
    }
}
