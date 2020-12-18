using System;
using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface InsertKeyOperation
    {
        Key Insert(string value, Id organization, DateTimeOffset createdAt);
    }
}