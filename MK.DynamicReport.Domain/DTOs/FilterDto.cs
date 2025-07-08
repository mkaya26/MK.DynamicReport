namespace MK.DynamicReport.Domain.DTOs
{
    public class FilterDto
    {
        public string Table { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }    // =, >, <, >=, <=, LIKE
        public string Value { get; set; }
    }
}
