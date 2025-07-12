using MK.DynamicReport.Domain.DTOs;

namespace MK.DynamicReport.Application.Interfaces
{
    public interface IReportService
    {
        string GenerateSql(string reportJson);
        Task<IEnumerable<Dictionary<string, object>>> ExecuteReportAsync(string reportJson);
        byte[] ExportToExcelWithNPOI(IEnumerable<Dictionary<string, object>> data);
        byte[] ExportToPdf(IEnumerable<Dictionary<string, object>> data);
        byte[] ExportToPdfWithChart(IEnumerable<Dictionary<string, object>> data);
        Task ScheduleReportAsync(ScheduledReportRequestDto request);
        List<ScheduledJobDto> GetScheduledJobs();
        bool DeleteScheduledJob(string jobId);

    }
}
