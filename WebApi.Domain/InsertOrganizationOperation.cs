using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface InsertOrganizationOperation
    {
        Organization Insert(string name, Id user, DateTimeOffset createdAt);
    }
}