using System;

namespace UrlShortnerApi.Models
{
    public class ShortUrl
    {
        /// <summary>
        /// Id = \{Id} portion of short path
        /// </summary>
        public string ShortUrlId { get; set; }
        public string LongUrl { get; set; }
        public string ApiDevKey { get; set; }
        public DateTime CreateDate { get; set; }
    }
}