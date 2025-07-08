using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MK.DynamicReport.Domain.Entities;
using MK.DynamicReport.Domain.Interfaces;

namespace MK.DynamicReport.Infrastructure.Repositories
{
    public class ReportDefinitionRepository : IReportDefinitionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ReportDefinitionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task AddAsync(ReportDefinition report)
        {
            const string sql = @"INSERT INTO ReportDefinitions (Id, ReportName, Description, ReportJson, CreatedDate)
                                 VALUES (@Id, @ReportName, @Description, @ReportJson, @CreatedDate)";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, report);
        }

        public async Task<IEnumerable<ReportDefinition>> GetAllAsync()
        {
            const string sql = @"SELECT * FROM ReportDefinitions ORDER BY CreatedDate DESC";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ReportDefinition>(sql);
        }

        public async Task<ReportDefinition> GetByIdAsync(int id)
        {
            const string sql = @"SELECT * FROM ReportDefinitions WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ReportDefinition>(sql, new { Id = id });
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = @"DELETE FROM ReportDefinitions WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { Id = id });
        }

    }
}
