using FluentResults.Extensions.AspNetCore.Options;
using FluentResults.Extensions.AspNetCore.ProblemDetails;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentResults.Extensions.AspNetCore.Success
{
    public class ResultSuccessValueFactory
    {
        private readonly ResultExtensionsOptions options;

        public ResultSuccessValueFactory(IOptions<ResultExtensionsOptions> options)
        {
            this.options = options.Value;
        }

        public object Create(IResultBase result)
        {
            var extractedValue = ExtractValueFromResult(result);
            return options.SuccessValueMap?.Invoke(result, extractedValue) ??
                extractedValue;
        }

        public static object ExtractValueFromResult(IResultBase result)
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
