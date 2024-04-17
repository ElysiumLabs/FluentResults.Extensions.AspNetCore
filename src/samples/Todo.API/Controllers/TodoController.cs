using FluentResults;
using FluentResults.Extensions.AspNetCore.ProblemDetails;
using FluentResults.Extensions.AspNetCore.Success;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TodoManager.Core.Commands;
using TodoManager.Core.Services;

namespace TodoManager.API.Controllers
{
    [ApiController]
    [Route("todo")]
    public class TodoController : TodoControllerBase
    {

        private readonly ILogger<TodoController> _logger;
        private readonly TodoService todoService;

        public TodoController(
            ILogger<TodoController> logger,
            TodoService todoService
            )
        {
            _logger = logger;
            this.todoService = todoService;
        }

        [HttpPost("create1")]
        public async Task<ActionResult<Core.Models.Todo>> Create1(
            [FromBody] UpsertTodoCommand command
            )
        {
            return Ok(await todoService.Create(command));
        }

        [HttpPost("create2")]
        public async Task<ActionResult<Core.Models.Todo>> Create2(
            [FromBody] UpsertTodoCommand command
            )
        {
            var r = await todoService.Create(command);
            if (r.IsSuccess)
            {
                return Ok(r.Value);
            }
            else
            {
                return BadRequest(r);
            }
        }

        [HttpPost("create3")]
        public async Task<ActionResult<Core.Models.Todo>> Create2(
            [FromServices] ResultErrorProblemDetailsFactory errorProblemDetailsFactory,
            [FromBody] UpsertTodoCommand command
            )
        {
            //if u dont want to use filters, but control the way out
            var r = await todoService.Create(command);
            return HandleResult(r);
        }

        [HttpPost("update")]
        public async Task<ActionResult<Core.Models.Todo>> Update(
            [FromBody] UpsertTodoCommand command
            )
        {
            throw new NotImplementedException();
        }
    }

    public class TodoControllerBase : ControllerBase
    {
        protected ResultErrorProblemDetailsFactory FluentProblemDetailsFactory => 
            HttpContext.RequestServices.GetRequiredService<ResultErrorProblemDetailsFactory>();

        protected ResultSuccessValueFactory FluentSuccessValueFactory =>
            HttpContext.RequestServices.GetRequiredService<ResultSuccessValueFactory>();

        protected ObjectResult HandleResult(IResultBase result)
        {
            if (!result.IsSuccess)
            {
                var pd = FluentProblemDetailsFactory.Create(result.Errors);
                return new ObjectResult(pd) { StatusCode = pd.Status };
            }
            else
            {
                return new ObjectResult(FluentSuccessValueFactory.Create(result));
            }
        }
    }


}
