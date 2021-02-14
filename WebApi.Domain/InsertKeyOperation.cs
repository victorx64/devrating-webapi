using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface InsertKeyOperation
    {
        Key Insert(string? name, string value, string organization, DateTimeOffset createdAt);
        Key Revoke(Id id, DateTimeOffset revokedAt);
    }
}