using API_ASP.NET_Core_Body_App.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public interface IPhysicalDataRepository
    {
        // GET
        Task<PhysicalData> GetPhysicalDataByIdAsync(string id);

        // POST
        Task AddPhysicalDataAsync(PhysicalData data);
        Task<List<PhysicalData>> GetAllPhysicalDataAsync(List<string> physicalDataIDs);

        // PATCH
        Task UpdatePhysicalDataByIdAsync(string physDataId, JsonPatchDocument<PhysicalData> physDataPatch);

        // DELTE
        Task DeletePhysicalDataByIdAsync(string id);
    }
}
