using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using masterCore.Entities;

namespace masterApi.Validation
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var jValidations = new JArray();
                var responseData = new responseData
                {
                    error = true,
                    errorValue = 2,
                    description = "An error occurred, check others validations for more details."
                };

                foreach (var val in context.ModelState)
                {
                    var errorSt = string.Empty;

                    foreach (var item in val.Value.Errors)
                    {
                        var jValidation = new JObject
                        {
                            { "description", item.ErrorMessage }
                        };
                        jValidations.Add(jValidation);
                    }
                }

                responseData.othersValidations = new JObject { { "errors", jValidations } };

                context.Result = new OkObjectResult(responseData);
                return;
            }

            await next();
        }
    }
}
