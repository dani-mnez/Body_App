using API_ASP.NET_Core_Body_App.Models;
using API_ASP.NET_Core_Body_App.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API_ASP.NET_Core_Body_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhysicalDataController : ControllerBase
    {
        private readonly IPhysicalDataRepository _physicalData;

        public PhysicalDataController(IPhysicalDataRepository physicalData)
        {
            _physicalData = physicalData;
        }

        // GET
        [HttpGet("{physicalDataId}")]
        public async Task<IActionResult> GetPhysicalData(string physicalDataId)
        {
            var physicalData = await _physicalData.GetPhysicalDataByIdAsync(physicalDataId);
            if (physicalData == null)
            {
                return NotFound();
            }

            return Ok(physicalData);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreatePhysicalData([FromBody] PhysicalData physicalDataToAdd)
        {
            await _physicalData.AddPhysicalDataAsync(physicalDataToAdd);
            return CreatedAtAction(
                nameof(GetPhysicalData),
                new { physicalDataId = physicalDataToAdd.Id },
                physicalDataToAdd
            );
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllPhysicalData([FromBody] List<string> physicalDataIds)
        {
            var physicalDataList = await _physicalData.GetAllPhysicalDataAsync(physicalDataIds);
            if (physicalDataList == null)
            {
                return NotFound();
            }
            return Ok(physicalDataList);
        }

        // PATCH
        [HttpPatch("{physicalDataId}")]
        public async Task<IActionResult> EditPhysicalData(string physicalDataId, [FromBody] JsonPatchDocument<PhysicalData> physDataPatch)
        {
            await _physicalData.UpdatePhysicalDataByIdAsync(physicalDataId, physDataPatch);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{physicalDataId}")]
        public async Task<IActionResult> DeletePhysicalData(string physicalDataId)
        {
            await _physicalData.DeletePhysicalDataByIdAsync(physicalDataId);
            return NoContent();
        }
    }
}
