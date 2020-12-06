using System.Collections.Generic;
using System.Threading.Tasks;
using Funda.Models;

namespace Funda.Services
{
    public interface IApiService
    {
        Task<IEnumerable<ObjectForSale>> GetEstateObjects(bool withTuinsOnly);
    }
}