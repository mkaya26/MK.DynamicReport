namespace MK.DynamicReport.Application.Interfaces
{
    public interface IGraphicsService
    {
        byte[] GenerateSampleChart();
        (string[] Labels, double[] Values) PrepareChartData(IEnumerable<Dictionary<string, object>> data, string labelField, string valueField);
        byte[] GenerateChart(string[] labels, double[] values);
    }

}
