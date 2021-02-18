using System;
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

        public bool Contains(string organization, string repository, string start, string end)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT w.Id 
                FROM Work w
                INNER JOIN Author a on a.Id = w.AuthorId
                WHERE a.Organization = @Organization
                AND a.Repository = @Repository 
                AND w.StartCommit = @StartCommit
                AND w.EndCommit = @EndCommit";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) {Value = organization});
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) {Value = repository});
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

        public bool Contains(string organization, string repository, DateTimeOffset after)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT w.Id
                FROM Work w
                INNER JOIN Author a on a.Id = w.AuthorId
                WHERE a.Organization = @Organization
                AND a.Repository = @Repository
                AND w.CreatedAt >= @After";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) {Value = organization});
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) {Value = repository});
            command.Parameters.Add(new SqlParameter("@After", SqlDbType.DateTimeOffset) { Value = after });

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}