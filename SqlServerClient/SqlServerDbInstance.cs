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
                    Repository   nvarchar(256) not null,
                    Email nvarchar(256) not null,
                    constraint UK_Author_Organization_Email
                        unique (Organization, Repository, Email)
                );

                create table Rating
                (
                    Id int identity
                        constraint PK_Rating
                            primary key,
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
                );";

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