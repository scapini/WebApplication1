using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebApplication1.Contexts;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly IHttpClientFactory http;

        public class Wrapper
        {
            public int ResultCount { get; set; }
            public IEnumerable<Albums> Results { get; set; }
        }

        private IRepository<Albums> albums;
        private PersonContext pc;

        public MusicController(IHttpClientFactory fact, IRepository<Albums> rep, PersonContext pc)
        {
            this.http = fact;
            this.albums = rep;
            this.pc = pc;
        }
        // GET: api/Music  get albums.
        /// <summary>
        /// Get albums (store them as well, for laughs).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("artist/{id}")]
        public async Task<Wrapper> Get(string id)
        {
            /* Hopefully formats URL correctly */
            String url = QueryHelpers.AddQueryString("https://itunes.apple.com/lookup?entity=album", "id", id);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");

            var client = http.CreateClient();

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            /* itunes doesn't send application/json */
            var xx = new JsonMediaTypeFormatter();
            xx.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var result = await response.Content.ReadAsAsync<Wrapper>(new[] { xx });

            foreach (var album in result.Results) {
                //albums.Insert(album);
                if (album.CollectionId != 0)
                {
                    pc.Albums.Update(album);
                }
            }
            pc.SaveChanges();
            return result;
        }

        [Route("artist")]
        public async Task<Wrapper> GetArtist([FromQuery] string name)
        {
            String url = QueryHelpers.AddQueryString("https://itunes.apple.com/search?entity=musicArtist", "term", name);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");

            var client = http.CreateClient();

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var xx = new JsonMediaTypeFormatter();
            xx.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var result = await response.Content.ReadAsAsync<Wrapper>(new[] { xx });
            return result;
        }

        // GET: api/Music/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Music
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Music/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
