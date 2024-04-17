#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MvcProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace FluentResults.Extensions.AspNetCore.Http
{
    public class ResultEndpointFilter : IEndpointFilter
    {
        private readonly ResultInternalFilter internalFilter;

        public ResultEndpointFilter(ResultInternalFilter internalFilter)
        {
            this.internalFilter = internalFilter;
        }

        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var result = await next(context);

            if (result is IResultBase fluentResultBase)
            {
                var handledResult = internalFilter.HandleResult(fluentResultBase);

                if (handledResult.Success)
                {
                    return Results.Ok(handledResult.ResultValue);
                }
                else
                {
                    return Results.Problem((MvcProblemDetails)handledResult.ResultValue);
                }
            }

            return result;
        }
    }
}
#endif