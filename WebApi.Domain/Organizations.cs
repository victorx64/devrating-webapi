namespace DevRating.WebApi.Domain
{
    public interface Organizations
    {
        InsertOrganizationOperation InsertOperation();
        GetOrganizationOperation GetOperation();
        ContainsOrganizationOperation ContainsOperation();
    }
}