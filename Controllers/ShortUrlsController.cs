using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortnerApi.Models;

namespace UrlShortnerApi.Controllers
{
    [Route("")]
    [ApiController]
    public class ShortUrlsController : ControllerBase
    {
        private readonly UrlShortnerContext _context;

        public ShortUrlsController(UrlShortnerContext context)
        {
            _context = context;
        }

        // TODO: Hide this
        //GET: /
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShortUrl>>> GetShortUrls()
        {
            return await _context.ShortUrls.ToListAsync();
        }

        // GET: /5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShortUrl>> GetShortUrl(string id)
        {
            string requestShortUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(Request);
            var ShortUrl = await _context.ShortUrls.FindAsync(id);

            if (ShortUrl == null)
            {
                return NotFound();
            }

            //return ShortUrl;
            return Redirect(ShortUrl.LongUrl);
        }

        // POST: /addurl
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("addurl")]
        [HttpPost]
        public async Task<ActionResult<string>> PostShortUrl(ShortUrl shortUrl)
        {
            bool isValidApiDevKey = true;
            // TODO: Validate API Key
            if (isValidApiDevKey) {
                var existUrl = await _context.ShortUrls.FirstOrDefaultAsync(m => m.LongUrl == shortUrl.LongUrl);
                if (existUrl == null) {
                    ShortUrl newUrl = new() { ShortUrlId = GetShortUrlId(), LongUrl = shortUrl.LongUrl, ApiDevKey = shortUrl.ApiDevKey, CreateDate = DateTime.Now };
                    _context.ShortUrls.Add(newUrl);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        throw;
                    }
                    string returnShortUrl = $"{Request.Scheme}://{Request.Host}/{newUrl.ShortUrlId}";
                    return CreatedAtAction(nameof(GetShortUrl), new { id = newUrl.ShortUrlId }, 
                        new ReturnShortUrl {ShortUrl = returnShortUrl });
                } else
                {
                    string returnShortUrl = $"{Request.Scheme}://{Request.Host}/{existUrl.ShortUrlId}";
                    return AcceptedAtAction(nameof(GetShortUrl), new { id = existUrl.ShortUrlId },
                        new ReturnShortUrl { ShortUrl = returnShortUrl });
                }
            } else
            {
                return Unauthorized();
            }
            
        }

        public string GetShortUrlId()
        {
            ulong counter = Zookeeper.Counter;
            var s = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray();
            string hash_str = "";
            while (counter > 0)
            {
                hash_str = s[counter % 62] + hash_str;
                counter /= 62;
            }
            
            return hash_str;
        }
    }
}
