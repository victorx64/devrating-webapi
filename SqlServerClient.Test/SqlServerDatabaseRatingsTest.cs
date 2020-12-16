using System;
using System.Linq;
using DevRating.DefaultObject;
using Xunit;

namespace DevRating.SqlServerClient.Test
{
    public sealed class SqlServerDatabaseRatingsTest
    {
        [Fact]
        public void ChecksInsertedRatingById()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                Assert.True(
                    database.Entities().Ratings().ContainsOperation().Contains(
                        database.Entities().Ratings().InsertOperation().Insert(
                            1100d,
                            null,
                            null,
                            new DefaultId(),
                            database.Entities().Works().InsertOperation().Insert(
                                "repo",
                                "startCommit",
                                "endCommit",
                                null,
                                author.Id(),
                                1u,
                                new DefaultId(),
                                null,
                                createdAt
                            ).Id(),
                            author.Id(),
                            createdAt
                        ).Id()
                    )
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ChecksInsertedRatingByAuthorId()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                database.Entities().Ratings().InsertOperation().Insert(
                    1100d,
                    null,
                    null,
                    new DefaultId(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        null,
                        author.Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Id(),
                    author.Id(),
                    createdAt
                );

                Assert.True(database.Entities().Ratings().ContainsOperation().ContainsRatingOf(author.Id()));
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsInsertedRatingById()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var rating = database.Entities().Ratings().InsertOperation().Insert(
                    1100d,
                    null,
                    null,
                    new DefaultId(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        null,
                        author.Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Id(),
                    author.Id(),
                    createdAt
                );

                Assert.Equal(rating.Id(), database.Entities().Ratings().GetOperation().Rating(rating.Id()).Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsInsertedRatingByAuthorId()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var rating = database.Entities().Ratings().InsertOperation().Insert(
                    1100d,
                    null,
                    null,
                    new DefaultId(),
                    database.Entities().Works().InsertOperation().Insert(
                        "repo",
                        "startCommit",
                        "endCommit",
                        null,
                        author.Id(),
                        1u,
                        new DefaultId(),
                        null,
                        createdAt
                    ).Id(),
                    author.Id(),
                    createdAt
                );

                Assert.Equal(rating.Id(), database.Entities().Ratings().GetOperation().RatingOf(author.Id()).Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsUnfilledRatingIfNotFoundByAuthorId()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                Assert.False(database.Entities().Ratings().GetOperation().RatingOf(new DefaultId(new Random().Next()))
                    .Id()
                    .Filled());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsInsertedRatingByWorkId()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var createdAt = DateTimeOffset.UtcNow;
                var author = database.Entities().Authors().InsertOperation().Insert("organization", "email", createdAt);

                var work = database.Entities().Works().InsertOperation().Insert(
                    "repo",
                    "startCommit",
                    "endCommit",
                    null,
                    author.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    createdAt
                );

                var rating = database.Entities().Ratings().InsertOperation().Insert(
                    1100d,
                    null,
                    null,
                    new DefaultId(),
                    work.Id(),
                    author.Id(),
                    createdAt
                );

                Assert.Equal(rating.Id(),
                    database.Entities().Ratings().GetOperation().RatingsOf(work.Id()).Single().Id());
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }

        [Fact]
        public void ReturnsEmptyListIfNotFoundByWorkId()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                Assert.Empty(database.Entities().Ratings().GetOperation()
                    .RatingsOf(new DefaultId(new Random().Next())));
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
        
        [Fact]
        public void ReturnsLastInsertedRatingsOfAuthor()
        {
            var database = new SqlServerDatabase(new TemporalLocalSqlConnection());

            database.Instance().Connection().Open();
            database.Instance().Create();

            try
            {
                var author = database.Entities().Authors().InsertOperation()
                    .Insert("organization", "email", DateTimeOffset.UtcNow);

                var repository = "repo";
                var date = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

                var work1 = database.Entities().Works().InsertOperation().Insert(
                    repository,
                    "startCommit1",
                    "endCommit1",
                    null,
                    author.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    date
                );

                var rating1 = database.Entities().Ratings().InsertOperation().Insert(
                    1100d,
                    null,
                    null,
                    new DefaultId(),
                    work1.Id(),
                    author.Id(),
                    work1.CreatedAt() + TimeSpan.FromHours(1)
                );

                var work2 = database.Entities().Works().InsertOperation().Insert(
                    repository,
                    "startCommit2",
                    "endCommit2",
                    null,
                    author.Id(),
                    1u,
                    new DefaultId(),
                    null,
                    work1.CreatedAt() + TimeSpan.FromHours(0.5)
                );

                var rating2 = database.Entities().Ratings().InsertOperation().Insert(
                    1200d,
                    null,
                    null,
                    new DefaultId(),
                    work2.Id(),
                    author.Id(),
                    rating1.CreatedAt() + TimeSpan.FromHours(0.5)
                );

                Assert.Equal(
                    rating2.Id(),
                    database.Entities().Ratings().GetOperation().Last(
                            author.Id(),
                            rating2.CreatedAt()
                        )
                        .Single().Id()
                );
            }
            finally
            {
                database.Instance().Connection().Close();
            }
        }
    }
}