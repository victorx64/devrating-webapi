using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface Organization : Entity
    {
        string Name();
        User User();
        DateTimeOffset CreatedAt();
    }
}