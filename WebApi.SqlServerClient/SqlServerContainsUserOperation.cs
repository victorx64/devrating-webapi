using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerContainsUserOperation : ContainsUserOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerContainsUserOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool Contains(string foreignId)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM User WHERE ForeignId = @ForeignId";

            command.Parameters.Add(new SqlParameter("@ForeignId", SqlDbType.NVarChar) {Value = foreignId});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(Id id)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM User WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}