using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentResults.Extensions.AspNetCore.Errors;

namespace FluentResults.Extensions.AspNetCore.Options
{
    public class ResultExtensionsOptionsSetup : IConfigureOptions<ResultExtensionsOptions>
    {
        public void Configure(ResultExtensionsOptions options)
        {
            options.ErrorTypeToStatusCodeMap ??= DefaultErrorTypeToStatusCodeMap();
            options.ErrorTypeToStatusCodeMapFallback ??= DefaultErrorTypeToStatusCodeMapFallback;
            options.ErrorGetTypeMap ??= DefaultTypeMap;
            options.ErrorGetTitleMap ??= GetTitleByDescription;
            options.ErrorGetDetailMap ??= DefaultDetailMap;
            options.ErrorGetExtensionsMap ??= DefaultExtensionsMap;
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
