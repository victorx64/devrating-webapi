using System;
using System.Data;
using DevRating.DefaultObject;
using DevRating.Domain;
using DevRating.WebApi.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal class SqlServerInsertKeyOperation : InsertKeyOperation
    {
        private readonly IDbConnection _connection;

        public SqlServerInsertKeyOperation(IDbConnection connection)
        {
            _connection = connection;
        }

        public Key Insert(string value, Id organization, DateTimeOffset createdAt)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Key
                    (Value, OrganizationId, CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                    (@Value, @OrganizationId, @CreatedAt)";

            command.Parameters.Add(new SqlParameter("@Value", SqlDbType.NVarChar, 256) {Value = value});
            command.Parameters.Add(new SqlParameter("@OrganizationId", SqlDbType.Int) {Value = organization.Value()});
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) {Value = createdAt});

            return new SqlServerKey(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}