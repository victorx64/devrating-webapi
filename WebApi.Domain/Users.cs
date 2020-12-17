namespace DevRating.WebApi.Domain
{
    public interface Users
    {
        GetUserOperation GetOperation();
        InsertUserOperation InsertOperation();
        ContainsUserOperation ContainsOperation();
    }
}