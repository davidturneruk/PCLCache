using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLCache.SampleApi.Flickr
{
    //http://api.flickr.com/services/rest/?method=flickr.photos.getRecent&api_key=2d91c9ee34a385c43a4187f9c31f0f3a&format=json
     
    public class Photo
    {
        private const string FlickUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}_{4}.jpg";
        private const string LargeSquareThumbnail = "q";
        private const string LargeImage = "b";
    
        public string id { get; set; }
        public string owner { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public int farm { get; set; }
        public string title { get; set; }
        public int ispublic { get; set; }
        public int isfriend { get; set; }
        public int isfamily { get; set; }
        public string ThumbnailPhotoUri
        {
            get
            {
                return string.Format(FlickUrl, farm, server, id, secret, LargeSquareThumbnail);
            }
        }
        public string PhotoUri
        {
            get
            {
                return string.Format(FlickUrl, farm, server, id, secret, LargeImage);
            }
        }
    }

    public class Photos
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public int total { get; set; }
        public List<Photo> photo { get; set; }
    
    }

    public class RootObject
    {
        public Photos photos { get; set; }
        public string stat { get; set; }
    }
}
