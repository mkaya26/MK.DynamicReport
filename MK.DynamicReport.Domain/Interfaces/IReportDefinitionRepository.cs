using MK.DynamicReport.Domain.Entities;

namespace MK.DynamicReport.Domain.Interfaces
{
    public interface IReportDefinitionRepository
    {
        Task<IEnumerable<ReportDefinition>> GetAllAsync();
        Task<ReportDefinition> GetByIdAsync(int id);
        Task AddAsync(ReportDefinition report);
        Task DeleteAsync(int id);
    }
}
