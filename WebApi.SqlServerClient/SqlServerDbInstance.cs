using System.Data;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.WebApi.SqlServerClient
{
    internal sealed class SqlServerDbInstance : DbInstance
    {
        private readonly IDbConnection _connection;

        public SqlServerDbInstance(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Create()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                create table Organization
                (
                    Id int identity
                        constraint PK_Organization
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    Name nvarchar(256) not null,
                    UserId nvarchar(max) not null,
                    constraint UK_Organization_Name
                        unique (Name)
                );

                create table Key
                (
                    Id int identity
                        constraint PK_Key
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    RevokedAt datetimeoffset(7),
                    Value nvarchar(256) not null,
                    OrganizationId int not null
                        constraint FK_Key_OrganizationId
                            references Organization
                );
            ";

            command.ExecuteNonQuery();
        }

        public bool Present()
        {
            return HasTable("Organization") &&
                   HasTable("Key");
        }

        public IDbConnection Connection()
        {
            return _connection;
        }

        private bool HasTable(string name)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE'
                AND TABLE_NAME = @table";

            command.Parameters.Add(new SqlParameter("@table", SqlDbType.NVarChar) {Value = name});

            var reader = command.ExecuteReader();

            var exist = reader.Read();

            reader.Close();

            return exist;
        }
    }
}