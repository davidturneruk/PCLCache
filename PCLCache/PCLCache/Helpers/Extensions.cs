namespace PortableCacheLibrary
{
    using PCLStorage;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Converts Uri to cache key extension method
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string ToCacheKey(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            string hashedResult = uri.GetHashCode().ToString();

            //http://stackoverflow.com/questions/3009284/using-regex-to-replace-invalid-characters
            string pattern = "[\\~#%&*{}/:<>?|\"-]";
            string replacement = " ";

            Regex regEx = new Regex(pattern);
            string sanitized = Regex.Replace(regEx.Replace(hashedResult, replacement), @"\s+", "_");

            return sanitized;
        }

        /// <summary>
        /// Extension method to check if file exist in folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> ContainsFileAsync(this IFolder folder, string fileName)
        {
            //This looks nicer, but gave a COM errors in some situations
            //TODO: Check again in final release of Windows 8 (or 9, or 10)
            //return (await folder.GetFilesAsync()).Where(file => file.Name == fileName).Any();

            try
            {
                await folder.GetFileAsync(fileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string FileType(this IFile file)
        {
            int index = file.Path.LastIndexOf(".");
            return file.Path.Substring(index + 1);
        }

    }
}
