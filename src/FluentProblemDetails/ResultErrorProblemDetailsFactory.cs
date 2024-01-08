using FluentProblemDetails.Errors;
using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;

namespace FluentProblemDetails
{
    public class ResultErrorProblemDetailsFactory
    {
        private readonly ResultErrorProblemDetailsOptions options;

        public ResultErrorProblemDetailsFactory(IOptions<ResultErrorProblemDetailsOptions> options)
        {
            this.options = options.Value;
        }

        public ResultErrorProblemDetails Create(IEnumerable<IError> errors)
        {
            if (!errors.Any())
            {
                throw new Exception("No errors in errors");
            }

            if (errors.Count() > 1)
            {
                var multipleError = new MultipleError(errors);
                return Create(new IError[] { multipleError }.AsEnumerable());
            }

            return CreateInternal(errors.Single(), true);
        }

        protected ResultErrorProblemDetails CreateInternal(IError error, bool mapStatusCode)
        {
            var errorType = error.GetType();

            var problemDetails = new ResultErrorProblemDetails
            {
                Type = options.GetTypeMap?.Invoke(error, errorType),
                Title = options.GetTitleMap?.Invoke(error, errorType),
                Detail = error.Message
            };

            var extensions = options.GetExtensionsMap?.Invoke(error, errorType);
            if (extensions is not null)
            {
                foreach (var extension in extensions)
                {
                    problemDetails.Extensions.Add(extension.Key, extension.Value);
                }
            }

            // Workarround while problemDetails.WithExceptionDetails() is not open and based only in exception
            if (error is ExceptionalError exceptionalError)
            {
                problemDetails.Exception = exceptionalError.Exception;
            }

            if (errorType == typeof(MultipleError) || options.Recursive)
            {
                var errors = error.Reasons.OfType<IError>();
                problemDetails.Reasons = errors.Any() ? errors.Select(x => CreateInternal(x, false)).ToList() : null;
            }

            if (mapStatusCode)
            {
                if (options.ErrorTypeToStatusCodeMap.TryGetValue(errorType, out var statusCode))
                {
                    problemDetails.Status = statusCode;
                }
                else
                {
                    problemDetails.Status = options.ErrorTypeToStatusCodeMapFallback(errorType);
                }

                if (problemDetails.Status is not null && string.IsNullOrEmpty(problemDetails.Title))
                {
                    problemDetails.Title = ReasonPhrases.GetReasonPhrase(problemDetails.Status.Value);
                }
            }

            return problemDetails;
        }
    }
}
