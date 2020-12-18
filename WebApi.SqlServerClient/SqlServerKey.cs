using System;
using System.Data;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerKey : Key
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerKey(IDbConnection connection, Id id)
        {
            _connection = connection;
            _id = id;
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM Key WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset) reader["CreatedAt"];
        }

        public Id Id()
        {
            return _id;
        }

        public Organization Organization()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT OrganizationId FROM Key WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerOrganization(_connection, new DefaultObject.DefaultId(reader["OrganizationId"]));
        }

        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public string Value()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Value FROM Key WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string) reader["Value"];
        }
    }
}