using EatOut.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace EatOut.Authorization
{
    public class AuthorizationServiceFactory : IAuthorizationServiceFactory
    {
        private readonly IConfigurationService configService;
        private readonly ILoggerFactory loggerFactory;

        public AuthorizationServiceFactory(IConfigurationService configService, ILoggerFactory loggerFactory)
        {
            this.configService = configService ?? throw new ArgumentNullException($"{nameof(configService)} is null");
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException($"{nameof(loggerFactory)} is null");
        }

        public IAuthorizationService GetAuthorizationService(AuthorizationType authorizationType)
        {
            if (IsDebugBuild() && configService.Authorization.BypassSecurity())
            {
                return new DevEnvAuthorizationService(loggerFactory);
            }

            return authorizationType switch
            {
                AuthorizationType.MultiTenantAuthorization => new MultiTenantAuthorizationService(configService, loggerFactory),
                _ => new AuthorizationService(configService, loggerFactory),
            };
        }

        private bool IsDebugBuild()
        {
#if DEBUG
            return true;
#else
            return false;
#endif

        }
    }
}
