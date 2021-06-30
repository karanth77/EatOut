using EatOut.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EatOut.Authorization
{
    // Base class that every azure http function derives from to get common authentication and authorization methods
    public abstract class HttpAzureFunctionBase
    {
        protected readonly ILogger Logger;
        private readonly IAuthorizationService authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpAzureFunctionBase"/> class.
        /// </summary>
        /// <param name="authorizationService">The security service</param>
        public HttpAzureFunctionBase(IAuthorizationService authorizationService, ILogger<HttpAzureFunctionBase> logger)
        {
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> ProcessPost<TArg>(
            HttpRequest req,
            Func<TArg, Task<IActionResult>> process,
            Action<TArg> validate = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            [CallerMemberName] string operationName = null)
        {
            Validate.IsNotNull(req, nameof(req));
            Validate.IsNotNull(process, nameof(process));

            IActionResult res;
            if ((res = Authorize(req, operationName)) != null)
            {
                return res;
            }

            TArg input;
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                input = JsonConvert.DeserializeObject<TArg>(requestBody, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                var error = $"Couldn't deserialize {operationName} - {ex}";
                Logger.LogError(error);
                return new BadRequestObjectResult(error);
            }

            if (validate != null)
            {
                try
                {
                    validate(input);
                }
                catch (Exception ex)
                {
                    return LogValidationErrorAndReturn400(operationName, ex.Message);
                }
            }

            return await Execute(process, input, operationName);
        }

        private IActionResult Authorize(HttpRequest req, string operationName)
        {
            Validate.IsNotNullOrWhiteSpace(operationName, nameof(operationName));

            if (!this.authorizationService.CheckAuthorized(req))
            {
                Logger.LogWarning("Auth failed.");
                return new UnauthorizedResult();
            }

            return null;
        }

        private async Task<IActionResult> Execute<TArg>(Func<TArg, Task<IActionResult>> func, TArg input, string operationName)
        {
            try
            {
                return await func(input);
            }
            catch (Exception ex)
            {
                return LogAndReturn500(operationName, ex);
            }
        }

        private IActionResult LogAndReturn500(string operationName, Exception ex)
        {
            Logger.LogError(ex, $"Couldn't execute {operationName}.{ex}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        private IActionResult LogValidationErrorAndReturn400(string operationName, string validationSummary)
        {
            Logger.LogError($"Input is invalid: {operationName}", $"Error validating inputs {operationName}.{validationSummary}");
            return new BadRequestObjectResult($"Input is invalid: {validationSummary}");
        }
    }
}
