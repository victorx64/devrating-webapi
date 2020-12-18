using System;
using System.Data;
using DevRating.DefaultObject;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerInsertOrganizationOperation : InsertOrganizationOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerInsertOrganizationOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Organization Insert(string name, string user, DateTimeOffset createdAt)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Organization
                    (UserId, Name, CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                    (@UserId, @Name, @CreatedAt)";

            command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) {Value = user});
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) {Value = name});
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) {Value = createdAt});

            return new SqlServerOrganization(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}