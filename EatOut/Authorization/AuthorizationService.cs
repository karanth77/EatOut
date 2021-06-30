using EatOut.Common;
using EatOut.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EatOut.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        public const string AppIdClaim = "appid";

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
        /// </summary>
        /// <param name="configService">The configuration service.</param>
        /// <param name="loggerFactory">The logger.</param>
        public AuthorizationService(IConfigurationService configService, ILoggerFactory loggerFactory)
        {
            ConfigService = configService ?? throw new ArgumentNullException($"{nameof(configService)} is null");
            logger = (loggerFactory ?? throw new ArgumentNullException($"{nameof(loggerFactory)} is null"))
                .CreateLogger(nameof(AuthorizationService)) ?? throw new ArgumentNullException($"{nameof(logger)} resolved into null");
        }

        private IConfigurationService ConfigService { get; }

        public bool CheckAuthorized(HttpRequest req)
        {
            Validate.IsNotNull(req, nameof(req));

            logger.LogInformation("CheckAuthorized: IncomingPrincipal from httpcontext");
            var incomingPrincipal = req?.HttpContext?.User;

            if (incomingPrincipal?.Claims != null)
            {
                if (incomingPrincipal.Claims != null)
                {
                    logger.LogInformation("CheckAuthorized: No claim provided.");
                }
                else
                {
                    logger.LogInformation("CheckAuthorized going through claims.");
                    var appids = incomingPrincipal.Claims.Where(c => string.Compare(c.Type, AppIdClaim, StringComparison.OrdinalIgnoreCase) == 0);
                    var whitelistedApps = GetWhitelistedApps();

                    foreach (var appid in appids)
                    {
                        if (whitelistedApps.Contains(appid.Value))
                        {
                            return true;
                        }
                    }
                }
            }

            logger.LogWarning("CheckAuthorized false.");
            return false;
        }

        private HashSet<string> GetWhitelistedApps()
        {
            var res = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var whitelistedApps = ConfigService.Authorization.WhitelistApps();
            if (!string.IsNullOrWhiteSpace(whitelistedApps))
            {
                res = new HashSet<string>(whitelistedApps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase);
            }

            return res;
        }
    }
}
