using Funda.Extensions;
using Funda.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Funda.Services
{
    public class ApiService : IApiService
    {
        private const int PageSize = 25;
        private const string Url = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/amsterdam";

        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<IEnumerable<EstateAgent>> GetEstateAgents(int amount, bool orderByDescending) => 
            await GetAgents(Url + $"/&page={{0}}&pagesize={PageSize}", amount, orderByDescending);

        public async Task<IEnumerable<EstateAgent>> GetEstateAgentsOfObjectsWithTuins(int amount, bool orderByDescending) => 
            await GetAgents(Url + $"/tuin/&page={{0}}&pagesize={PageSize}", amount, orderByDescending);
            

        private async Task<IEnumerable<EstateAgent>> GetAgents(string url, int amount, bool orderByDescending)
        {
            var firstPageResult = await GetPage(string.Format(url, 1));
            if (firstPageResult.TotaalAantalObjecten == 0) return new List<EstateAgent>();

            var urls = Enumerable.Range(2, firstPageResult.Paging.AantalPaginas - 1).Select(i => string.Format(url, i)).ToList();
            var requests = urls.Select(async url => await GetPage(url));
            var responses = await Task.WhenAll(requests);

            var objectsForSale = responses.Where(x => x != null).SelectMany(x => x.Objects).ToList();
            objectsForSale.AddRange(firstPageResult.Objects);

            var groupedObjects = objectsForSale.GroupBy(x => x.MakelaarId);

            var sortedObjects = orderByDescending
                ? groupedObjects.OrderByDescending(x => x.Count())
                : groupedObjects.OrderBy(x => x.Count());

            return sortedObjects
                .Take(amount)
                .Select(x => new EstateAgent(x.Key, x.First().MakelaarNaam, x.Count()));
        }

        private async Task<FeedResponse> GetPage(string url)
        {
            var response = await _httpClient.GetAsyncWithPolicy(url);
            return JsonConvert.DeserializeObject<FeedResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
