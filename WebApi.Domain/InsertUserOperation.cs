using System;

namespace DevRating.WebApi.Domain
{
    public interface InsertUserOperation
    {
        User Insert(string foreignId, DateTimeOffset createdAt);
    }
}