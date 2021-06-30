namespace EatOut.Authorization
{
    public interface IAuthorizationServiceFactory
    {
        IAuthorizationService GetAuthorizationService(AuthorizationType authorizationType);
    }
}
