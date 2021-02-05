using System;
using System.Data;
using System.Text.Json;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerRating : Rating
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerRating(IDbConnection connection, Id id)
        {
            _connection = connection;
            _id = id;
        }

        public Id Id()
        {
            return _id;
        }

        private sealed class Dto
        {
            public object Id { get; set; } = new object();
            public double Value { get; set; }
            public object? PreviousRatingId { get; set; }
            public double? PreviousRating { get; set; }
            public object WorkId { get; set; } = new object();
            public object AuthorId { get; set; } = new object();
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
            private string _email = string.Empty;

            public string AuthorEmail
            {
                get
                {
                    var at = _email.IndexOf("@", StringComparison.Ordinal);

                    return _email.Substring(0, at);
                }
                set
                {
                    _email = value;
                }
            }

            public uint? CountedDeletions { get; set; }
            public uint? IgnoredDeletions { get; set; }
        }

        public string ToJson()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT 
                    R1.Id, 
                    R1.Rating, 
                    R1.PreviousRatingId, 
                    R2.Rating PreviousRating,
                    R1.WorkId, 
                    R1.AuthorId,
                    R1.CountedDeletions,
                    R1.IgnoredDeletions,
                    A.Email,
                    R1.CreatedAt
                FROM Rating R1
                LEFT JOIN Rating R2 ON R1.PreviousRatingId = R2.Id
                INNER JOIN Author A on R1.AuthorId = A.Id
                WHERE R1.Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return JsonSerializer.Serialize<Dto>(
                new Dto
                {
                    Id = reader["Id"],
                    Value = (float) reader["Rating"],
                    PreviousRatingId = reader["PreviousRatingId"] != DBNull.Value ? reader["PreviousRatingId"] : null,
                    PreviousRating = reader["PreviousRating"] != DBNull.Value
                        ? (float) reader["PreviousRating"]
                        : (double?) null,
                    WorkId = reader["WorkId"],
                    AuthorId = reader["AuthorId"],
                    CountedDeletions = reader["CountedDeletions"] != DBNull.Value
                        ? (uint?) (int) reader["CountedDeletions"]
                        : null,
                    IgnoredDeletions = reader["IgnoredDeletions"] != DBNull.Value
                        ? (uint?) (int) reader["IgnoredDeletions"]
                        : null,
                    AuthorEmail = (string) reader["Email"],
                    CreatedAt = (DateTimeOffset) reader["CreatedAt"]
                }
            );
        }

        public Rating PreviousRating()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT PreviousRatingId FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerRating(_connection, new DefaultId(reader["PreviousRatingId"]));
        }

        public uint? CountedDeletions()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CountedDeletions FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            var result = command.ExecuteScalar();

            return result is DBNull ? null : (uint?) (int?) result;
        }

        public uint? IgnoredDeletions()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT IgnoredDeletions FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            var result = command.ExecuteScalar();

            return result is DBNull ? null : (uint?) (int?) result;
        }

        public Work Work()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT WorkId FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerWork(_connection, new DefaultId(reader["WorkId"]));
        }

        public Author Author()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT AuthorId FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerAuthor(_connection, new DefaultId(reader["AuthorId"]));
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            return (DateTimeOffset) command.ExecuteScalar()!;
        }

        public double Value()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Rating FROM Rating WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (float) reader["Rating"];
        }
    }
}