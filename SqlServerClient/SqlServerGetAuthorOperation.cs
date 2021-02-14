using System;
using System.Collections.Generic;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerGetAuthorOperation : GetAuthorOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerGetAuthorOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Author Author(string organization, string repository, string email)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id
                FROM Author
                WHERE Email = @Email
                AND Organization = @Organization
                AND Repository = @Repository";

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = email });
            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) { Value = organization });
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) { Value = repository });

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerAuthor(_connection, new DefaultId(reader["Id"]));
        }

        public Author Author(Id id)
        {
            return new SqlServerAuthor(_connection, id);
        }

        public IEnumerable<Author> Top(string organization, string repository, DateTimeOffset after)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT a.Id
                FROM Author a
                INNER JOIN Rating r1 ON a.Id = r1.AuthorId
                LEFT OUTER JOIN Rating r2 ON (a.id = r2.AuthorId AND r1.Id < r2.Id)
                WHERE a.Organization = @Organization AND a.Repository = @Repository AND r1.CreatedAt > @After
                AND r2.Id IS NULL
                ORDER BY r1.Rating DESC";

            command.Parameters.Add(new SqlParameter("@Organization", SqlDbType.NVarChar, 256) { Value = organization });
            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar, 256) { Value = repository });
            command.Parameters.Add(new SqlParameter("@After", SqlDbType.DateTimeOffset) { Value = after });

            using var reader = command.ExecuteReader();

            var authors = new List<SqlServerAuthor>();

            while (reader.Read())
            {
                authors.Add(new SqlServerAuthor(_connection, new DefaultId(reader["Id"])));
            }

            return authors;
        }
    }
}