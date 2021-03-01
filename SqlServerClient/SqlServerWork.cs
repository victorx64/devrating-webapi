using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerWork : Work
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerWork(IDbConnection connection, Id id)
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
            public string Repository { get; set; } = string.Empty;
            public string? Link { get; set; }
            public string StartCommit { get; set; } = string.Empty;
            public string EndCommit { get; set; } = string.Empty;
            public string? SinceCommit { get; set; }
            public object AuthorId { get; set; } = new object();
            public object? UsedRatingId { get; set; }
            public double? UsedRating { get; set; }
            public object? NewRatingId { get; set; }
            public double? NewRating { get; set; }
            public uint Additions { get; set; }
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
            public string AuthorEmail{ get; set; } = string.Empty;
        }

        public string ToJson()
        {
            var ratings = new EntitiesCollection(Ratings()).ToJson();

            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT 
                    Work.Id,
                    Link,
                    StartCommit,
                    EndCommit,
                    SinceCommit,
                    Work.AuthorId,
                    UsedRatingId,
                    R2.Rating,
                    Additions,
                    A.Email,
                    R3.Id NewRatingId,
                    R3.Rating NewRating,
                    Work.CreatedAt
                FROM Work
                LEFT JOIN Rating R2 on Work.UsedRatingId = R2.Id
                INNER JOIN Author A on Work.AuthorId = A.Id
                LEFT JOIN Rating R3 on R3.WorkId = Work.Id AND R3.AuthorId = Work.AuthorId
                WHERE Work.Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return JsonSerializer.Serialize<Dto>(
                new Dto
                {
                    Id = reader["Id"],
                    Link = reader["Link"] != DBNull.Value ? (string) reader["Link"] : null,
                    StartCommit = (string) reader["StartCommit"],
                    EndCommit = (string) reader["EndCommit"],
                    SinceCommit = reader["SinceCommit"] != DBNull.Value ? (string) reader["SinceCommit"] : null,
                    AuthorId = reader["AuthorId"],
                    UsedRatingId = reader["UsedRatingId"] != DBNull.Value ? reader["UsedRatingId"] : null,
                    UsedRating = reader["Rating"] != DBNull.Value ? (float) reader["Rating"] : (double?) null,
                    Additions = (uint) (int) reader["Additions"],
                    AuthorEmail = (string) reader["Email"],
                    NewRatingId = reader["NewRatingId"] != DBNull.Value ? reader["NewRatingId"] : null,
                    NewRating = reader["NewRating"] != DBNull.Value ? (float) reader["NewRating"] : (double?) null,
                    CreatedAt = (DateTimeOffset) reader["CreatedAt"],
                }
            ).Insert(1, "\"Ratings\":" + ratings + ",");
        }

        public uint Additions()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Additions FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return (uint) (int) reader["Additions"];
        }

        public string? Since()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT SinceCommit FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            return command.ExecuteScalar() as string;
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});
            
            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset) reader["CreatedAt"];
        }

        public Author Author()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT AuthorId FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerAuthor(_connection, new DefaultId(reader["AuthorId"]));
        }

        public Rating UsedRating()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT UsedRatingId FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerRating(_connection, new DefaultId(reader["UsedRatingId"]));
        }

        public string Start()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT StartCommit FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            return (string) command.ExecuteScalar()!;
        }

        public string End()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT EndCommit FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            return (string) command.ExecuteScalar()!;
        }

        public string? Link()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Link FROM Work WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) {Value = _id.Value()});

            return command.ExecuteScalar() as string;
        }

        private IEnumerable<Rating> Ratings()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Rating WHERE WorkId = @WorkId";

            command.Parameters.Add(new SqlParameter("@WorkId", SqlDbType.Int) {Value = _id.Value()});

            using var reader = command.ExecuteReader();

            var ratings = new List<Rating>();

            while (reader.Read())
            {
                ratings.Add(new SqlServerRating(_connection, new DefaultId(reader["Id"])));
            }

            return ratings;
        }
    }
}