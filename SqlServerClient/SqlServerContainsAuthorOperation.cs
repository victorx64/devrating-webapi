using System.Data;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerContainsAuthorOperation : ContainsAuthorOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerContainsAuthorOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool Contains(string organization, string repository, string email)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id
                FROM Author
                WHERE Email = @Email
                AND Organization = @Organization
                AND Repository = @Repository";

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) {Value = email});
            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) {Value = organization});
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) {Value = repository});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(Id id)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Author WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}