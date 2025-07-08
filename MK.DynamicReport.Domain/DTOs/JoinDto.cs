namespace MK.DynamicReport.Domain.DTOs
{
    public class JoinDto
    {
        public string LeftTable { get; set; }
        public string LeftField { get; set; }
        public string RightTable { get; set; }
        public string RightField { get; set; }
        public string JoinType { get; set; } = "INNER";  // INNER, LEFT, RIGHT gibi
    }
}
