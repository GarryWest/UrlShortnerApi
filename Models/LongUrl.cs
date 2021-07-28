using System;

namespace UrlShortnerApi.Models
{
    public class LongUrl
    {
        /// <summary>
        /// Id = \{Id} portion of short path
        /// </summary>
        public string Id { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
    }
}