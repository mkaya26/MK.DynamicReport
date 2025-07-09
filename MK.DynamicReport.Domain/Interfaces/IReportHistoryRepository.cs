using MK.DynamicReport.Domain.Entities;

namespace MK.DynamicReport.Domain.Interfaces
{
    public interface IReportHistoryRepository
    {
        Task AddAsync(ReportHistory reportHistory);
        Task<IEnumerable<ReportHistory>> GetAllAsync();
        Task<ReportHistory> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}
