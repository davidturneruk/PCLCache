namespace PCLCache
{

    using Newtonsoft.Json;
    using PCLStorage;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Save object to local storage, serializes as json and writes object to a file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StorageHelper<T> : IStorageHelper<T>
    {
        private StorageType _storageType;
        private string _subFolder;
        private StorageSerializer _serializerType;



        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="StorageType"></param>
        /// <param name="subFolder"></param>
        /// <param name="serializerType"></param>
        public StorageHelper(StorageType StorageType, string subFolder = null, StorageSerializer serializerType = StorageSerializer.JSON)
        {
            _storageType = StorageType;
            _subFolder = subFolder;
            _serializerType = serializerType;
        }

        /// <summary>
        /// Gets file extension based on serializer type
        /// Never deserialize with the wrong serializer
        /// </summary>
        /// <returns></returns>
        internal string GetFileExtension()
        {
            switch (_serializerType)
            {
                case StorageSerializer.JSON:
                    return ".json";
                case StorageSerializer.XML:
                    return ".xml";
            }

            return string.Empty;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName"></param>
        public async Task DeleteAsync(string fileName)
        {
            fileName = fileName + GetFileExtension();
            try
            {
                IFolder folder = await GetFolderAsync().ConfigureAwait(false);

                var contains = await folder.ContainsFileAsync(fileName).ConfigureAwait(false);
                if (contains)
                {
                    var file = await folder.GetFileAsync(fileName);
                    await file.DeleteAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save object from file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public async Task SaveAsync(T obj, string fileName)
        {

            fileName = fileName + GetFileExtension();
            try
            {
                if (obj != null)
                {
                    //Get file
                    IFile file = null;
                    IFolder folder = await GetFolderAsync().ConfigureAwait(false);
                    file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);


                    //Serialize object with JSON or XML serializer
                    switch (_serializerType)
                    {
                        case StorageSerializer.JSON:
                            var text = JsonConvert.SerializeObject(obj);
                            //Write content to file
                            using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                byte[] content = Encoding.UTF8.GetBytes(text);
                                await stream.WriteAsync(content, 0, content.Length);
                                await stream.FlushAsync();
                            }
                            break;
                        case StorageSerializer.XML:

                            using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                XmlSerializer serializer = new XmlSerializer(typeof(T));
                                serializer.Serialize(stream, obj);
                                await stream.FlushAsync();
                            }
                            break;
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Load object from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<T> LoadAsync(string fileName)
        {
            fileName = fileName + GetFileExtension();
            try
            {

                IFile file = null;
                IFolder folder = await GetFolderAsync().ConfigureAwait(false);

                var contains = await folder.ContainsFileAsync(fileName).ConfigureAwait(false);
                if (contains)
                {
                    file = await folder.GetFileAsync(fileName);

                    //Deserialize to object with JSON or XML serializer
                    T result = default(T);

                    switch (_serializerType)
                    {
                        case StorageSerializer.JSON:
                            using (Stream stream = await file.OpenAsync(FileAccess.Read))
                            {
                                byte[] content = new byte[stream.Length];
                                await stream.ReadAsync(content, 0, (int)stream.Length);
                                var data = Encoding.UTF8.GetString(content, 0, content.Length);
                                result = JsonConvert.DeserializeObject<T>(data);
                            
                            }
                            break;
                        case StorageSerializer.XML:
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            using (Stream stream = await file.OpenAsync(FileAccess.Read))
                            {
                                result = (T)serializer.Deserialize(stream);
                                stream.Dispose();
                            }
                            break;
                    }

                    return result;
                }
                else
                {
                    return default(T);
                }

            }
            catch (Exception)
            {
                //Unable to load contents of file
                throw;
            }
        }

        /// <summary>
        /// Get folder based on storagetype
        /// </summary>
        /// <returns></returns>
        public async Task<IFolder> GetFolderAsync()
        {
            IFolder folder;
            switch (_storageType)
            {
                case StorageType.Local:
                    folder = FileSystem.Current.LocalStorage;
                    break;
                case StorageType.Roaming:
                    folder = FileSystem.Current.RoamingStorage;
                    break;
                default:
                    throw new Exception(String.Format("Unknown StorageType: {0}", _storageType));
            }

            if (!string.IsNullOrEmpty(_subFolder))
            {
                folder = await folder.CreateFolderAsync(_subFolder, CreationCollisionOption.OpenIfExists);
            }

            return folder;
        }

        /// <summary>
        /// Deletes all files in current folder
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllFiles()
        {
            IFolder folder = await GetFolderAsync().ConfigureAwait(false);

            try
            {
                await folder.DeleteAsync();
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>
        /// Clear the complete cache
        /// </summary>
        /// <returns></returns>
        public static Task ClearLocalAll()
        {
            return Task.Run(async () =>
            {
                StorageHelper<object> storage = new StorageHelper<object>(StorageType.Local);
                var folder = await storage.GetFolderAsync().ConfigureAwait(false);

                //Remove subfolders
                foreach (var sub in await folder.GetFoldersAsync())
                {
                    try
                    {
                        await sub.DeleteAsync();
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                //Remove files
                foreach (var file in await folder.GetFilesAsync())
                {
                    try
                    {
                        await file.DeleteAsync();
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            });
        }

    }
}
