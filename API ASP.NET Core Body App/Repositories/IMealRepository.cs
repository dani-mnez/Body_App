using API_ASP.NET_Core_Body_App.Models.UserMeal;
using Microsoft.AspNetCore.JsonPatch;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public interface IMealRepository
    {
        // GET
        Task<Meal> GetMealByIdAsync(string mealId);


        // POST
        Task AddMealAsync(Meal meal);
        Task<List<Meal>> GetAllListMealByIdAsync(List<string> mealIds);


        // PATCH
        Task UpdateMealPropertyByIdAsync(string userId, JsonPatchDocument<Meal> userDataPatch);


        // DELETE
        Task DeleteMealByIdAsync(string mealId);
    }
}
