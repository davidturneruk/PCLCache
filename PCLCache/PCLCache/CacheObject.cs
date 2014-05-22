namespace PCLCache
{
    using System;

    /// <summary>
    /// Used as a wrapper around the stored file to keep metadata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheObject
    {
        /// <summary>
        /// Expire date of cached file
        /// </summary>
        public DateTime? ExpireDateTime { get; set; }

        /// <summary>
        /// Is the cache file valid?
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (ExpireDateTime == null || ExpireDateTime.Value > DateTime.Now);
            }
        }
    }

    /// <summary>
    /// Used as a wrapper around the stored file to keep metadata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheObject<T> : CacheObject
    {
        /// <summary>
        /// Actual file being stored
        /// </summary>
        public T File { get; set; }
    }

}
