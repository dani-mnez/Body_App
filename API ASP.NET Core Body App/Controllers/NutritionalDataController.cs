using API_ASP.NET_Core_Body_App.Models.NutritionData;
using API_ASP.NET_Core_Body_App.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API_ASP.NET_Core_Body_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionalDataController : ControllerBase
    {
        private readonly INutritionalDataRepository _nutritionalDataRepository;

        public NutritionalDataController(INutritionalDataRepository nutritionalDataRepository)
        {
            _nutritionalDataRepository = nutritionalDataRepository;
        }


        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNutritionalData(string id)
        {
            var nutritionalData = await _nutritionalDataRepository.GetNutritionDataByIdAsync(id);
            if (nutritionalData == null)
            {
                return NotFound();
            }
            return Ok(nutritionalData);
        }


        // POST
        [HttpPost]
        public async Task<IActionResult> CreateNutritionalData([FromBody] NutritionalData nutritionalDataToAdd)
        {
            await _nutritionalDataRepository.AddNutritionDataAsync(nutritionalDataToAdd);
            return CreatedAtAction(
                nameof(GetNutritionalData),
                new { id = nutritionalDataToAdd.Id },
                nutritionalDataToAdd
            );
        }

        [HttpPost("manyDeletes")]
        public async Task<IActionResult> DeleteManyNutritionalDataAsync([FromBody] List<string> ids)
        {
            await _nutritionalDataRepository.DeleteMultipleNutritionalDataByIdListAsync(ids);
            return NoContent();
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> CreateMultipleNutritionalData([FromBody] List<NutritionalData> nutritionalDataList)
        {
            var nutIdList = await _nutritionalDataRepository.AddMultipleNutritionalDataAsync(nutritionalDataList);
            return Ok(nutIdList);
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllListNutritionalData([FromBody] List<string> nutritionalDataIds)
        {
            var nutritionalDataList = await _nutritionalDataRepository.GetAllListNutritionDataByIdAsync(nutritionalDataIds);
            if (nutritionalDataList == null)
            {
                return NotFound();
            }
            return Ok(nutritionalDataList);
        }


        // PATCH
        [HttpPatch("{nutritionalDataId}")]
        public async Task<IActionResult> UpdateGeneralNutritionalDataAsync(string nutritionalDataId, [FromBody] JsonPatchDocument<NutritionalData> nutDataPatch)
        {
            await _nutritionalDataRepository.UpdateGeneralAttributesAsync(nutritionalDataId, nutDataPatch);
            return NoContent();
        }

        [HttpPatch("multiplePatch")]
        public async Task<IActionResult> UpdateMultipleDocumentsGeneralNutritionalDataAsync(Dictionary<string, JsonPatchDocument<NutritionalData>> nutritionalDataPatches)
        {
            await _nutritionalDataRepository.UpdateMultipleDocumentsGeneralAttributesAsync(nutritionalDataPatches);
            return NoContent();
        }


        // PUT
        [HttpPut]
        public async Task<IActionResult> EditNutritionalData([FromBody] NutritionalData nutritionalData)
        {
            await _nutritionalDataRepository.UpdateNutritionDataAsync(nutritionalData);
            return NoContent();
        }


        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNutritionalData(string id)
        {
            await _nutritionalDataRepository.DeleteNutritionDataByIdAsync(id);
            return NoContent();
        }

    }
}
