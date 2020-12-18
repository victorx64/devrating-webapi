using DevRating.Domain;

namespace DevRating.WebApi.Domain
{
    public interface Database
    {
        Entities Entities();
        DbInstance Instance();
    }
}