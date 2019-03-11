using Moq;
using Moq.Protected;
using System;

using Microsoft.AspNetCore;
using WebApplication1.Contexts;
using WebApplication1.Controllers;
using System.Net.Http.Formatting;
using Xunit;
using System.Net.Http;
using WebApplication1.Model;
using System.Threading.Tasks;
using static WebApplication1.Controllers.MusicController;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text;
using WebApplication1.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

/**
 * Attempt at understanding mocking in C#/DotNet
 * 
 * Not pretty code yet, but uses 
 * - mocked DbContext
 * - mocked HTTPRequest
 * - verifying argument parameters.
 */
[assembly : InternalsVisibleTo("XUnitTestProject1")]
namespace XUnitTestProject1
{
    public class UnitTest1
    {
        // Tests that result from calling itunes REST API is handled correctly.
        [Fact]
        public async void TestArtist()
        {
            var mockContext = new Mock<PersonContext>();
            var mockRepo = new Mock<Repository<Albums>>();
            var mockFact = new Mock<IHttpClientFactory>();
            HttpResponseMessage mockResp = new HttpResponseMessage();

            // Music constructor
            var music = new ITunesService(mockFact.Object, mockContext.Object);

            // Key HttpClient setup
            var mockHandler = new Mock<HttpMessageHandler>();
            mockFact.Setup(x => x.CreateClient("")).Returns(new HttpClient(mockHandler.Object));

            // Mock SendAsync
            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).
                ReturnsAsync(mockResp);

            mockResp.Content = new StringContent("{\"resultCount\":1, \"results\" :[{\"artistId\":1, \"collectionId\":2,\"collectionName\":\"album\"}]}", Encoding.UTF8, "text/javascript");

            var ret = await music.GetArtist("zztop");

            Assert.Equal(1, ret.ResultCount);

        }

        [Fact]
        public async void TestAlbum()
        {
            var mockContext = new Mock<PersonContext>();
            var mockRepo = new Mock<Repository<Albums>>();
            var mockFact = new Mock<IHttpClientFactory>();
            HttpResponseMessage mockResp = new HttpResponseMessage();

            // Music constructor
            var music = new ITunesService(mockFact.Object, mockContext.Object);

            // Key HttpClient setup
            var mockHandler = new Mock<HttpMessageHandler>();
            mockFact.Setup(x => x.CreateClient("")).Returns(new HttpClient(mockHandler.Object));

            // Mock SendAsync
            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).
                ReturnsAsync(mockResp);

            // Return from itunes HTTP call
            mockResp.Content = new StringContent("{\"resultCount\":1, \"results\" :[{\"artistId\":1, \"collectionId\":2,\"collectionName\":\"album\"}]}", Encoding.UTF8, "text/javascript");

            // Mock repository
            var mockSet = new Mock<DbSet<Albums>>();
            IQueryable<Albums> empty = new List<Albums>().AsQueryable();

            // Return an actual queryable list.
            mockSet.As<IQueryable<Albums>>().Setup(m => m.Provider).Returns(empty.Provider);
            mockSet.As<IQueryable<Albums>>().Setup(m => m.Expression).Returns(empty.Expression);
            mockSet.As<IQueryable<Albums>>().Setup(m => m.ElementType).Returns(empty.ElementType);
            mockSet.As<IQueryable<Albums>>().Setup(m => m.GetEnumerator()).Returns(empty.GetEnumerator());

            mockContext.Setup(m => m.Albums).Returns(mockSet.Object);

            var ret = await music.GetAlbums("zztop");

            Assert.Equal(1, ret.ResultCount);

            // Verify correct object stored.
            mockSet.Verify(x => x.Add(It.Is<Albums>(a => a.ArtistId == 1 && a.CollectionId == 2 && a.CollectionName == "album")));
        }

    }
}
