using API_ASP.NET_Core_Body_App.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace API_ASP.NET_Core_Body_App.Repositories
{
    public interface IUserRepository
    {
        // GET
        Task<User> GetUserByIdAsync(string id);


        //POST
        Task AddUserAsync(User user);
        Task<User> GetUserByCredentialsAsync(string username, string password);


        // PATCH
        Task UpdateGeneralAttributesAsync(string userId, JsonPatchDocument<User> userDataPatch);


        // DELETE
        Task DeleteUserByIdAsync(string id);
        Task DeleteFromHistoricalDataArrayByIdAsync(string userId, string historicalDataId);
        Task DeleteFromMealArrayByIdAsync(string userId, string mealId);
    }
}
