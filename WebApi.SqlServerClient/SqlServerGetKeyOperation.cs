using System.Collections.Generic;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerGetKeyOperation : GetKeyOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetKeyOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Key Key(Id id)
        {
            return new SqlServerKey(_connection, id);
        }

        public IEnumerable<Key> OrganizationKeys(string organization)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT k.Id
                FROM [Key] k
                INNER JOIN Organization o ON o.Id = k.OrganizationId
                WHERE o.Name = @Organization";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) { Value = organization });

            using var reader = command.ExecuteReader();

            var keys = new List<Key>();

            while (reader.Read())
            {
                keys.Add(new SqlServerKey(_connection, new DefaultId(reader["Id"])));
            }

            return keys;
        }

        public IEnumerable<Key> OrganizationKeys(Id organization)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT k.Id
                FROM [Key] k
                WHERE k.OrganizationId = @OrganizationId";

            command.Parameters.Add(new SqlParameter("@OrganizationId", SqlDbType.Int) { Value = organization.Value() });

            using var reader = command.ExecuteReader();

            var keys = new List<Key>();

            while (reader.Read())
            {
                keys.Add(new SqlServerKey(_connection, new DefaultId(reader["Id"])));
            }

            return keys;
        }
    }
}