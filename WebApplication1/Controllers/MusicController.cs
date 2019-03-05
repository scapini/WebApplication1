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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly IHttpClientFactory http;

        public class Music {
            public string artistName { get; set; }
            public string collectionName { get; set; }
            public int artistId { get; set; }
        }

        public class Results
        {
            public int resultCount { get; set; }
            public IEnumerable<Music> results { get; set; }
        }

        public MusicController(IHttpClientFactory fact)
        {
            this.http = fact;
        }
        // GET: api/Music
        [Route("artist/{id}")]
        public async Task<Results> Get(string id)
        {
            String url = QueryHelpers.AddQueryString("https://itunes.apple.com/lookup?entity=album", "id", id);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");

            var client = http.CreateClient();

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var xx = new JsonMediaTypeFormatter();
            xx.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var result = await response.Content.ReadAsAsync<Results>(new[] { xx });
            return result;
        }

        [Route("artist")]
        public async Task<Results> GetArtist([FromQuery] string name)
        {
            String url = QueryHelpers.AddQueryString("https://itunes.apple.com/search?entity=musicArtist", "term", name);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");

            var client = http.CreateClient();

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var xx = new JsonMediaTypeFormatter();
            xx.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var result = await response.Content.ReadAsAsync<Results>(new[] { xx });
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
