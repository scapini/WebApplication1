using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebApplication1.Contexts;
using WebApplication1.Model;
using WebApplication1.Services;

/// <summary>
/// Trivial authenticated controller for 'itunes' music.
/// </summary>
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MusicController : ControllerBase
    {
        private readonly IHttpClientFactory http;

        private IRepository<Albums> albums;
        private PersonContext pc;
        private IITunesService itunes;

        public MusicController(IHttpClientFactory fact, IRepository<Albums> rep, PersonContext pc,
            IITunesService itunes)
        {
            this.http = fact;
            this.albums = rep;
            this.pc = pc;
            this.itunes = itunes;
        }

        // GET: api/Music  get albums.
        /// <summary>
        /// Get albums for an artist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("artist/{id}")]
        public async Task<Wrapper> Get(string id)
        {
            return await itunes.GetAlbums(id);

        }

        /// <summary>
        /// Search for an artist,
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("artist")]
        public async Task<Wrapper> GetArtist([FromQuery] string name)
        {
            return await itunes.GetArtist(name);
        }

    }
}
