using System;
namespace PCLCache.SampleApi.Flickr
{
    public interface ICachedFlickrRecentPhotos : IFlickrRecentPhotos
    {
        int CacheExpiryMinutes { get; set; }
        
        void ClearCache();

        HashFunction ComputeHash { get; set; }

    }
}
