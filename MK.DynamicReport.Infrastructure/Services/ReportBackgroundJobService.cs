using FluentEmail.Core;
using Hangfire;
using Microsoft.Extensions.Options;
using MK.DynamicReport.Application.Interfaces;
using MK.DynamicReport.Domain.Interfaces;
using MK.DynamicReport.Domain.Settings;

namespace MK.DynamicReport.Infrastructure.Services
{
    public class ReportBackgroundJobService
    {
        private readonly IReportDefinitionRepository _reportDefinitionRepository;
        private readonly IReportService _reportService;
        private readonly IFluentEmail _fluentEmail;
        private readonly SmtpSettings _smtpSettings;

        public ReportBackgroundJobService(
            IReportDefinitionRepository reportDefinitionRepository,
            IReportService reportService,
            IFluentEmail fluentEmail,
            IOptions<SmtpSettings> smtpOptions)
        {
            _reportDefinitionRepository = reportDefinitionRepository;
            _reportService = reportService;
            _fluentEmail = fluentEmail;
            _smtpSettings = smtpOptions.Value;
        }
        [AutomaticRetry(Attempts = 0)]
        public async Task ExportReportAndSendEmailAsync(int reportId, string emailTo)
        {
            var reportDefinition = await _reportDefinitionRepository.GetByIdAsync(reportId);
            if (reportDefinition == null)
            {
                Console.WriteLine($"[Hangfire] Rapor bulunamadı. ReportId: {reportId}");
                return;
            }

            var data = await _reportService.ExecuteReportAsync(reportDefinition.ReportJson);
            var pdfBytes = _reportService.ExportToPdf(data);

            var fileName = $"Rapor_{reportId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            var response = await _fluentEmail
                .To(emailTo)
                .Subject($"Otomatik Rapor: {reportDefinition.ReportName}")
                .Body($"Merhaba,<br><br>İstediğiniz rapor ekte sunulmuştur.<br><br>Rapor: {reportDefinition.ReportName}", true)
                .Attach(new FluentEmail.Core.Models.Attachment
                {
                    Data = new MemoryStream(pdfBytes),
                    Filename = fileName,
                    ContentType = "application/pdf"
                })
                .SendAsync();

            Console.WriteLine(response.Successful
                ? $"[Hangfire] E-Mail gönderildi: {emailTo}"
                : $"[Hangfire] E-Mail gönderilemedi: {response.ErrorMessages.FirstOrDefault()}");
        }
    }
}
