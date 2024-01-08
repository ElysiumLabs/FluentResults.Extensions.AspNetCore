using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentResults;
using FluentProblemDetails.Errors;

namespace FluentProblemDetails
{
    public class ResultErrorProblemDetailsOptionsSetup : IConfigureOptions<ResultErrorProblemDetailsOptions>
    {
        public void Configure(ResultErrorProblemDetailsOptions options)
        {
            options.ErrorTypeToStatusCodeMap ??= DefaultErrorTypeToStatusCodeMap();
            options.ErrorTypeToStatusCodeMapFallback ??= DefaultErrorTypeToStatusCodeMapFallback;
            options.GetTypeMap ??= DefaultTypeMap;
            options.GetTitleMap ??= GetTitleByDescription;
            options.GetDetailMap ??= DefaultDetailMap;
            options.GetExtensionsMap ??= DefaultExtensionsMap;
        }

        private Dictionary<Type, int> DefaultErrorTypeToStatusCodeMap()
        {
            return new Dictionary<Type, int>()
            {
                { typeof(MultipleError), (int)HttpStatusCode.BadRequest },
            };
        }

        private string DefaultTypeMap(IError error, Type type)
        {
            return type.Name;
        }

        private static int DefaultErrorTypeToStatusCodeMapFallback(Type errorType)
        {
            return (int)HttpStatusCode.BadRequest;
        }

        private string DefaultDetailMap(IError error, Type type)
        {
            return error.Message;
        }

        private static string GetTitleByDescription(IError error, Type errorType)
        {
            var descriptionAtt = errorType.GetCustomAttributes<DescriptionAttribute>(true).FirstOrDefault();
            return descriptionAtt?.Description ?? null;
        }

        private Dictionary<string, object> DefaultExtensionsMap(IError error, Type type)
        {
            return error.Metadata;
        }
    }
}
