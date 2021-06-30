using EatOut.Common;
using System;

namespace EatOut.Configuration
{
    public class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public const string BypassSecurityKey = "BypassSecurity";
        private const string WhitelistedAppsConst = "WhitelistedApps";
        private readonly IConfigurationReader configurationReader;
        public AuthorizationConfiguration(IConfigurationReader configurationReader)
        {
            Validate.IsNotNull(configurationReader, nameof(configurationReader));

            this.configurationReader = configurationReader;
        }

        /// <inheritdoc/>
        public string WhitelistAppsLiteral => WhitelistedAppsConst;

        /// <inheritdoc/>
        public bool BypassSecurity()
        {
            return string.Equals(configurationReader.ReadSetting(BypassSecurityKey), "True", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public string WhitelistApps() => configurationReader.ReadSetting(WhitelistedAppsConst);
    }

}

