using API_ASP.NET_Core_Body_App.Models.FoodData;
using Microsoft.AspNetCore.JsonPatch;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public interface IFoodDataRepository
    {
        // GET
        Task<FoodData> GetFoodDataByIdAsync(string id);

        // POST
        Task AddFoodDataAsync(FoodData foodData);
        Task<List<string>> AddMultipleFoodDataAsync(List<FoodData> foodDataList);

        // PATCH
        Task UpdateGeneralAttributesAsync(string foodDataId, JsonPatchDocument<FoodData> foodDataPatch);

        // PUT
        Task UpdateFoodDataPropertyByIdAsync(FoodData foodData);
        Task<List<FoodData>> GetAllListFoodDataByIdAsync(List<string> foodDataIds);

        // DELETE
        Task DeleteFoodDataByIdAsync(string id);
    }
}
