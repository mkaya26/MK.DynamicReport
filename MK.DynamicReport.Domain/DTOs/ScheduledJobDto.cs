namespace MK.DynamicReport.Domain.DTOs
{
    public class ScheduledJobDto
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public string Method { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ScheduledAt { get; set; }
    }
}
