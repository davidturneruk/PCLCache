using PCLCache.SampleApi.Flickr;
using PortableCacheLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PCLCache.SampleSharedCode
{
    public class RecentPhotosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<Photo> recentPhotoList = new List<Photo>();

        public RecentPhotosViewModel()
        {
            this.UpdatePhotos = (result) =>
            {
                if (result != null && result.photos != null)
                {
                    RecentPhotos = result.photos.photo;
                }
            };
        }

        private DataLoader loader;
        public DataLoader Loader
        {
            get
            {
                return loader;
            }
            set
            {
                loader = value;
                NotifyPropertyChanged();
            }
        }

        public List<Photo> RecentPhotos
        {
            get
            {
                return recentPhotoList;
            }
            private set
            {
                recentPhotoList = value;
                NotifyPropertyChanged();
            }
        }

        public Action<RootObject> UpdatePhotos
        {
            get;
            private set;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var propertyChanged = this.PropertyChanged;
            if(propertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
