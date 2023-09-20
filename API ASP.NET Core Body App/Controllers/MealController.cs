using API_ASP.NET_Core_Body_App.Models.UserMeal;
using API_ASP.NET_Core_Body_App.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API_ASP.NET_Core_Body_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController : ControllerBase
    {
        private readonly IMealRepository _mealRepository;

        public MealController(IMealRepository mealRepository)
        {
            _mealRepository = mealRepository;
        }

        // GET
        [HttpGet("{mealId}")]
        public async Task<IActionResult> GetMealData(string mealId)
        {
            var meal = await _mealRepository.GetMealByIdAsync(mealId);
            if (meal == null)
            {
                return NotFound();
            }
            return Ok(meal);
        }


        // POST
        [HttpPost]
        public async Task<IActionResult> AddMeal([FromBody] Meal meal)
        {
            await _mealRepository.AddMealAsync(meal);

            return CreatedAtAction(
                nameof(GetMealData),
                new { mealId = meal.Id },
                meal
            );
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllListMeal([FromBody] List<string> mealIds)
        {
            var mealList = await _mealRepository.GetAllListMealByIdAsync(mealIds);
            if (mealList == null)
            {
                return NotFound();
            }

            return Ok(mealList);
        }


        // PATCH
        [HttpPatch("{mealId}")]
        public async Task<IActionResult> UpdateMealAsync(string mealId, [FromBody] JsonPatchDocument<Meal> mealDataPatch)
        {
            await _mealRepository.UpdateMealPropertyByIdAsync(mealId, mealDataPatch);
            return NoContent();
        }


        // DELETE
        [HttpDelete("{mealId}")]
        public async Task<IActionResult> DeleteMealById(string mealId)
        {
            await _mealRepository.DeleteMealByIdAsync(mealId);
            return NoContent();
        }
    }
}
