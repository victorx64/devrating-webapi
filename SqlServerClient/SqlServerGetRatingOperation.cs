using System;
using System.Collections.Generic;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerGetRatingOperation : GetRatingOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetRatingOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Rating RatingOf(Id author)
        {
            using var command = _connection.CreateCommand();

            command.CommandText =
                "SELECT TOP (1) Id FROM Rating WHERE AuthorId = @AuthorId ORDER BY Id DESC";

            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Value()});

            using var reader = command.ExecuteReader();

            return new SqlServerRating(_connection, new DefaultId(reader.Read() ? reader["Id"] : DBNull.Value));
        }

        public Rating Rating(Id id)
        {
            return new SqlServerRating(_connection, id);
        }

        public IEnumerable<Rating> RatingsOf(Id work)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Rating WHERE WorkId = @WorkId";

            command.Parameters.Add(new SqlParameter("@WorkId", SqlDbType.Int) {Value = work.Value()});

            using var reader = command.ExecuteReader();

            var ratings = new List<Rating>();

            while (reader.Read())
            {
                ratings.Add(new SqlServerRating(_connection, new DefaultId(reader["Id"])));
            }

            return ratings;
        }

        public IEnumerable<Rating> Last(Id author, DateTimeOffset after)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "SELECT Id FROM Rating WHERE AuthorId = @AuthorId AND CreatedAt >= @After";

            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Value()});
            command.Parameters.Add(new SqlParameter("@After", SqlDbType.DateTimeOffset) {Value = after});

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