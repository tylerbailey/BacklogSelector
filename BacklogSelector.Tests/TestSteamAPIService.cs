using BacklogBrowser.Exceptions;
using BacklogBrowser.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BacklogBrowser.Tests
{
    public class TestSteamAPIService
    {
        [Fact]
        public async void TestGetIdGood()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(conf => conf["AppSettings:APIKey"]).Returns("testkey");
            var httpClientFactory = SetupHttpClientFactory("{\"response\":{\"steamid\":\"12345\",\"success\":1}}");
           
            ISteamAPIService apiService = new SteamAPIService(config.Object, httpClientFactory);
            var steamId = await apiService.GetSteamId("testId");
            Assert.True("12345".Equals(steamId));
        }

        [Fact]
        public async void TestGetIdBad()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(conf => conf["AppSettings:APIKey"]).Returns("testkey");           
            var httpClientFactory = SetupHttpClientFactory("{\"response\":{\"success\":42,\"message\":\"No match\"}}");

            ISteamAPIService apiService = new SteamAPIService(config.Object, httpClientFactory);
            await Assert.ThrowsAsync<SteamUserException>(async () => await apiService.GetSteamId("testId"));
        }

        [Fact]
        public async void TestGetSelectedGames()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(conf => conf["AppSettings:APIKey"]).Returns("testkey");
            var httpClientFactory = SetupHttpClientFactory("{\"response\": {\"game_count\": 1,\"games\":[{\"appid\": 70,\"name\": \"Half-Life\",\"playtime_forever\": 0,\"img_icon_url\": \"95be6d131fc61f145797317ca437c9765f24b41c\",\"img_logo_url\": \"6bd76ff700a8c7a5460fbae3cf60cb930279897d\",\"has_community_visible_stats\": true,\"playtime_windows_forever\": 0,\"playtime_mac_forever\": 0,\"playtime_linux_forever\": 0}]}}");
           
            ISteamAPIService apiService = new SteamAPIService(config.Object, httpClientFactory);
            var game = await apiService.GetSelectedGame("steamId");
            Assert.True(game.Name.Equals("Half-Life"));
        }

        [Fact]
        public async void TestGetSelectedGamesNoneFound()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(conf => conf["AppSettings:APIKey"]).Returns("testkey");
            var httpClientFactory = SetupHttpClientFactory("{\"response\": {\"game_count\": 0,\"games\":[]}}");

            ISteamAPIService apiService = new SteamAPIService(config.Object, httpClientFactory);
            await Assert.ThrowsAsync<SteamGamesException>(async () => await apiService.GetSelectedGame("steamId"));
            
        }

        [Fact]
        public async void TestGetSelectedGamesBadId()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(conf => conf["AppSettings:APIKey"]).Returns("testkey");
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("<html><head><title>500 Internal Server Error </title></head><body><h1>Internal Server Error</h1></body></html>")

            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/address")
            };

            var httpClientFactory = SetupHttpClientFactory("<html><head><title>500 Internal Server Error </title></head><body><h1>Internal Server Error</h1></body></html>");

            ISteamAPIService apiService = new SteamAPIService(config.Object, httpClientFactory);
            await Assert.ThrowsAsync<SteamGamesException>(async () => await apiService.GetSelectedGame("steamId"));
        }

        private IHttpClientFactory SetupHttpClientFactory(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)

            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/address")
            };           
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return httpClientFactory.Object;
        }
    }
}
