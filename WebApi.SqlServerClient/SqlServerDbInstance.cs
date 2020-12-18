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
                create table User
                (
                    Id int identity
                        constraint PK_User
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    ForeignId nvarchar(max) not null,
                    constraint UK_User_ForeignId
                        unique (ForeignId)
                );

                create table Organization
                (
                    Id int identity
                        constraint PK_Organization
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    Name nvarchar(256) not null,
                    constraint UK_Organization_Name
                        unique (Name),
                    UserId int not null
                        constraint FK_Organization_UserId
                            references User
                );

                create table Key
                (
                    Id int identity
                        constraint PK_Key
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
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
            return HasTable("User") &&
                   HasTable("Organization") &&
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