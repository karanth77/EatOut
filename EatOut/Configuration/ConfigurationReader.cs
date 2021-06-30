using EatOut.Common;
using Microsoft.Extensions.Configuration;
using System;

namespace EatOut.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration class.</param>
        /// <param name="fallbackConfiguration">The backup configuration class.  The value is optional.</param>
        public ConfigurationReader(IConfiguration configuration)
        {
            Validate.IsNotNull(configuration, nameof(configuration));

            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public string ReadSetting(string key, string defaultValue = null, bool throwIfNotFound = false)
        {
            Validate.IsNotNullOrWhiteSpace(key, nameof(key));

            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                if (throwIfNotFound)
                {
                    if (!string.IsNullOrWhiteSpace(defaultValue))
                    {
                        return defaultValue;
                    }

                    throw new ArgumentException($"Environment variable {key} is null or empty");
                }

                return defaultValue;
            }

            return value;
        }
    }

}

