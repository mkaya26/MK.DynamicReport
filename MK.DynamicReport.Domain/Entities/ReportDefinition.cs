namespace MK.DynamicReport.Domain.Entities
{
    public class ReportDefinition
    {
        public int Id { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ReportJson { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
