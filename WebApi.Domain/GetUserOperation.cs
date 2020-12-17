using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface GetUserOperation
    {
        User User(string foreignId);
        User User(Id id);
    }
}