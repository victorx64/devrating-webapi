using System;
using System.Linq;
using DevRating.DefaultObject;
using Xunit;

namespace DevRating.SqlServerClient.Test
{
    public sealed class SqlServerDatabaseAuthorsTest
    {
        [Fact]
        public void ChecksInsertedAuthorById()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                Assert.True(database.Entities().Authors().ContainsOperation()
                    .Contains(database.Entities().Authors().InsertOperation()
                        .Insert("organization", "repository", "email", DateTimeOffset.UtcNow).Id()));
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ChecksInsertedAuthorByCreds()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";
                var repo = "repo";

                Assert.True(
                    database.Entities().Authors().ContainsOperation().Contains(
                        organization,
                        repo,
                        database
                            .Entities()
                            .Authors()
                            .InsertOperation()
                            .Insert(organization, repo, "email", DateTimeOffset.UtcNow)
                            .Email()
                    )
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void InsertsLongEmail()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());
            database.Instance().Connection().Open();
            database.Instance().Create();
            try
            {
                var organization = "organization";
                var repo = "repo";
                var email = "longer.than.50.longer.than.50.longer.than.50.longer.than.50.longer.than.50";

                Assert.Equal(
                    email,
                    database
                        .Entities()
                        .Authors()
                        .InsertOperation()
                        .Insert(organization, repo, email, DateTimeOffset.UtcNow)
                        .Email()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsInsertedAuthorByCreds()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";
                var repo = "repo";

                var author = database.Entities().Authors().InsertOperation()
                    .Insert(organization, repo, "email", DateTimeOffset.UtcNow);

                Assert.Equal(author.Id(),
                    database.Entities().Authors().GetOperation().Author(organization, repo, author.Email()).Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsInsertedAuthorById()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var author = database.Entities().Authors().InsertOperation()
                    .Insert("organization", "repo", "email", DateTimeOffset.UtcNow);

                Assert.Equal(author.Id(), database.Entities().Authors().GetOperation().Author(author.Id()).Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsTop()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";
                var repo = "repo";
                var createdAt = DateTimeOffset.UtcNow;
                var author1 = database.Entities().Authors().InsertOperation().Insert(organization, repo, "email1", createdAt);
                var author2 = database.Entities().Authors().InsertOperation().Insert(organization, repo, "email2", createdAt);
                var author3 = database.Entities().Authors().InsertOperation()
                    .Insert("ANOTHER organization", repo, "email3", createdAt);

                var work = database.Entities().Works().InsertOperation().Insert(
                    "start",
                    "end",
                    null,
                    author1.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    createdAt
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    100,
                    null,
                    null,
                    new DefaultId(),
                    work.Id(),
                    author1.Id()
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    50,
                    null,
                    null,
                    new DefaultId(),
                    work.Id(),
                    author2.Id()
                );

                var work2 = database.Entities().Works().InsertOperation().Insert(
                    "other start",
                    "other end",
                    null,
                    author3.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    createdAt
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    150,
                    null,
                    null,
                    new DefaultId(),
                    work2.Id(),
                    author3.Id()
                );

                Assert.Equal(
                    author1.Id(), 
                    database.Entities()
                        .Authors()
                        .GetOperation()
                        .Top(organization, repo, createdAt - TimeSpan.FromDays(1))
                        .First()
                        .Id()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void AllowsSameAuthorForTwoRepos()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";
                var email = "email";

                var moment = DateTimeOffset.UtcNow;

                var author1 = database.Entities().Authors().InsertOperation()
                    .Insert(organization, "repo1", email, moment);
                var author2 = database.Entities().Authors().InsertOperation()
                    .Insert(organization, "repo2", email, moment);

                Assert.Equal(
                    author1.Organization() + author1.Email(),
                    author2.Organization() + author2.Email()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ThrowsOnInsertingSameAuthor()
        {
            Assert.Throws<Microsoft.Data.SqlClient.SqlException>(
                () =>
                {
                    var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

                    database.Instance().Connection().Open();
                    database.Instance().Create();

                    try
                    {
                        var organization = "organization";
                        var email = "email";
                        var repo = "repo";
                        var moment = DateTimeOffset.UtcNow;

                        var author1 = database.Entities().Authors().InsertOperation()
                                .Insert(organization, repo, email, moment);
                        var author2 = database.Entities().Authors().InsertOperation()
                                .Insert(organization, repo, email, moment);
                    }
                    finally
                    {
                        database.Instance().Connection().Close();
                    }
                }
            );
        }
    }
}