using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentProblemDetails.Mvc
{
    internal class ResultErrorProblemDetailsApplicationModelProvider : IApplicationModelProvider
    {
        private ResultErrorProblemDetailsOptions Options { get; }
        private List<IActionModelConvention> ActionModelConventions { get; }

        public ResultErrorProblemDetailsApplicationModelProvider(IOptions<ResultErrorProblemDetailsOptions> options)
        {
            Options = options.Value;
            ActionModelConventions = new List<IActionModelConvention>
            {
                new ResultErrorProblemDetailsResultFilterConvention()
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
