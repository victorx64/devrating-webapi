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

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(string name)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Name = @Name";

            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) {Value = name});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(Id id, Id userId)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Id = @Id AND UserId = @UserId";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});
            command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) {Value = userId.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}