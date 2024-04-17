using FluentResults.Extensions.AspNetCore.Options;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentResults.Extensions.AspNetCore.Mvc
{
    internal class ResultApplicationModelProvider : IApplicationModelProvider
    {
        private ResultExtensionsOptions Options { get; }
        private List<IActionModelConvention> ActionModelConventions { get; }

        public ResultApplicationModelProvider(IOptions<ResultExtensionsOptions> options)
        {
            Options = options.Value;
            ActionModelConventions = new List<IActionModelConvention>
            {
                new ResultMvcFilterConvention()
            };
        }

        public int Order => -1000 + 200;

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            if (!Options.UseDefaultActionModelConventions)
            {
                return;
            }

            foreach (var controller in context.Result.Controllers)
            {
                if (!IsApiController(controller))
                {
                    continue;
                }

                foreach (var action in controller.Actions)
                {
                    foreach (var convention in ActionModelConventions)
                    {
                        convention.Apply(action);
                    }
                }
            }
        }

        private static bool IsApiController(ControllerModel controller)
        {
            if (controller.Attributes.OfType<IApiBehaviorMetadata>().Any())
            {
                return true;
            }

            var assembly = controller.ControllerType.Assembly;
            var attributes = assembly.GetCustomAttributes();

            return attributes.OfType<IApiBehaviorMetadata>().Any();
        }

        void IApplicationModelProvider.OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            // Not needed.
        }
    }
}
