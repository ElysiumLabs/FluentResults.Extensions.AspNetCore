using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FluentResults.Extensions.AspNetCore.Mvc
{
    internal class ResultMvcFilterFactory : IFilterFactory, IOrderedFilter
    {
        public bool IsReusable => true;

        /// <summary>
        /// The same order as the built-in ClientErrorResultFilterFactory.
        /// One after HellangProblemDetails
        /// </summary>
        public int Order => -2001;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<ResultMvcFilter>(serviceProvider);
        }
    }
}
