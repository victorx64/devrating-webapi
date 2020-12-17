using System.Data;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerContainsWorkOperation : ContainsWorkOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerContainsWorkOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool Contains(string repository, string start, string end)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id 
                FROM Work 
                WHERE Repository = @Repository 
                AND StartCommit = @StartCommit
                AND EndCommit = @EndCommit";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = repository});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50) {Value = start});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = end});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(Id id)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

    }
}