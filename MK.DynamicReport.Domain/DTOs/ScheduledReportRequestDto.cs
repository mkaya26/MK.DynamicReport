namespace MK.DynamicReport.Domain.DTOs
{
    public class ScheduledReportRequestDto
    {
        public string ReportJson { get; set; }           
        public DateTime ScheduledTime { get; set; }      
        public string ExportType { get; set; }           
        public string EmailTo { get; set; }            
        public string FileName { get; set; }
    }
}
