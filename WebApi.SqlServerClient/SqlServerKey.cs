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
                    Id,
                    CreatedAt,
                    RevokedAt,
                    Organization,
                    Name
                FROM [Key]
                WHERE k.Id = @Id
            ";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return JsonSerializer.Serialize<Dto>(
                new Dto
                {
                    Id = reader["Id"],
                    Organization = (string)reader["Organization"],
                    Name = reader["Name"] == DBNull.Value ? null : (string)reader["Name"],
                    CreatedAt = (DateTimeOffset)reader["CreatedAt"],
                    RevokedAt = reader["RevokedAt"] == DBNull.Value ? null : (DateTimeOffset?)reader["RevokedAt"]
                }
            );
        }

        public string? Name()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Name FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return reader["Name"] == DBNull.Value ? null : (string)reader["Name"];
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

        public string Organization()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Organization FROM [Key] WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string)reader["Organization"];
        }

        private sealed class Dto
        {
            public object Id { get; set; } = new object();
            public string? Name { get; set; } = default;
            public string Organization { get; set; } = string.Empty;
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
            public DateTimeOffset? RevokedAt { get; set; } = default;
        }
    }
}