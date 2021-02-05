using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevRating.Domain;

namespace DevRating.SqlServerClient
{
    public sealed class EntitiesCollection : JsonObject
    {
        private readonly IEnumerable<JsonObject> _entities;

        public EntitiesCollection(IEnumerable<JsonObject> entities)
        {
            _entities = entities;
        }

        public string ToJson()
        {
            var builder = new StringBuilder("[");

            if (_entities.Any())
            {
                foreach (var author in _entities)
                {
                    builder.Append(author.ToJson());
                    builder.Append(",");
                }

                builder.Remove(builder.Length - 1, 1);
            }

            builder.Append("]");

            return builder.ToString();
        }
    }
}