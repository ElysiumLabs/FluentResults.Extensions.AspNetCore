using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TodoManager.Core.Commands;
using TodoManager.Core.Services;

namespace TodoManager.API.Controllers
{
    [ApiController]
    [Route("todo")]
    public class TodoController : ControllerBase
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

        [HttpPost("create")]
        public async Task<ActionResult<Core.Models.Todo>> Create(
            [FromBody] UpsertTodoCommand command
            ) 
        {
            return Ok(await todoService.Create(command));
        }

        [HttpPost("update")]
        public async Task<ActionResult<Core.Models.Todo>> Update(
            [FromBody] UpsertTodoCommand command
            )
        {
            throw new NotImplementedException();
        }
    }
}
