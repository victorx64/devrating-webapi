using System;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerInsertAuthorOperation : InsertAuthorOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerInsertAuthorOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Author Insert(string organization, string repository, string email, DateTimeOffset createdAt)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Author
                    (Organization, Repository, Email, CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                    (@Organization, @Repository, @Email, @CreatedAt)";

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) {Value = email});
            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) {Value = organization});
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) {Value = repository});
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) {Value = createdAt});

            return new SqlServerAuthor(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}