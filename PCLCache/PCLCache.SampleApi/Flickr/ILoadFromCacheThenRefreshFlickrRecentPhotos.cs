namespace PCLCache.SampleApi.Flickr
{
    using PortableCacheLibrary;
    using System;

    public interface ILoadFromCacheThenRefreshFlickrRecentPhotos : ICachedFlickrRecentPhotos
    {
        Action<Exception> ErrorCallback { get; set; }
        DataLoader Loader { get; set; }
    }
}
