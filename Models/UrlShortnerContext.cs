using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortnerApi.Models
{
    public class UrlShortnerContext: DbContext
    {
        public UrlShortnerContext(DbContextOptions<UrlShortnerContext> options) : base(options)
        {}

        public DbSet<ShortUrl> ShortUrls { get; set; }
    }
}
