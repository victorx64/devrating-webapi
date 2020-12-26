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
                        .Insert("organization", "email", DateTimeOffset.UtcNow).Id()));
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ChecksInsertedAuthorByOrgAndEmail()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";

                Assert.True(database.Entities().Authors().ContainsOperation().Contains(organization,
                    database.Entities().Authors().InsertOperation().Insert(organization, "email", DateTimeOffset.UtcNow)
                        .Email()));
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsInsertedAuthorByOrgAndEmail()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";

                var author = database.Entities().Authors().InsertOperation()
                    .Insert(organization, "email", DateTimeOffset.UtcNow);

                Assert.Equal(author.Id(),
                    database.Entities().Authors().GetOperation().Author(organization, author.Email()).Id());
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
                    .Insert("organization", "email", DateTimeOffset.UtcNow);

                Assert.Equal(author.Id(), database.Entities().Authors().GetOperation().Author(author.Id()).Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsOrganizationTopAuthors()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";
                var createdAt = DateTimeOffset.UtcNow;
                var author1 = database.Entities().Authors().InsertOperation().Insert(organization, "email1", createdAt);
                var author2 = database.Entities().Authors().InsertOperation().Insert(organization, "email2", createdAt);
                var author3 = database.Entities().Authors().InsertOperation()
                    .Insert("ANOTHER organization", "email3", createdAt);

                var work = database.Entities().Works().InsertOperation().Insert(
                    "repo",
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
                    author1.Id(),
                    createdAt
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    50,
                    null,
                    null,
                    new DefaultId(),
                    work.Id(),
                    author2.Id(),
                    createdAt
                );

                var work2 = database.Entities().Works().InsertOperation().Insert(
                    "other repo",
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
                    author3.Id(),
                    createdAt
                );

                Assert.Equal(author1.Id(),
                    database.Entities().Authors().GetOperation()
                    .TopOfOrganization(organization, createdAt - TimeSpan.FromDays(1)).First().Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsRepoTopAuthors()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var organization = "organization";
                var createdAt = DateTimeOffset.UtcNow;
                var author1 = database.Entities().Authors().InsertOperation().Insert(organization, "email1", createdAt);
                var author2 = database.Entities().Authors().InsertOperation().Insert(organization, "email2", createdAt);
                var author3 = database.Entities().Authors().InsertOperation().Insert(organization, "email3", createdAt);

                var repository = "first repo";

                var work1 = database.Entities().Works().InsertOperation().Insert(
                    repository,
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
                    work1.Id(),
                    author1.Id(),
                    createdAt
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    50,
                    null,
                    null,
                    new DefaultId(),
                    work1.Id(),
                    author2.Id(),
                    createdAt
                );

                var work2 = database.Entities().Works().InsertOperation().Insert(
                    "OTHER REPOSITORY",
                    "some start commit",
                    "some end commit",
                    null,
                    author1.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    createdAt
                );

                database.Entities().Ratings().InsertOperation().Insert(
                    75,
                    null,
                    null,
                    new DefaultId(),
                    work2.Id(),
                    author3.Id(),
                    createdAt
                );

                Assert.Equal(2, database.Entities().Authors().GetOperation()
                .TopOfRepository(repository, createdAt - TimeSpan.FromDays(1)).Count());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
    }
}