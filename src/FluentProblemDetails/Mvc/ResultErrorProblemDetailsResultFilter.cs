using System;
using System.Linq;
using System.Reflection;
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

        #region Implementation of IActionFilter
        
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is not ObjectResult objectResult)
            {
                return;
            }

            if (objectResult.Value is IResultBase result)
            {
                context.Result = HandleResult(result);
            }
        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        #endregion  



        /// <summary>
        /// Handles the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private IActionResult HandleResult(IResultBase result)
        {
            if (!result.IsSuccess)
            {
                return HandleFailedResult(result);
            }

            // Extract out the result value and return it - this avoids any superfluous IResultBase members from being serialised to the response output.
            var successValue = ExtractValueFromResult(result);

            return new ObjectResult(successValue ?? null);
        }
        

        private IActionResult HandleFailedResult(IResultBase result)
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

            return new ObjectResult(returningResult)
            {
                StatusCode = problemDetails.Status
            };
        }


        private object ExtractValueFromResult(IResultBase result)
        {
            // If the result value is a reference type then covariance will let us access it via IResult<object>
            if (result is IResult<object> valueResult)
            {
                return valueResult.Value;
            }

            // But what if the result value is a value type (eg: int or enum)
            var resultType = result.GetType();
            var genericResultInterface = resultType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResult<>));

            if (genericResultInterface != null)
            {
                var valueProperty = genericResultInterface.GetProperty(nameof(IResult<object>.Value), BindingFlags.Instance | BindingFlags.Public);

                if (valueProperty == null)
                    throw new Exception($"Failed to find Value property on type {resultType}.");

                var value = valueProperty.GetValue(result);

                return value;
            }
            
            return null;
        }




    }
}
