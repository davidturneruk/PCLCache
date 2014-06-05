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
    
    
    public class FlickrRecentPhotos : IFlickrRecentPhotos, IDisposable
    {
        public const string RecentPhotosUrlFormat = "http://api.flickr.com/services/rest/?method=flickr.photos.getRecent&api_key={0}&format=json";

        private HttpClient client;
        private string flickrKey;
        private bool disposed = false;

        public FlickrRecentPhotos()
        {
            client = new HttpClient();
        }

        ~FlickrRecentPhotos()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (client != null)
                    {
                        client.Dispose();
                        client = null;
                    }
                }

                // Dispose unmanaged resources
            }
            disposed = true;
        }

        public string FlickrKey
        {
            get
            {
                return flickrKey;
            }
            set
            {
                flickrKey = value;
                this.RecentPhotosUrl = string.Format(RecentPhotosUrlFormat, flickrKey);
            }
        }

        protected string RecentPhotosUrl
        {
            get;
            set;
        }

        public virtual async Task GetRecentPhotos(Action<RootObject> callback, bool forceRefresh = false)
        {
            var photos = await this.GetRecentPhotos(forceRefresh);
            if (callback != null)
                callback(photos);
        }

        private DateTime lastRequested = DateTime.Today;

        public virtual async Task<RootObject> GetRecentPhotos(bool forceRefresh = false)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(RecentPhotosUrl, UriKind.Absolute));
                if(forceRefresh)
                    request.Headers.IfModifiedSince = lastRequested;
                var response = await client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var contentJson = content.Substring(14, content.Length - 15);
                    var photos = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RootObject>(contentJson));
                    lastRequested = DateTime.Now;
                    return photos;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }


    }
}
