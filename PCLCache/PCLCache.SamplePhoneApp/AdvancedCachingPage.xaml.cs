﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PCLCache.SamplePhoneApp.Resources;
using PCLCache.SampleApi.Flickr;
using System.Threading.Tasks;
using PCLCache.SamplePhoneApp.Common;
using PCLCache.SampleSharedCode;

namespace PCLCache.SamplePhoneApp
{
    public partial class AdvancedCachingPage : PhoneApplicationPage, IDisposable
    {
        private LoadFromCacheThenRefreshFlickrRecentPhotos photoLoader =
            new LoadFromCacheThenRefreshFlickrRecentPhotos();

        private RecentPhotosViewModel viewModel;

        public RecentPhotosViewModel ViewModel
        {
            get { return viewModel; }
            private set { viewModel = value; }
        }

        // Constructor
        public AdvancedCachingPage()
        {
            this.photoLoader.FlickrKey = App.FlickrKey;
            this.photoLoader.ComputeHash = (input) =>
            {
                //MD5 implementation via Jeff Wilcox: http://www.jeff.wilcox.name/2008/03/silverlight-2-md5/
                //TODO: MD5 implementation is compatible with PCL and could be moved into the API project
                return MD5CryptoServiceProvider.GetMd5String(input);
            };
            this.photoLoader.ErrorCallback = (exception) =>
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
            };
            ViewModel = new RecentPhotosViewModel();
            viewModel.Loader = photoLoader.Loader;
            this.DataContext = ViewModel;

            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }

        #region IDisposable implementation

        private bool disposed = false;

        ~AdvancedCachingPage()
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadPhotos();
        }

        private async Task LoadPhotos(bool forceRefresh = false)
        {
            await photoLoader.GetRecentPhotos(ViewModel.UpdatePhotos, forceRefresh);
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButtonPagePrevious = new ApplicationBarIconButton(new Uri("/Assets/AppBar/back.png", UriKind.Relative));
            appBarButtonPagePrevious.Text = AppResources.AppBarPagePrevText;
            ApplicationBar.Buttons.Add(appBarButtonPagePrevious);
            appBarButtonPagePrevious.Click += appBarButtonPagePrevious_Click;
            ApplicationBarIconButton appBarButtonRefresh = new ApplicationBarIconButton(new Uri("/Assets/AppBar/refresh.png", UriKind.Relative));
            appBarButtonRefresh.Text = AppResources.AppBarRefreshText;
            ApplicationBar.Buttons.Add(appBarButtonRefresh);
            appBarButtonRefresh.Click += appBarButtonRefresh_Click;
            ApplicationBarIconButton appBarButtonForceRefresh = new ApplicationBarIconButton(new Uri("/Assets/AppBar/refresh.png", UriKind.Relative));
            appBarButtonForceRefresh.Text = AppResources.AppBarForceRefreshText;
            ApplicationBar.Buttons.Add(appBarButtonForceRefresh);
            appBarButtonForceRefresh.Click += appBarButtonForceRefresh_Click;
            ApplicationBarIconButton appBarButtonClearCache = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            appBarButtonClearCache.Text = AppResources.AppBarClearCacheText;
            ApplicationBar.Buttons.Add(appBarButtonClearCache);
            appBarButtonClearCache.Click += appBarButtonClearCache_Click;
        }
        private void appBarButtonPagePrevious_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SimpleCachingPage.xaml", UriKind.Relative));
        }

        private async void appBarButtonRefresh_Click(object sender, EventArgs e)
        {
            await LoadPhotos();
        }

        private async void appBarButtonForceRefresh_Click(object sender, EventArgs e)
        {
            await LoadPhotos(true);
        }

        private void appBarButtonClearCache_Click(object sender, EventArgs e)
        {
            photoLoader.ClearCache();
        }

    }
}