using System;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerInsertRatingOperation : InsertRatingOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerInsertRatingOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Rating Insert(
            double value,
            uint? counted,
            uint? ignored,
            Id previous,
            Id work,
            Id author,
            DateTimeOffset createdAt
        )
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Rating
                    (Rating
                    ,CountedDeletions
                    ,IgnoredDeletions
                    ,PreviousRatingId
                    ,WorkId
                    ,AuthorId
                    ,CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                       (@Rating
                       ,@CountedDeletions
                       ,@IgnoredDeletions
                       ,@PreviousRatingId
                       ,@WorkId
                       ,@AuthorId
                       ,@CreatedAt)";

            command.Parameters.Add(new SqlParameter("@Rating", SqlDbType.Real) {Value = value});
            command.Parameters.Add(new SqlParameter("@PreviousRatingId", SqlDbType.Int) {Value = previous.Value()});
            command.Parameters.Add(new SqlParameter("@WorkId", SqlDbType.Int) {Value = work.Value()});
            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Value()});
            command.Parameters.Add(new SqlParameter("@CountedDeletions", SqlDbType.Int) {Value = counted ?? (object) DBNull.Value});
            command.Parameters.Add(new SqlParameter("@IgnoredDeletions", SqlDbType.Int) {Value = ignored ?? (object) DBNull.Value});
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) {Value = createdAt});

            return new SqlServerRating(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}