namespace DevRating.WebApi.Domain
{
    public interface Keys
    {
        InsertKeyOperation InsertOperation();
        GetKeyOperation GetOperation();
        ContainsKeyOperation ContainsOperation();
    }
}