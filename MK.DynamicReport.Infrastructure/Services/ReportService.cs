using MK.DynamicReport.Application.Interfaces;
using MK.DynamicReport.Domain.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace MK.DynamicReport.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IConfiguration _configuration;

        public ReportService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateSql(string reportJson)
        {
            var request = JsonConvert.DeserializeObject<ReportRequestDto>(reportJson);

            if (request == null || !request.Tables.Any() || !request.Fields.Any())
                return "Invalid report definition";

            var selectClause = string.Join(", ", request.Fields.Select(f => $"{f.Table}.{f.Field}"));
            var fromClause = request.Tables.First();

            var joinClause = "";
            if (request.Joins.Any())
            {
                joinClause = string.Join(" ", request.Joins.Select(j =>
                    $"{j.JoinType.ToUpper()} JOIN {j.RightTable} ON {j.LeftTable}.{j.LeftField} = {j.RightTable}.{j.RightField}"
                ));
            }

            var whereClause = "";
            if (request.Filters.Any())
            {
                whereClause = " WHERE " + string.Join(" AND ", request.Filters.Select(f =>
                    $"{f.Table}.{f.Field} {f.Operator} '{f.Value}'"
                ));
            }

            var sql = $"SELECT {selectClause} FROM {fromClause} {joinClause}{whereClause}";
            return sql;
        }

        public async Task<IEnumerable<Dictionary<string, object>>> ExecuteReportAsync(string reportJson)
        {
            var sql = GenerateSql(reportJson);
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var result = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var rows = await connection.QueryAsync(sql);

                foreach (var row in rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in row)
                    {
                        dict[prop.Key] = prop.Value;
                    }
                    result.Add(dict);
                }
            }

            return result;
        }

        public byte[] ExportToExcelWithNPOI(IEnumerable<Dictionary<string, object>> data)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Report");

            int rowIndex = 0;

            if (data.Any())
            {
                IRow headerRow = sheet.CreateRow(rowIndex++);
                var headers = data.First().Keys.ToList();
                for (int i = 0; i < headers.Count; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(headers[i]);
                }

                foreach (var record in data)
                {
                    IRow row = sheet.CreateRow(rowIndex++);
                    int cellIndex = 0;
                    foreach (var value in record.Values)
                    {
                        //row.CreateCell(cellIndex++).SetCellValue(value?.ToString() ?? "");
                        var formattedValue = FormatValue(value);
                        row.CreateCell(cellIndex++).SetCellValue(formattedValue);

                    }
                }

                for (int i = 0; i < headers.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            }

            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);
                return stream.ToArray();
            }
        }

        public byte[] ExportToPdf(IEnumerable<Dictionary<string, object>> data)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().PaddingBottom(10).Text("Dinamik Rapor").FontSize(16).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        if (!data.Any())
                        {
                            table.ColumnsDefinition(columns => columns.RelativeColumn());
                            table.Cell().Element(CellStyle).Text("Veri yok");
                            return;
                        }

                        var headers = data.First().Keys.ToList();
                        int columnCount = headers.Count;

                        // 👉 Kolon Sayısını Tanımla
                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < columnCount; i++)
                                columns.RelativeColumn();
                        });

                        // Başlıklar
                        //foreach (var header in headers)
                        //{
                        //    table.Cell().Element(CellStyle).Text(header).FontSize(12).SemiBold();
                        //}
                        foreach (var header in headers)
                        {
                            table.Cell().Element(container =>
                            {
                                return container
                                    .Border(1)
                                    .Background(Colors.Grey.Lighten2)  // Buraya
                                    .Padding(5)
                                    .AlignLeft()
                                    .AlignMiddle();
                            })
                            .Text(header)
                            .FontSize(12)
                            .Bold();
                        }

                        // Veriler
                        foreach (var record in data)
                        {
                            foreach (var value in record.Values)
                            {
                                //table.Cell().Element(CellStyle).Text(value?.ToString() ?? "");
                                var formattedValue = FormatValue(value);
                                table.Cell().Element(CellStyle).Text(formattedValue);

                            }
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Border(1)
                                .Padding(5)
                                .AlignLeft()
                                .AlignMiddle();
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static string FormatValue(object value)
        {
            if (value is DateTime dt)
                return dt.ToString("dd.MM.yyyy");

            if (value is decimal dec)
                return dec.ToString("N2");  // 1.234,56

            if (value is double dbl)
                return dbl.ToString("N2");

            if (value is float flt)
                return flt.ToString("N2");

            if (value is int i)
                return i.ToString("N0");  // Tam sayılar → binlik ayraçlı

            if (value is long l)
                return l.ToString("N0");

            return value?.ToString() ?? "";
        }


    }
}
