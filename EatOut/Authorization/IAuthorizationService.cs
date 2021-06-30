using Microsoft.AspNetCore.Http;

namespace EatOut.Authorization
{
    public interface IAuthorizationService
    {
        bool CheckAuthorized(HttpRequest req);
    }
}
