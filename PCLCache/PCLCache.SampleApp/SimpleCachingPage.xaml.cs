using Newtonsoft.Json;
using PCLCache.SampleApp.Common;
using PCLCache.SampleApi.Flickr;
using PortableCacheLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PCLCache.SampleSharedCode;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace PCLCache.SampleApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class SimpleCachingPage : Page, IDisposable
    {
        private NavigationHelper navigationHelper;
        private RecentPhotosViewModel viewModel = new RecentPhotosViewModel();
        private CachedFlickrRecentPhotos photoLoader = 
            new CachedFlickrRecentPhotos();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public RecentPhotosViewModel ViewModel
        {
            get { return this.viewModel;  }
        }

        public SimpleCachingPage()
        {
            this.photoLoader.FlickrKey = App.FlickrKey;
            this.photoLoader.ComputeHash = (input) => {
                var md5h = HashAlgorithmProvider.OpenAlgorithm("MD5").CreateHash();
                var buff = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf16BE);
                md5h.Append(buff);
                var buffHash = md5h.GetValueAndReset();
                return CryptographicBuffer.EncodeToBase64String(buffHash);
            };
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        #region IDisposable implementation

        private bool disposed = false;

        ~SimpleCachingPage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (photoLoader != null)
                    {
                        photoLoader.Dispose();
                        photoLoader = null;
                    }
                }
                // Dispose unmanaged resources

            }
            disposed = true;
        }

        #endregion

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await LoadPhotos();

        }

        private async Task LoadPhotos(bool forceRefresh = false)
        {
            await photoLoader.GetRecentPhotos(ViewModel.UpdatePhotos, forceRefresh);
        }

        
        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "e")]
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var photo = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(ItemDetailPage), (Photo)photo);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            
            this.Frame.Navigate(typeof(ItemDetailPage), ((Photo)e.ClickedItem));
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void BtnClear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            photoLoader.ClearCache();
        }

        private async void BtnRefresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await LoadPhotos();
        }

        private async void BtnForceRefresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await LoadPhotos(true);
        }

        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AdvancedCachingPage));
        }
    }
}