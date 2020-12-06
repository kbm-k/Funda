using Funda.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Funda.Services
{
    public interface IApiService
    {
        Task<IEnumerable<EstateAgent>> GetEstateAgents(int amount, bool orderByDescending);

        Task<IEnumerable<EstateAgent>> GetEstateAgentsOfObjectsWithTuins(int amount, bool orderByDescending);
    }
}