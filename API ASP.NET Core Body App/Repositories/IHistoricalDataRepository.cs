using API_ASP.NET_Core_Body_App.Models.HistoricData;
using Microsoft.AspNetCore.JsonPatch;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public interface IHistoricalDataRepository
    {
        // GET
        Task<HistoricalData> GetHistoricalDataByIdAsync(string id);


        // POST
        Task AddHistoricalDataAsync(HistoricalData historicalData);
        Task<List<HistoricalData>> GetAllListHistoricalDataByIdAsync(List<string> historicalDataIds);
        Task DeleteMultipleHistoricalDataByIdListAsync(List<string> idList);

        // PATCH
        Task UpdateSingleDocAttributesAsync(string histDataId, JsonPatchDocument<HistoricalData> histDataPatch);
        Task UpdateMultipleDocsAttributesAsync(Dictionary<string, JsonPatchDocument<HistoricalData>> histDataPatches);


        // DELETE
        Task DeleteHistoricalDataByIdAsync(string id);
    }
}