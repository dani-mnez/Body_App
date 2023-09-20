using API_ASP.NET_Core_Body_App.Models.FoodData;
using API_ASP.NET_Core_Body_App.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API_ASP.NET_Core_Body_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodDataController : ControllerBase
    {
        private readonly IFoodDataRepository _foodDataRepository;

        public FoodDataController(IFoodDataRepository foodDataRepository)
        {
            _foodDataRepository = foodDataRepository;
        }


        // GET
        [HttpGet("{foodDataId}")]
        public async Task<IActionResult> GetFoodData(string foodDataId)
        {
            var foodData = await _foodDataRepository.GetFoodDataByIdAsync(foodDataId);
            if (foodData == null)
            {
                return NotFound();
            }

            return Ok(foodData);
        }


        // PATCH
        [HttpPatch("{foodDataId}")]
        public async Task<IActionResult> UpdateGeneralFoodDataAsync(string foodDataId, JsonPatchDocument<FoodData> foodDataPatch)
        {
            await _foodDataRepository.UpdateGeneralAttributesAsync(foodDataId, foodDataPatch);
            return NoContent();
        }


        // POST
        [HttpPost]
        public async Task<IActionResult> AddFoodData([FromBody] FoodData foodData)
        {
            await _foodDataRepository.AddFoodDataAsync(foodData);
            return CreatedAtAction(
                nameof(GetFoodData),
                new { foodDataId = foodData.Id },
                foodData
            );
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> AddMultipleFoodData([FromBody] List<FoodData> foodDataList)
        {
            var foodDataIdList = await _foodDataRepository.AddMultipleFoodDataAsync(foodDataList);
            return Ok(foodDataIdList);
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllListFoodData([FromBody] List<string> foodDataIds)
        {
            var foodDataList = await _foodDataRepository.GetAllListFoodDataByIdAsync(foodDataIds);
            if (foodDataList == null)
            {
                return NotFound();
            }
            return Ok(foodDataList);
        }


        // PUT
        [HttpPut]
        public async Task<IActionResult> UpdateFoodDataPropertyById([FromBody] FoodData foodData)
        {
            await _foodDataRepository.UpdateFoodDataPropertyByIdAsync(foodData);
            return NoContent();
        }


        // DELETE
        [HttpDelete("{foodDataId}")]
        public async Task<IActionResult> DeleteFoodDataById(string foodDataId)
        {
            await _foodDataRepository.DeleteFoodDataByIdAsync(foodDataId);
            return NoContent();
        }
    }
}
