using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortnerApi.Models;

namespace UrlShortnerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LongUrlsController : ControllerBase
    {
        private readonly UrlShortnerContext _context;

        public LongUrlsController(UrlShortnerContext context)
        {
            _context = context;
        }

        // GET: api/LongUrls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LongUrl>>> GetLongUrls()
        {
            return await _context.LongUrls.ToListAsync();
        }

        // GET: api/LongUrls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LongUrl>> GetLongUrl(string id)
        {
            string requestPath = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(Request);
            var longUrl = await _context.LongUrls.FindAsync(requestPath);

            if (longUrl == null)
            {
                return NotFound();
            }

            return longUrl;
        }

        // POST: api/LongUrls
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<string>> PostLongUrl(LongUrl longUrl)
        {
            var existUrl = await _context.LongUrls.FirstOrDefaultAsync(m => m.Path == longUrl.Path);
            if (existUrl == null) {
                LongUrl newUrl = new() { Id = ShortUrl(), Path = longUrl.Path, CreateDate = DateTime.Now };
                _context.LongUrls.Add(newUrl);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                        throw;
                }
                return CreatedAtAction(nameof(GetLongUrl), new { id = newUrl.Id }, newUrl);
            } else
            {
                return AcceptedAtAction(nameof(GetLongUrl), new { id = existUrl.Id }, existUrl);
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
            string requestPath = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(Request);
            return requestPath + "/" + hash_str;
        }
    }
}
