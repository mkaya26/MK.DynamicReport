namespace MK.DynamicReport.Application.Interfaces
{
    public interface IReportService
    {
        string GenerateSql(string reportJson);
        Task<IEnumerable<Dictionary<string, object>>> ExecuteReportAsync(string reportJson);
        byte[] ExportToExcelWithNPOI(IEnumerable<Dictionary<string, object>> data);
        byte[] ExportToPdf(IEnumerable<Dictionary<string, object>> data);
    }
}
