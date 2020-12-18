using System;
using System.Data;
using DevRating.DefaultObject;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerInsertUserOperation : InsertUserOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerInsertUserOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public User Insert(string foreignId, DateTimeOffset createdAt)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO User
                    (ForeignId, CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                    (@ForeignId, @CreatedAt)";

            command.Parameters.Add(new SqlParameter("@ForeignId", SqlDbType.NVarChar) {Value = foreignId});
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) {Value = createdAt});

            return new SqlServerUser(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}