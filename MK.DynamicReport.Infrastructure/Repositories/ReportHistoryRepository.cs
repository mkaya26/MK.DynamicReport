using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MK.DynamicReport.Domain.Entities;
using MK.DynamicReport.Domain.Interfaces;

namespace MK.DynamicReport.Infrastructure.Repositories
{
    public class ReportHistoryRepository : IReportHistoryRepository
    {
        private readonly string _connectionString;

        public ReportHistoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task AddAsync(ReportHistory reportHistory)
        {
            const string query = @"
                INSERT INTO ReportHistories 
                (ReportDefinitionId, ReportName, ExportFormat, FilePath, CreatedDate, CreatedBy)
                VALUES (@ReportDefinitionId, @ReportName, @ExportFormat, @FilePath, @CreatedDate, @CreatedBy)";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, reportHistory);
        }

        public async Task<IEnumerable<ReportHistory>> GetAllAsync()
        {
            const string query = "SELECT * FROM ReportHistories";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ReportHistory>(query);
        }

        public async Task<ReportHistory> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM ReportHistories WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ReportHistory>(query, new { Id = id });
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM ReportHistories WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
