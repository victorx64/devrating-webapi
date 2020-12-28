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

        public Key Insert(string? name, string value, Id organization, DateTimeOffset createdAt)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO [Key]
                    (Name, Value, OrganizationId, CreatedAt)
                OUTPUT Inserted.Id
                VALUES
                    (@Name, @Value, @OrganizationId, @CreatedAt)";

            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 256) { Value = name });
            command.Parameters.Add(new SqlParameter("@Value", SqlDbType.NVarChar, 256) { Value = value });
            command.Parameters.Add(new SqlParameter("@OrganizationId", SqlDbType.Int) { Value = organization.Value() });
            command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTimeOffset) { Value = createdAt });

            return new SqlServerKey(_connection, new DefaultId(command.ExecuteScalar()!));
        }

        public Key Revoke(Id id, DateTimeOffset revokedAt)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                UPDATE [Key]
                SET
                    RevokedAt = @RevokedAt
                OUTPUT Inserted.Id
                WHERE
                    Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id.Value() });
            command.Parameters.Add(new SqlParameter("@RevokedAt", SqlDbType.DateTimeOffset) { Value = revokedAt });

            return new SqlServerKey(_connection, new DefaultId(command.ExecuteScalar()!));
        }
    }
}