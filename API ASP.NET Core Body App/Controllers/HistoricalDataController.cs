using API_ASP.NET_Core_Body_App.Models.HistoricData;
using API_ASP.NET_Core_Body_App.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API_ASP.NET_Core_Body_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricalDataController : ControllerBase
    {
        private readonly IHistoricalDataRepository _historicalDataRepository;

        public HistoricalDataController(IHistoricalDataRepository historicalDataRepository)
        {
            _historicalDataRepository = historicalDataRepository;
        }



        // GET
        [HttpGet("{historicalDataId}")]
        public async Task<IActionResult> GetHistoricalData(string historicalDataId)
        {
            var historicalData = await _historicalDataRepository.GetHistoricalDataByIdAsync(historicalDataId);
            if (historicalData == null)
            {
                return NotFound();
            }

            return Ok(historicalData);
        }



        // POST
        [HttpPost]
        public async Task<IActionResult> CreateHistoricalData([FromBody] HistoricalData historicalDataToAdd)
        {
            await _historicalDataRepository.AddHistoricalDataAsync(historicalDataToAdd);
            
            return CreatedAtAction(
                nameof(GetHistoricalData),
                new { historicalDataId = historicalDataToAdd.Id },
                historicalDataToAdd
            );
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllListHistoricalData([FromBody] List<string> historicalDataIds)
        {
            var historicalDataList = await _historicalDataRepository.GetAllListHistoricalDataByIdAsync(historicalDataIds);
            if (historicalDataList == null)
            {
                return NotFound();
            }
            return Ok(historicalDataList);
        }

        [HttpPost("manyDeletes")]
        public async Task<IActionResult> DeleteMultipleHistoricalDataByIdListAsync([FromBody] List<string> idList)
        {
            await _historicalDataRepository.DeleteMultipleHistoricalDataByIdListAsync(idList);
            return NoContent();
        }



        // PATCH
        [HttpPatch("{histDataId}")]
        public async Task<IActionResult> UpdateSingleDocAttribute(string histDataId, [FromBody] JsonPatchDocument<HistoricalData> histDataPatch)
        {
            await _historicalDataRepository.UpdateSingleDocAttributesAsync(histDataId, histDataPatch);
            return NoContent();
        }

        [HttpPatch("multiplePatch")]
        public async Task<IActionResult> UpdateMultipleDocAttributes([FromBody] Dictionary<string, JsonPatchDocument<HistoricalData>> histDataPatches)
        {
            await _historicalDataRepository.UpdateMultipleDocsAttributesAsync(histDataPatches);
            return NoContent();
        }



        // DELETE
        [HttpDelete("{historicalDataId}")]
        public async Task<IActionResult> DeleteHistoricalData(string historicalDataId)
        {
            await _historicalDataRepository.DeleteHistoricalDataByIdAsync(historicalDataId);
            return NoContent();
        }
    }
}