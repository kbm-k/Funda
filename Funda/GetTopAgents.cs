using Funda.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Funda
{
    public class GetTopAgents
    {
        private readonly IApiService _apiService;

        public GetTopAgents(IApiService apiService) => _apiService = apiService;

        [FunctionName("GetTopAgents")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request)
        {
            try
            {
                var topAgents = new
                {
                    EstateAgents = await _apiService.GetEstateAgents(10, true),
                    EstateAgentsOfObjectsWithTuins = await _apiService.GetEstateAgentsOfObjectsWithTuins(10, true),
                };

                return new OkObjectResult(JsonConvert.SerializeObject(topAgents));
            }
            catch (Exception)
            {
                return new InternalServerErrorResult();
            }
        }
    }
}
