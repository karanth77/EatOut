using EatOut.Common;
using Microsoft.Extensions.Configuration;

namespace EatOut.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        IConfiguration configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            Validate.IsNotNull(configuration, nameof(configuration));

            this.configuration = configuration;
        }

        public static ConfigurationService Create(IConfiguration configuration)
        {
            Validate.IsNotNull(configuration, nameof(configuration));

            return new ConfigurationService(configuration);
        }

        public IAuthorizationConfiguration Authorization
        {
            get { return new AuthorizationConfiguration(new ConfigurationReader(configuration));  }
        }

        public string ReadSetting(string key, string defaultValue = null, bool throwIfNotFound = false)
        {
            Validate.IsNotNullOrWhiteSpace(key, nameof(key));
            var configurationReader = new ConfigurationReader(configuration);
            return configurationReader.ReadSetting(key, defaultValue, throwIfNotFound);
        }
    }
}

