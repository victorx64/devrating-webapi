using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerContainsOrganizationOperation : ContainsOrganizationOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerContainsOrganizationOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool Contains(Id id)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id.Value() });

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(string name)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Name = @Name";

            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) { Value = name });

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(Id id, string subject)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Id = @Id AND AuthorizedSubject = @AuthorizedSubject";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id.Value() });
            command.Parameters.Add(new SqlParameter("@AuthorizedSubject", SqlDbType.NVarChar) { Value = subject });

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(string name, string subject)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Name = @Name AND AuthorizedSubject = @AuthorizedSubject";

            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) { Value = name });
            command.Parameters.Add(new SqlParameter("@AuthorizedSubject", SqlDbType.NVarChar) { Value = subject });

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}