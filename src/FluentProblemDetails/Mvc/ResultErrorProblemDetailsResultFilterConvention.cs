using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FluentProblemDetails.Mvc
{
    internal class ResultErrorProblemDetailsResultFilterConvention : IActionModelConvention
    {
        private readonly ResultErrorProblemDetailsResultFilterFactory _factory = new();

        public void Apply(ActionModel action)
        {
            action.Filters.Add(_factory);
        }
    }
}
