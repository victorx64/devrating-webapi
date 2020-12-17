using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface User : Entity
    {
        string ForeignId();
        DateTimeOffset CreatedAt();
    }
}