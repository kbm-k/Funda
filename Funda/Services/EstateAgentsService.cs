using Funda.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Funda.Services
{
    public class EstateAgentsService : IEstateAgentsService
    {
        private readonly IApiService _apiService;

        public EstateAgentsService(IApiService apiService) => _apiService = apiService;

        public async Task<IEnumerable<EstateAgent>> GetEstateAgents(int amount, bool orderByDescending) =>
            await GetAgents(amount, false, orderByDescending);

        public async Task<IEnumerable<EstateAgent>> GetEstateAgentsOfObjectsWithTuins(int amount, bool orderByDescending) =>
            await GetAgents(amount, true, orderByDescending);

        private async Task<IEnumerable<EstateAgent>> GetAgents(int amount, bool withTuinsOnly, bool orderByDescending)
        {
            var objectsForSale = await _apiService.GetEstateObjects(withTuinsOnly);
            
            var groupedObjects = objectsForSale.GroupBy(x => x.MakelaarId);

            var sortedObjects = orderByDescending
                ? groupedObjects.OrderByDescending(x => x.Count())
                : groupedObjects.OrderBy(x => x.Count());

            return sortedObjects
                .Take(amount)
                .Select(x => new EstateAgent(x.Key, x.First().MakelaarNaam, x.Count()));
        }
    }
}
