using System;
using System.Data;
using System.Text.Json;
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

            command.CommandText = "SELECT CreatedAt FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset)reader["CreatedAt"];
        }

        public Id Id()
        {
            return _id;
        }

        public Organization Organization()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT OrganizationId FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerOrganization(_connection, new DefaultObject.DefaultId(reader["OrganizationId"]));
        }

        public DateTimeOffset? RevokedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT RevokedAt FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return reader["RevokedAt"] == DBNull.Value
                ? null
                : (DateTimeOffset?)reader["RevokedAt"];
        }

        public string ToJson()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    k.Id, 
                    k.CreatedAt, 
                    k.RevokedAt, 
                    k.OrganizationId,
                    o.Name
                FROM [Key]
                INNER JOIN Organization o on k.OrganizationId = o.Id
                WHERE Id = @Id
            ";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return JsonSerializer.Serialize<Dto>(
                new Dto
                {
                    Id = reader["Id"],
                    Organization = (string) reader["Name"],
                    OrganizationId = reader["OrganizationId"],
                    CreatedAt = (DateTimeOffset) reader["CreatedAt"],
                    RevokedAt = reader["RevokedAt"] == DBNull.Value ? null : (DateTimeOffset?) reader["RevokedAt"]
                }
            );
        }

        public string Value()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Value FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string)reader["Value"];
        }

        private sealed class Dto
        {
            public object Id { get; set; } = new object();
            public string Organization { get; set; } = string.Empty;
            public object OrganizationId { get; set; } = new object();
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
            public DateTimeOffset? RevokedAt { get; set; } = default;
        }
    }
}