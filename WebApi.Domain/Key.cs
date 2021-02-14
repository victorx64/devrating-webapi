using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface Key : Entity
    {
        string? Name();
        string Value();
        DateTimeOffset? RevokedAt();
        string Organization();
        DateTimeOffset CreatedAt();
    }
}