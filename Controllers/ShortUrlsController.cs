using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortnerApi.Models;

namespace UrlShortnerApi.Controllers
{
    [Route("x")]
    [ApiController]
    public class ShortUrlsController : ControllerBase
    {
        private readonly UrlShortnerContext _context;

        public ShortUrlsController(UrlShortnerContext context)
        {
            _context = context;
        }

        // GET: x
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShortUrl>>> GetShortUrls()
        {
            return await _context.ShortUrls.ToListAsync();
        }

        // GET: x/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShortUrl>> GetShortUrl(string id)
        {
            string requestShortUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(Request);
            var ShortUrl = await _context.ShortUrls.FindAsync(requestShortUrl);

            if (ShortUrl == null)
            {
                return NotFound();
            }

            return ShortUrl;
        }

        // POST: x
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<string>> PostShortUrl(ShortUrl shortUrl)
        {
            var existUrl = await _context.ShortUrls.FirstOrDefaultAsync(m => m.LongUrl == shortUrl.LongUrl);
            if (existUrl == null) {
                ShortUrl newUrl = new() { ShortUrlId = ShortUrl(), LongUrl = shortUrl.LongUrl, CreateDate = DateTime.Now };
                _context.ShortUrls.Add(newUrl);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                        throw;
                }
                return CreatedAtAction(nameof(GetShortUrl), new { id = newUrl.ShortUrlId }, newUrl);
            } else
            {
                return AcceptedAtAction(nameof(GetShortUrl), new { id = existUrl.ShortUrlId }, existUrl);
            }
            
        }

        public string ShortUrl()
        {
            ulong counter = Zookeeper.Counter;
            var s = "012345689abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray();
            string hash_str = "";
            while (counter > 0)
            {
                hash_str = s[counter % 62] + hash_str;
                counter /= 62;
            }
            string shortUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(Request);
            return shortUrl + "/" + hash_str;
        }
    }
}
