using FluentProblemDetails.Mvc;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace FluentProblemDetails
{
    public static class ResultErrorProblemDetailsServiceCollectionExtensions
    {
        public static IServiceCollection AddResultProblemDetails(
            this IServiceCollection services,
            Action<ResultErrorProblemDetailsOptions> configure = null
            )
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.TryAddSingleton<ResultErrorProblemDetailsFactory>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ResultErrorProblemDetailsOptions>, ResultErrorProblemDetailsOptionsSetup>());
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ResultErrorProblemDetailsApplicationModelProvider>());

            return services;
        }
    }
}
