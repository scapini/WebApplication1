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

[assembly : InternalsVisibleTo("XUnitTestProject1")]
namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var mockContext = new Mock<PersonContext>();
            var mockRepo = new Mock<Repository<Albums>>();
            var mockFact = new Mock<IHttpClientFactory>();
            HttpResponseMessage mockResp = new HttpResponseMessage();

            // Controller constructor
            var music = new MusicController(mockFact.Object, mockRepo.Object, mockContext.Object);

            // Key HttpClient setup
            var mockHandler = new Mock<HttpMessageHandler>();
            mockFact.Setup(x => x.CreateClient("")).Returns(new HttpClient(mockHandler.Object));

            // Mock SendAsync
            mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).
                ReturnsAsync(mockResp);

            mockResp.Content = new StringContent("{\"resultCount\":1}", Encoding.UTF8, "text/javascript");

            var ret = await music.GetArtist("zztop");

            Assert.Equal(1, ret.ResultCount);

        }

    }
}
