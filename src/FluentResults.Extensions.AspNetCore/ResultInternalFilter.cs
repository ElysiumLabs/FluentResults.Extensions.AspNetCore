using FluentResults.Extensions.AspNetCore.ProblemDetails;
using FluentResults.Extensions.AspNetCore.Success;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

namespace FluentResults.Extensions.AspNetCore
{
    public class ResultInternalFilter
    {
        protected ResultSuccessValueFactory SuccessValueFactory { get; }

        protected ResultErrorProblemDetailsFactory ErrorFactory { get; }


        public ResultInternalFilter(
            ResultSuccessValueFactory successValueFactory,
            ResultErrorProblemDetailsFactory errorProblemDetailsFactory
            )
        {
            SuccessValueFactory = successValueFactory;
            ErrorFactory = errorProblemDetailsFactory;
        }

        public ResultInternalFilterResult HandleResult(IResultBase result) => 
            result.IsSuccess ? HandleSuccessResult(result) : HandleFailedResult(result);
  

        private ResultInternalFilterResult HandleFailedResult(IResultBase result)
        {
            var problemDetails = ErrorFactory.Create(result.Errors);

            object returningResult;
            if (problemDetails.Exception is not null)
            {
                returningResult = new ProblemDetailsException(problemDetails, problemDetails.Exception);
            }
            else
            {
                returningResult = problemDetails;
            }

            return new ResultInternalFilterResult(false, returningResult);
        }

        private ResultInternalFilterResult HandleSuccessResult(IResultBase result)
        {
            var returningResult = SuccessValueFactory.Create(result);
            return new ResultInternalFilterResult(true, returningResult);
        }

        
    }

    public class ResultInternalFilterResult
    {
        public ResultInternalFilterResult()
        {

        }

        public ResultInternalFilterResult(bool success, object resultValue)
        {
            Success = success;
            ResultValue = resultValue;
        }

        public bool Success { get; set; }

        public object ResultValue { get; set; }
    }
}
