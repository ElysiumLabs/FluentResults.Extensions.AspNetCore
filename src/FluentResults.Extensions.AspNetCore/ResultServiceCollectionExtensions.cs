using FluentResults.Extensions.AspNetCore.Mvc;
using FluentResults.Extensions.AspNetCore.Options;
using FluentResults.Extensions.AspNetCore.ProblemDetails;
using FluentResults.Extensions.AspNetCore.Success;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace FluentResults.Extensions.AspNetCore
{
    public static class ResultServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentResultExtensionsCore(
            this IServiceCollection services,
            Action<ResultExtensionsOptions> configure = null
            )
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.TryAddSingleton<ResultInternalFilter>();
            services.TryAddSingleton<ResultErrorProblemDetailsFactory>();
            services.TryAddSingleton<ResultSuccessValueFactory>();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ResultExtensionsOptions>, ResultExtensionsOptionsSetup>());

            return services;
        }

        public static IServiceCollection AddFluentResultExtensions(
            this IServiceCollection services,
            Action<ResultExtensionsOptions> configure = null
            )
        {
            services.AddFluentResultExtensionsCore(configure);

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ResultApplicationModelProvider>());

            return services;
        }
    }
}
