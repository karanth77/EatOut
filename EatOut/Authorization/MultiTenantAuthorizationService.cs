using EatOut.Common;
using EatOut.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EatOut.Authorization
{
    public class MultiTenantAuthorizationService : IAuthorizationService
    {
        public const string AppIdClaim = "appid";
        public const string TenantIdClaim = "http://schemas.microsoft.com/identity/claims/tenantid";

        private static readonly char[] ValueSeparator = new char[] { ':' };
        private readonly ILogger logger;
        private readonly IConfigurationService configService;

        /// <summary>
        /// Performs authorization using both the appId and tenantId for applications that allow requests from different tenants
        /// </summary>
        public MultiTenantAuthorizationService(IConfigurationService configService, ILoggerFactory loggerFactory)
        {
            this.configService = configService ?? throw new ArgumentNullException($"{nameof(configService)} is null");
            logger = (loggerFactory ?? throw new ArgumentNullException($"{nameof(loggerFactory)} is null"))
                .CreateLogger(nameof(AuthorizationService)) ?? throw new ArgumentNullException($"{nameof(logger)} resolved into null");
        }

        public bool CheckAuthorized(HttpRequest req)
        {
            Validate.IsNotNull(req, nameof(req));
            var errorMessage = string.Empty;

            logger.LogInformation("CheckAuthorized: IncomingPrincipal from httpcontext");
            ClaimsPrincipal incomingPrincipal = req?.HttpContext?.User;

            if (incomingPrincipal?.Claims != null)
            {
                logger.LogInformation("CheckAuthorized going through claims.");

                var appId = incomingPrincipal.Claims.Where(c => string.Equals(c.Type, AppIdClaim, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                var tId = incomingPrincipal.Claims.Where(c => string.Equals(c.Type, TenantIdClaim, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                var whitelistedAppIdTenantIdPair = GetWhitelistedAppIdTenantIdPair();

                if (appId != null && tId != null && whitelistedAppIdTenantIdPair.ContainsKey(appId.Value))
                {
                    logger.LogInformation("CheckAuthorized AppId: " + appId.Value + " TenantId: " + tId.Value);
                    return whitelistedAppIdTenantIdPair.TryGetValue(appId.Value, out string value) && value.Equals(tId.Value);
                }
                else
                {
                    errorMessage += tId is null ? "tld is null." : null;
                    errorMessage += appId != null ? (whitelistedAppIdTenantIdPair.ContainsKey(appId.Value) ? "Value is not whitelisted" : null) : "AppId is null.";
                }
            }
            else
            {
                errorMessage = "CheckAuthorized Claim is null";
            }

            logger.LogWarning($"CheckAuthorized false: {errorMessage}");
            return false;
        }

        /// <summary>
        /// Get appId:tenantId value from settings
        /// </summary>
        private IDictionary<string, string> GetWhitelistedAppIdTenantIdPair()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var whitelistedAppIdTenantIdPairs = configService.ReadSetting("WhitelistedAppIdTenantIdPairs");

            if (!string.IsNullOrWhiteSpace(whitelistedAppIdTenantIdPairs))
            {
                var whiteListedValues = whitelistedAppIdTenantIdPairs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var value in whiteListedValues)
                {
                    var v = value.Split(ValueSeparator, StringSplitOptions.RemoveEmptyEntries);
                    dict.TryAdd(v[0], v[1]);
                }
            }

            return dict;
        }
    }
}
