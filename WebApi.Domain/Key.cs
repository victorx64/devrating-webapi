using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface Key : Entity
    {
        string Value();
        Organization Organization();
        DateTimeOffset CreatedAt();
    }
}