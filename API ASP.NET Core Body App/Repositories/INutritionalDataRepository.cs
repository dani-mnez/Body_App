using API_ASP.NET_Core_Body_App.Models.NutritionData;
using Microsoft.AspNetCore.JsonPatch;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public interface INutritionalDataRepository
    {
        // GET
        Task<NutritionalData> GetNutritionDataByIdAsync(string id);


        // POST
        Task AddNutritionDataAsync(NutritionalData nutritionData);
        Task<List<string>> AddMultipleNutritionalDataAsync(List<NutritionalData> nutritionalDataList);
        Task<List<NutritionalData>> GetAllListNutritionDataByIdAsync(List<string> nutritionDataIds);
        Task DeleteMultipleNutritionalDataByIdListAsync(List<string> idList);


        // PATCH
        Task UpdateGeneralAttributesAsync(string nutritionalDataId, JsonPatchDocument<NutritionalData> nutDataPatch);
        Task UpdateMultipleDocumentsGeneralAttributesAsync(Dictionary<string, JsonPatchDocument<NutritionalData>> nutritionalDataPatches);


        // PUT
        Task UpdateNutritionDataAsync(NutritionalData nutritionDataToUpdate);


        // DELETE
        Task DeleteNutritionDataByIdAsync(string id);
    }
}
