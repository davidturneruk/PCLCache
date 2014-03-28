namespace PortableCacheLibrary
{
    using PCLStorage;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Can cache Uri data
    /// </summary>
    public static class WebDataCache
    {
        private static readonly string CacheFolder = "_webdatacache";

        /// <summary>
        /// Stores webdata in cache based on uri as key
        /// Returns file
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="forceGet"></param>
        /// <returns></returns>
        public async static Task<IFile> GetAsync(Uri uri, bool forceGet = false, HttpClient client = null)
        {
            string key = uri.ToCacheKey();

            IFile file = null;

            //Try get the data from the cache
            var folder = await GetFolderAsync().ConfigureAwait(false);
            var exist = await folder.ContainsFileAsync(key).ConfigureAwait(false);

            //If file is not available or we want to force getting this file
            if (!exist || forceGet)
            {
                //else, load the data
                file = await SetAsync(uri, client).ConfigureAwait(false);
            }

            if (file == null)
                file = await folder.GetFileAsync(key);

            return file;
        }

        /// <summary>
        /// Get the cache folder to read/write
        /// </summary>
        /// <returns></returns>
        private static async Task<IFolder> GetFolderAsync()
        {
            var folder = FileSystem.Current.LocalStorage;

            if (!string.IsNullOrEmpty(CacheFolder))
            {
                folder = await folder.CreateFolderAsync(CacheFolder, CreationCollisionOption.OpenIfExists);
            }

            //IFolder.GetFolderFromPathAsync

            return folder;
        }

        /// <summary>
        /// Insert given uri in cache. Data will be loaded from the web
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static async Task<IFile> SetAsync(Uri uri, HttpClient client = null)
        {
            string key = uri.ToCacheKey();

            var folder = await GetFolderAsync();

            if (client == null)
                client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(uri).ConfigureAwait(false);

            //Save data to cache
            var file = await folder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenAsync(FileAccess.ReadAndWrite);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            return file;
        }

        /// <summary>
        /// Delete from cache based on Uri (=key)
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Task Delete(Uri uri)
        {
            return Task.Run(async () =>
            {
                var file = await GetAsync(uri).ConfigureAwait(false);

                await file.DeleteAsync();
            });
        }

        /// <summary>
        /// Clear the complete webcache
        /// </summary>
        /// <returns></returns>
        public static Task ClearAll()
        {
            return Task.Run(async () =>
            {
                var folder = await GetFolderAsync().ConfigureAwait(false);

                try
                {
                    await folder.DeleteAsync().ConfigureAwait(false);
                }
                catch (UnauthorizedAccessException)
                {
                }
            });
        }
    }
}
