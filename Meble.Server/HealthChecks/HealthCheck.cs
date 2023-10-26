using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace Meble.Server.HealthChecks
{
    public class HealthCheck : IHealthCheck
    {
        private const string DefaultTestQuery = "SELECT 1";
        private string _connectionString { get; }
        private string _testQuery { get; }

        public HealthCheck(string connectionString) : this(connectionString, testQuery: DefaultTestQuery) { }

        public HealthCheck(string connectionString, string testQuery = DefaultTestQuery)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _testQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    if (_testQuery != null)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = _testQuery;
                        command.ExecuteScalar();

                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }

                    return HealthCheckResult.Healthy("Database is healthy");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Database is offline or damaged", ex);
                }
            }
        }
    }
}
