using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerContainsKeyOperation : ContainsKeyOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerContainsKeyOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool Contains(Id id)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = id.Value()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public bool Contains(Id organization, string value)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM [Key] WHERE OrganizationId = @OrganizationId AND Value = @Value";

            command.Parameters.Add(new SqlParameter("@OrganizationId", SqlDbType.Int) {Value = organization.Value()});
            command.Parameters.Add(new SqlParameter("@Value", SqlDbType.NVarChar) {Value = value});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }
    }
}