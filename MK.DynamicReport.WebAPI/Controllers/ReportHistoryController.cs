using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MK.DynamicReport.Domain.Entities;
using MK.DynamicReport.Domain.Interfaces;

namespace MK.DynamicReport.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportHistoryController : ControllerBase
    {
        private readonly IReportHistoryRepository _reportHistoryRepository;

        public ReportHistoryController(IReportHistoryRepository reportHistoryRepository)
        {
            _reportHistoryRepository = reportHistoryRepository;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var histories = await _reportHistoryRepository.GetAllAsync();
            return Ok(histories);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var history = await _reportHistoryRepository.GetByIdAsync(id);
            if (history == null)
                return NotFound();
            return Ok(history);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ReportHistory reportHistory)
        {
            await _reportHistoryRepository.AddAsync(reportHistory);
            return Ok(new { message = "Rapor geçmişi eklendi." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _reportHistoryRepository.DeleteAsync(id);
            return Ok(new { message = "Rapor geçmişi silindi." });
        }
    }
}
