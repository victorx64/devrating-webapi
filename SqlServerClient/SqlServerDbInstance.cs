using System.Data;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
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
                create table Author
                (
                    Id int identity
                        constraint PK_Author
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    Organization nvarchar(256) not null,
                    Email nvarchar(256) not null,
                    constraint UK_Author_Organization_Email
                        unique (Organization, Email)
                );

                create table Rating
                (
                    Id int identity
                        constraint PK_Rating
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    Rating real not null,
                    CountedDeletions int,
                    IgnoredDeletions int,
                    PreviousRatingId int
                        constraint FK_Rating_PreviousRatingId
                            references Rating,
                    WorkId int not null,
                    AuthorId int not null
                        constraint FK_Rating_AuthorId
                            references Author
                );

                create unique index UK_Rating_PreviousRatingId
                    on Rating (PreviousRatingId)
                    where [PreviousRatingId] IS NOT NULL;

                create table Work
                (
                    Id int identity
                        constraint PK_Work
                            primary key,
                    CreatedAt datetimeoffset(7) not null,
                    Repository nvarchar(max) not null,
                    Link nvarchar(max),
                    StartCommit nvarchar(50) not null,
                    EndCommit nvarchar(50) not null,
                    SinceCommit nvarchar(50),
                    AuthorId int not null
                        constraint FK_Work_AuthorId
                            references Author,
                    Additions int not null,
                    UsedRatingId int
                        constraint FK_Work_RatingId
                            references Rating,
                    constraint UK_Work_Commits
                        unique (StartCommit, EndCommit)
                );

                alter table Rating
                    add constraint FK_Rating_WorkId
                        foreign key (WorkId) references Work;

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
            return HasTable("Author") &&
                   HasTable("Rating") &&
                   HasTable("Work");
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