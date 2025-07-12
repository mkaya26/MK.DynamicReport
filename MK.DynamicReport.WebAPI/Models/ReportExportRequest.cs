namespace MK.DynamicReport.WebAPI.Models
{
    public class ReportExportRequest
    {
        public string ReportJson { get; set; }
        public bool IncludeChart { get; set; }
    }
}
