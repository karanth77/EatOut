using EatOut.Authorization;
using EatOut.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(EatOut.Startup))]

namespace EatOut
{
    public class Startup : FunctionsStartup
    {
        // Use Dependency Injection to build all services.
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IConfigurationService, ConfigurationService>((s) =>
            {
                var configuration = s.GetService<IConfiguration>();
                return ConfigurationService.Create(configuration);
            });

            builder.Services.AddSingleton<ILocationDomainService, LocationDomainService>();

            builder.Services.AddSingleton<IVendorRepository, VendorRepository>();

            builder.Services.AddHttpClient<IVendorRepository, VendorRepository>();

            builder.Services.AddSingleton<IAuthorizationServiceFactory, AuthorizationServiceFactory>();
            
            // We can add additional services like HealthCheck, other Repository etc here
        }
    }
}
