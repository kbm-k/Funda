using Funda.Models;
using Funda.Services;
using Moq;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Funda.Tests
{
    public class ApiServiceTests
    {
        private readonly List<FeedResponse> _responses;

        public ApiServiceTests()
        {
            var serializedResponses = File.ReadAllText("TestData.json");
            _responses = JsonConvert.DeserializeObject<List<FeedResponse>>(serializedResponses);
        }

        [Fact]
        public async Task GetAgentsTest()
        {
            const string firstPage = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/amsterdam/&page=1&pagesize=25";
            const string secondPage = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/amsterdam/&page=2&pagesize=25";

            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.SetupRequest(HttpMethod.Get, firstPage).ReturnsResponse(JsonConvert.SerializeObject(_responses[0]));
            messageHandler.SetupRequest(HttpMethod.Get, secondPage).ReturnsResponse(JsonConvert.SerializeObject(_responses[1]));

            var client = messageHandler.CreateClient();
            var apiService = new ApiService(client);

            var estateAgents = (await apiService.GetEstateAgents(10, true)).ToList();

            CompareIdsAndCounts(34, 10, estateAgents[0]);
            CompareIdsAndCounts(35, 9, estateAgents[1]);
            CompareIdsAndCounts(2, 8, estateAgents[2]);
            CompareIdsAndCounts(15, 7, estateAgents[3]);
            CompareIdsAndCounts(6, 5, estateAgents[4]);
            CompareIdsAndCounts(30, 4, estateAgents[5]);
            CompareIdsAndCounts(25, 3, estateAgents[6]);
        }

        private static void CompareIdsAndCounts(int expectedId, int expectedCount, EstateAgent estateAgent)
        {
            Assert.Equal(expectedId, estateAgent.Id);
            Assert.Equal(expectedCount, estateAgent.Count);
        }
    }
}
