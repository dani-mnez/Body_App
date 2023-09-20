using API_ASP.NET_Core_Body_App.Models;
using API_ASP.NET_Core_Body_App.Models.RequestModels;
using API_ASP.NET_Core_Body_App.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace API_ASP.NET_Core_Body_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET
        [HttpGet("{userId}/{key?}")]
        public async Task<IActionResult> GetUserData(string userId, string? key = null)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (key != null)
            {
                Type userType = user.GetType();
                PropertyInfo[] propertyInfoList = userType.GetProperties();
                var propiedadSiExiste = propertyInfoList.FirstOrDefault(p =>
                    p.Name.Equals(key, StringComparison.OrdinalIgnoreCase)
                );

                if (propiedadSiExiste != null)
                {
                    return Ok(propiedadSiExiste.GetValue(user));
                }
                else
                {
                    return BadRequest("La propiedad no se ha encontrado");
                }
            }
            else
            {
                return Ok(user);
            };
        }


        // POST
        [HttpPost("authenticate")]
        public async Task<IActionResult> GetUserDataByPwdMail([FromBody] UserCredentials userCredentials)
        {
            var user = await _userRepository.GetUserByCredentialsAsync(userCredentials.Mail, userCredentials.Password);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            await _userRepository.AddUserAsync(user);

            return CreatedAtAction(
                nameof(GetUserData),
                new { id = user.Id },
                user
            );
        }


        // PATCH
        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] JsonPatchDocument<User> userDataPatch)
        {
            await _userRepository.UpdateGeneralAttributesAsync(userId, userDataPatch);
            return NoContent();
        }


        // DELETE
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userRepository.DeleteUserByIdAsync(userId);
            return NoContent();
        }

        [HttpDelete("{userId}/historicalData/{historicalDataId}")]
        public async Task<IActionResult> DeleteFromUserHistoricalDataArray(string userId, string historicalDataId)
        {
            await _userRepository.DeleteFromHistoricalDataArrayByIdAsync(userId, historicalDataId);
            return NoContent();
        }

        [HttpDelete("{userId}/meals/{mealId}")]
        public async Task<IActionResult> DeleteFromUserMealsArray(string userId, string mealId)
        {
            await _userRepository.DeleteFromMealArrayByIdAsync(userId, mealId);
            return NoContent();
        }
    }
}
