using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace FluentResults.Extensions.AspNetCore.Mvc
{
    public class ResultMvcFilter : IActionFilter, IOrderedFilter
    {
        private readonly ResultInternalFilter internalFilter;

        public int Order => 1;

        public ResultMvcFilter(ResultInternalFilter internalFilter)
        {
            this.internalFilter = internalFilter;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult originalObjectResult)
            {
                if (originalObjectResult.Value is IResultBase originalObjectResultValue)
                {
                    ObjectResult returningObjectResult;

                    var handledResult = internalFilter.HandleResult(originalObjectResultValue);

                    if (handledResult.Success)
                    {
                        returningObjectResult = new ObjectResult(handledResult.ResultValue);
                    }
                    else
                    {
                        var problemDetailsHandledResult = (MvcProblemDetails)handledResult.ResultValue;
                        returningObjectResult = new ObjectResult(problemDetailsHandledResult)
                        {
                            StatusCode = problemDetailsHandledResult.Status
                        };
                    }

                    context.Result = returningObjectResult;
                }
            }

        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

    }
}
