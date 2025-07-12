using MK.DynamicReport.Application.Interfaces;
using NPOI.SS.Formula.Functions;
using ScottPlot;
using ScottPlot.Plottables;
namespace MK.DynamicReport.Infrastructure.Services
{
    public class GraphicsService : IGraphicsService
    {
        public byte[] GenerateSampleChart()
        {
            var plt = new ScottPlot.Plot();

            double[] xs = { 1, 2, 3, 4, 5 };
            double[] ys = { 10, 20, 15, 30, 25 };

            plt.Add.Scatter(xs, ys);
            plt.Axes.Title.Label.Text = "Örnek Satış Grafiği";
            plt.Axes.Bottom.Label.Text = "Tarih";
            plt.Axes.Left.Label.Text = "Satış";

            return plt.GetImageBytes(600, 400, ImageFormat.Png);
        }

        public (string[] Labels, double[] Values) PrepareChartData(IEnumerable<Dictionary<string, object>> data, string labelField, string valueField)
        {
            var labels = data.Select(d => d.ContainsKey(labelField) ? d[labelField]?.ToString() ?? "" : "").ToArray();
            var values = data.Select(d =>
            {
                if (d.ContainsKey(valueField) && double.TryParse(d[valueField]?.ToString(), out double val))
                    return val;
                return 0.0;
            }).ToArray();

            return (labels, values);
        }
        public byte[] GenerateChart(string[] labels, double[] values)
        {
            ScottPlot.Plot myPlot = new();
            var barPlot = myPlot.Add.Bars(values);

            // define the content of labels
            int i = 0;
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = labels[i];
                i++;
            }

            // customize label style
            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;

            myPlot.Axes.Margins(bottom: 0, top: .2);


            return myPlot.GetImageBytes(600, 400, ImageFormat.Png);

        }
    }
}
