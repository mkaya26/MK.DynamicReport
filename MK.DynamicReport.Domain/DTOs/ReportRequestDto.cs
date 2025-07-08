namespace MK.DynamicReport.Domain.DTOs
{
    public class ReportRequestDto
    {
        public List<string> Tables { get; set; } = new();
        public List<JoinDto> Joins { get; set; } = new();
        public List<FieldDto> Fields { get; set; } = new();
        public List<FilterDto> Filters { get; set; } = new();
    }
}
