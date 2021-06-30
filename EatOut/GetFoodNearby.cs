using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EatOut.Authorization;
using EatOut.Common;
using System.Threading;

namespace EatOut
{
    public class GetFoodNearby: HttpAzureFunctionBase
    {
        private readonly ILocationDomainService domainService;

        public GetFoodNearby(ILocationDomainService domainService, IAuthorizationServiceFactory authorizationServiceFactory, ILogger<GetFoodNearby> logger)
        : base(authorizationServiceFactory?.GetAuthorizationService(AuthorizationType.MultiTenantAuthorization), logger)
        {
            Validate.IsNotNull(domainService, nameof(domainService));
            this.domainService = domainService;
        }
        /// <summary>
        /// This Method returns the list of all locations based on the distance.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("GetFoodNearby")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetFoodNearby")] HttpRequest req,
            CancellationToken cancellationToken = default)
        {
            return await ProcessPost(
                req,
                process: async (Location request) =>
                {
                    try
                    {
                        return new OkObjectResult(await domainService.FindNearBy(request, cancellationToken));
                    }
                    catch (ArgumentException ex)
                    {
                        return new BadRequestObjectResult($"Error Validating Input: {ex}");
                    }
                },
                validate: (Location request) => request.ValidateRequiredValues());
        }
    }
}
