using System;

namespace DevRating.WebApi.Domain
{
    public interface InsertOrganizationOperation
    {
        Organization Insert(string name, string subject, DateTimeOffset createdAt);
    }
}