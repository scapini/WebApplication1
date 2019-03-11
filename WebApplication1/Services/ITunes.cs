using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApplication1.Contexts;
using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class Wrapper
    {
        public int ResultCount { get; set; }
        public IEnumerable<Albums> Results { get; set; }
    }

    public interface IITunesService
    {
        Task<Wrapper> GetAlbums(string id);
        Task<Wrapper> GetArtist(string name);
    }

    public class ITunesService : IITunesService
    {
        private PersonContext pc;
        private IHttpClientFactory http;
        public ITunesService(IHttpClientFactory fact, PersonContext pc)
        {
            this.http = fact;
            this.pc = pc;
        }

        public async Task<Wrapper> GetAlbums(string id)
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

            foreach (var album in result.Results)
            {
                //albums.Insert(album);
                if (album.CollectionId != 0)
                {
                    if (!pc.Albums.Any(o => o.CollectionId == album.CollectionId))
                    {
                        pc.Albums.Add(album);
                    }
                }
            }
            pc.SaveChanges();
            return result;
        }

        public async Task<Wrapper> GetArtist(string name)
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
    }
}
