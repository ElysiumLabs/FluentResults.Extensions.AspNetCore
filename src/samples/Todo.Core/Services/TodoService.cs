using AutoMapper;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoManager.Core.Commands;
using TodoManager.Core.Entities;
using TodoManager.Core.Errors;

namespace TodoManager.Core.Services
{
    public class TodoService
    {
        private List<Todo> _todoList = new List<Todo>();

        private readonly IMapper mapper;

        public TodoService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<Result<Models.Todo>> Create(UpsertTodoCommand command)
        {
            var result = new Result<Models.Todo>();

			try
			{
                var existingTodo = _todoList.FirstOrDefault(x => x.Id == command.Id);
                if (existingTodo is not null)
                {
                    var conflictError = new ConflictError("This todo item id already exists");
                    conflictError.WithMetadata("existingTodoItem", mapper.Map<Models.Todo>(existingTodo));
                    return result.WithError(conflictError);
                }

                var todo = new Todo()
                {
                    Id = command.Id,
                    Title = command.Title,
                    Description = command.Description,
                    Completed = command.Completed,
                };

                var validationResult = Validate(todo);

                if (validationResult.IsFailed)
                {
                    return result.WithError(new ValidationError(validationResult.Errors));
                }

                _todoList.Add(todo);

                result.WithValue(mapper.Map<Models.Todo>(todo));
            }
			catch (Exception ex)
			{
                result.WithError(new ExceptionalError("Create Error", ex));
            }

            return result;
        }

        private Result Validate(Todo todo)
        {
            Result result = new Result();

            if (string.IsNullOrEmpty(todo.Title))
            {
                result.WithError(new ValidationItemError("Title is empty"));
            }

            if (string.IsNullOrEmpty(todo.Description))
            {
                result.WithError(new ValidationItemError("Descripton is empty"));
            }

            return result;
        }
    }
}
