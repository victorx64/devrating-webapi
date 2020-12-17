using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface GetKeyOperation
    {
        Key Key(Id id);
    }
}