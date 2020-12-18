using System;
using System.Data;
using System.Text.Json;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerOrganization : Organization
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerOrganization(IDbConnection connection, Id id)
        {
            _connection = connection;
            _id = id;
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset) reader["CreatedAt"];
        }

        public Id Id()
        {
            return _id;
        }

        public string Name()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Name FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string) reader["Name"];
        }

        public string ToJson()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    Id, 
                    CreatedAt, 
                    Name
                FROM Organization
                WHERE Id = @Id
            ";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return JsonSerializer.Serialize<Dto>(
                new Dto
                {
                    Id = reader["Id"],
                    Name = (string) reader["Name"],
                    CreatedAt = (DateTimeOffset) reader["CreatedAt"]
                }
            );
        }

        private sealed class Dto
        {
            public object Id { get; set; } = new object();
            public string Name { get; set; } = string.Empty;
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
        }

        public string User()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT UserId FROM Organization WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string) reader["UserId"];
        }
    }
}