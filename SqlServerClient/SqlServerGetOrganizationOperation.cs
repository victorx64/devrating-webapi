using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal class SqlServerGetOrganizationOperation : GetOrganizationOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetOrganizationOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Organization Organization(Id id)
        {
            return new SqlServerOrganization(_connection, id);
        }

        public Organization Organization(string name)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE Name = @Name";

            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) {Value = name});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerOrganization(_connection, new DefaultId(reader["Id"]));
        }
    }
}