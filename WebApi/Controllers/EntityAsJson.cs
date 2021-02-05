using System.Collections.Generic;
using DevRating.Domain;
using DevRating.SqlServerClient;
using Microsoft.AspNetCore.Mvc;

namespace DevRating.WebApi.Controllers
{
    public sealed class EntityAsJson : ContentResult
    {
        public EntityAsJson(IEnumerable<JsonObject> entities) : this(new EntitiesCollection(entities))
        {
        }

        public EntityAsJson(JsonObject entity)
        {
            Content = entity.ToJson();
            ContentType = "application/json";
        }

        public EntityAsJson(object @object)
        {
            Content = @object.ToString();
            ContentType = "application/json";
        }
    }
}
