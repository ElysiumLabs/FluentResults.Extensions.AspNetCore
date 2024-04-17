using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FluentResults.Extensions.AspNetCore.Mvc
{
    internal class ResultMvcFilterConvention : IActionModelConvention
    {
        private readonly ResultMvcFilterFactory _factory = new();

        public void Apply(ActionModel action)
        {
            action.Filters.Add(_factory);
        }
    }
}
