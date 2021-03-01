using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerAuthor : Author
    {
        private readonly IDbConnection _connection;
        private readonly Id _id;

        public SqlServerAuthor(IDbConnection connection, Id id)
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
            public string Email{ get; set; } = string.Empty;
            public object? RatingId { get; set; }
            public double? Rating { get; set; }
            public string Organization { get; set; } = string.Empty;
            public string Repository { get; set; } = string.Empty;
            public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
        }

        public string ToJson()
        {
            var ratings = Last90DaysRatings().ToList();

            var dto = AuthorDataObject();

            dto.Rating = ratings.LastOrDefault()?.Value();

            dto.RatingId = ratings.LastOrDefault()?.Id().Value();

            return JsonSerializer.Serialize<Dto>(dto)
                .Insert(1, "\"Ratings\":" + new EntitiesCollection(ratings).ToJson() + ",");
        }

        private Dto AuthorDataObject()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id, Email, Organization, Repository, CreatedAt FROM Author WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return new Dto
            {
                Id = reader["Id"],
                Email = (string)reader["Email"],
                Organization = (string)reader["Organization"],
                Repository = (string)reader["Repository"],
                CreatedAt = (DateTimeOffset)reader["CreatedAt"]
            };
        }

        public string Email()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Email FROM Author WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string)reader["Email"];
        }

        public string Organization()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Organization FROM Author WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string)reader["Organization"];
        }

        public DateTimeOffset CreatedAt()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT CreatedAt FROM Author WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (DateTimeOffset)reader["CreatedAt"];
        }

        private IEnumerable<Rating> Last90DaysRatings()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT r.Id
                FROM Rating r
                INNER JOIN Work w ON w.Id = r.WorkId
                WHERE r.AuthorId = @AuthorId
                    AND w.CreatedAt >= @After
                ORDER BY r.Id";

            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) { Value = _id.Value() });
            command.Parameters.Add(
                new SqlParameter("@After", SqlDbType.DateTimeOffset)
                {
                    Value = DateTimeOffset.Now - TimeSpan.FromDays(90)
                }
            );

            using var reader = command.ExecuteReader();

            var ratings = new List<Rating>();

            while (reader.Read())
            {
                ratings.Add(new SqlServerRating(_connection, new DefaultId(reader["Id"])));
            }

            return ratings;
        }

        public string Repository()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Repository FROM Author WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = _id.Value() });

            using var reader = command.ExecuteReader();

            reader.Read();

            return (string)reader["Repository"];
        }
    }
}