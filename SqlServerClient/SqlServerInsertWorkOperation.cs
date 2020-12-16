using System;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerInsertWorkOperation : InsertWorkOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerInsertWorkOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Work Insert(
            string repository,
            string start,
            string end,
            string? since,
            Id author,
            uint additions,
            Id rating,
            string? link,
            DateTimeOffset createdAt
        )
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Work
                    (Repository
                    ,Link
                    ,StartCommit
                    ,EndCommit
                    ,SinceCommit
                    ,AuthorId
                    ,Additions
                    ,UsedRatingId
                    ,CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                    (@Repository
                    ,@Link
                    ,@StartCommit
                    ,@EndCommit
                    ,@SinceCommit
                    ,@AuthorId
                    ,@Additions
                    ,@UsedRatingId
                    ,@CreatedAt)";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = repository});
            command.Parameters.Add(new SqlParameter("@Link", SqlDbType.NVarChar) {Value = link ?? (object) DBNull.Value});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50) {Value = start});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = end});
            command.Parameters.Add(new SqlParameter("@SinceCommit", SqlDbType.NVarChar, 50) {Value = since ?? (object) DBNull.Value});
            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Value()});
            command.Parameters.Add(new SqlParameter("@Additions", SqlDbType.Int) {Value = additions});
            command.Parameters.Add(new SqlParameter("@UsedRatingId", SqlDbType.Int) {Value = rating.Value()});
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) {Value = createdAt});

            return new SqlServerWork(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}