using System.Collections.Generic;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
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

            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) { Value = name });

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerOrganization(_connection, new DefaultId(reader["Id"]));
        }

        public IEnumerable<Organization> OrganizationsByUser(string user)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Organization WHERE UserId = @UserId";

            command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { Value = user });

            using var reader = command.ExecuteReader();

            var organizations = new List<Organization>();

            while (reader.Read())
            {
                organizations.Add(new SqlServerOrganization(_connection, new DefaultId(reader["Id"])));
            }

            return organizations;
        }
    }
}