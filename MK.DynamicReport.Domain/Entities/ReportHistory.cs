namespace MK.DynamicReport.Domain.Entities
{
    public class ReportHistory
    {
        public int Id { get; set; }
        public int ReportDefinitionId { get; set; }
        public string ReportName { get; set; }
        public string ExportFormat { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
