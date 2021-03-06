﻿using Funda.Extensions;
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
        
        public async Task<IEnumerable<ObjectForSale>> GetEstateObjects(bool withTuinsOnly)
        {
            var url = withTuinsOnly ? Url + $"/tuin/&page={{0}}&pagesize={PageSize}" : Url + $"/&page={{0}}&pagesize={PageSize}";

            var firstPageResponse = await GetPage(string.Format(url, 1));
            if (firstPageResponse.TotaalAantalObjecten == 0) return new List<ObjectForSale>();

            var urls = Enumerable.Range(2, firstPageResponse.Paging.AantalPaginas - 1).Select(i => string.Format(url, i)).ToList();
            var requests = urls.Select(GetPage);
            var responses = (await Task.WhenAll(requests)).ToList();
            responses.Add(firstPageResponse);

            return responses.SelectMany(x => x.Objects).Where(x => x != null).ToList();
        }

        private async Task<FeedResponse> GetPage(string url)
        {
            var response = await _httpClient.GetAsyncWithPolicy(url);
            return JsonConvert.DeserializeObject<FeedResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
