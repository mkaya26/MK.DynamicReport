using MK.DynamicReport.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MK.DynamicReport.Domain.DTOs;
using Newtonsoft.Json;
using MK.DynamicReport.Domain.Interfaces;
using MK.DynamicReport.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace MK.DynamicReport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IReportDefinitionRepository _reportDefinitionRepository;
        public ReportController(IReportService reportService, IReportDefinitionRepository reportDefinitionRepository)
        {
            _reportService = reportService;
            _reportDefinitionRepository = reportDefinitionRepository;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("generatesql")]
        public IActionResult GenerateSql([FromBody] ReportRequestDto request)
        {
            var sql = _reportService.GenerateSql(JsonConvert.SerializeObject(request));
            return Ok(sql);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("executereport")]
        public async Task<IActionResult> ExecuteReport([FromBody] ReportRequestDto request)
        {
            var result = await _reportService.ExecuteReportAsync(JsonConvert.SerializeObject(request));
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("exportexcel")]
        public async Task<IActionResult> ExportExcel([FromBody] ReportRequestDto request)
        {
            var data = await _reportService.ExecuteReportAsync(JsonConvert.SerializeObject(request));
            var fileBytes = _reportService.ExportToExcelWithNPOI(data);

            var fileName = $"Rapor_{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost("exportpdf")]
        public async Task<IActionResult> ExportPdf([FromBody] ReportRequestDto request)
        {
            var data = await _reportService.ExecuteReportAsync(JsonConvert.SerializeObject(request));
            var pdfBytes = _reportService.ExportToPdf(data);

            var fileName = $"Rapor_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf";
            return File(pdfBytes, "application/pdf", fileName);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost("savereport")]
        public async Task<IActionResult> SaveReport([FromBody] ReportDefinition report)
        {
            await _reportDefinitionRepository.AddAsync(report);
            return Ok(new { message = "Rapor başarıyla kaydedildi." });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("getreports")]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _reportDefinitionRepository.GetAllAsync();
            return Ok(reports);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("deletereport/{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            await _reportDefinitionRepository.DeleteAsync(id);
            return Ok(new { message = "Rapor silindi." });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("runreport/{id}")]
        public async Task<IActionResult> RunReport(int id)
        {
            var reportDefinition = await _reportDefinitionRepository.GetByIdAsync(id);
            if (reportDefinition == null)
                return NotFound(new { message = "Rapor bulunamadı." });

            var data = await _reportService.ExecuteReportAsync(reportDefinition.ReportJson);
            return Ok(data);
        }

    }
}
