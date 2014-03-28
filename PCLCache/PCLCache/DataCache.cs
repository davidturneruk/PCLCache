﻿namespace PortableCacheLibrary
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Stores objects as json or xml in the localstorage
    /// </summary>
    public static class DataCache
    {
        private static readonly string CacheFolder = "_jsoncache";

        /// <summary>
        /// Get object based on key, or generate the value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="generate"></param>
        /// <param name="expireDate"></param>
        /// <param name="forceRefresh"></param>
        /// <param name="serializerType">JSON or XML serializer</param>
        /// <returns></returns>
        public async static Task<T> GetAsync<T>(string key, Func<Task<T>> generate, DateTime? expireDate = null, bool forceRefresh = false, StorageSerializer serializerType = StorageSerializer.JSON)
        {
            object value;

            //Force bypass of cache?
            if (!forceRefresh)
            {
                //Check cache
                value = await GetFromCache<T>(key, serializerType).ConfigureAwait(false);
                if (value != null)
                {
                    return (T)value;
                }
            }

            value = await generate().ConfigureAwait(false);
            await Set(key, value, expireDate).ConfigureAwait(false);

            return (T)value;

        }

        /// <summary>
        /// Get value from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="serializerType">JSON or XML serializer</param>
        /// <returns></returns>
        public async static Task<T> GetFromCache<T>(string key, StorageSerializer serializerType = StorageSerializer.JSON)
        {
            IStorageHelper<CacheObject<T>> storage = new StorageHelper<CacheObject<T>>(StorageType.Local, CacheFolder, serializerType);

            //Get cache value
            var value = await storage.LoadAsync(key).ConfigureAwait(false);

            if (value == null)
                return default(T);
            else if (value.IsValid)
                return value.File;
            else
            {
                //Delete old value
                //Do not await
                Delete(key, serializerType);

                return default(T);
            }
        }

        /// <summary>
        /// Set value in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireDate"></param>
        /// <param name="serializerType">JSON or XML serializer</param>
        /// <returns></returns>
        public static Task Set<T>(string key, T value, DateTime? expireDate = null, StorageSerializer serializerType = StorageSerializer.JSON)
        {
            IStorageHelper<CacheObject<T>> storage = new StorageHelper<CacheObject<T>>(StorageType.Local, CacheFolder, serializerType);

            CacheObject<T> cacheFile = new CacheObject<T>() { File = value, ExpireDateTime = expireDate };

            return storage.SaveAsync(cacheFile, key);
        }

        /// <summary>
        /// Delete key from cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="serializerType">JSON or XML serializer</param>
        /// <returns></returns>
        public static Task Delete(string key, StorageSerializer serializerType = StorageSerializer.JSON)
        {
            IStorageHelper<object> storage = new StorageHelper<object>(StorageType.Local, CacheFolder, serializerType);

            return storage.DeleteAsync(key);
        }

        /// <summary>
        /// Clear the complete cache
        /// </summary>
        /// <returns></returns>
        public static Task ClearAll()
        {
            IStorageHelper<object> storage = new StorageHelper<object>(StorageType.Local, CacheFolder);
            return storage.DeleteAllFiles();
        }

        /// <summary>
        /// Clear expired cache files
        /// </summary>
        /// <returns></returns>
        public static async Task ClearInvalid(StorageSerializer serializerType = StorageSerializer.JSON)
        {
            StorageHelper<CacheObject> storage = new StorageHelper<CacheObject>(StorageType.Local, CacheFolder, serializerType);
            var validExtension = storage.GetFileExtension();
            var folder = await storage.GetFolderAsync().ConfigureAwait(false);

            var files = await folder.GetFilesAsync();

            foreach (var file in files.Where(x => x.FileType() == validExtension))
            {
                var loadedFile = await storage.LoadAsync(file.Name).ConfigureAwait(false);

                if (loadedFile != null && !loadedFile.IsValid)
                    await file.DeleteAsync();
            }

        }
    }
}
