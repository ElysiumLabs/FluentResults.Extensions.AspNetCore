using FluentResults;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FluentProblemDetails.Mvc
{
    internal class ResultErrorProblemDetailsResultFilter : IActionFilter, IOrderedFilter
    {
        public int Order => 1;

        private ResultErrorProblemDetailsFactory Factory { get; }

        private ResultErrorProblemDetailsOptions Options { get; }

        public ResultErrorProblemDetailsResultFilter(
            ResultErrorProblemDetailsFactory factory,
            IOptions<ResultErrorProblemDetailsOptions> options
            )
        {
            Factory = factory;
            Options = options.Value;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is not ObjectResult objectResult)
            {
                return;
            }

            if (objectResult.Value is IResult<object> result)
            {
                if (!result.IsSuccess)
                {
                    var problemDetails = Factory.Create(result.Errors);

                    object returningResult;
                    if (problemDetails.Exception is not null)
                    {
                        returningResult = new ProblemDetailsException(problemDetails, problemDetails.Exception);
                    }
                    else
                    {
                        returningResult = problemDetails;
                    }

                    context.Result = new ObjectResult(returningResult)
                    {
                        StatusCode = problemDetails.Status
                    };
                }
                else
                {
                    context.Result = new ObjectResult(result.Value);
                }
            }


        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }



    }
}
