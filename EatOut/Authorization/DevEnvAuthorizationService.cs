using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace EatOut.Authorization
{
    public class DevEnvAuthorizationService : IAuthorizationService
    {
        private readonly ILogger logger;

        public DevEnvAuthorizationService(ILoggerFactory loggerFactory)
        {
            logger = (loggerFactory ?? throw new ArgumentNullException($"{nameof(loggerFactory)} is null"))
                .CreateLogger(nameof(AuthorizationService)) ?? throw new ArgumentNullException($"{nameof(logger)} resolved into null");
        }

        public bool CheckAuthorized(HttpRequest req)
        {
#if DEBUG
            logger.LogWarning($"In DevEnv - auth disabled.");
            return true;
#else
            throw new Exception("Security: DevEnvAuthorizationService service is used in release build");
#endif
        }
    }
}
