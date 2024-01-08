img src="https://github.com/ElysiumLabs/FluentProblemDetails/blob/master/resources/icons/FluentResults-Icon-64.png" alt="FluentResults"/>

# FluentResults

[![Nuget downloads](https://img.shields.io/nuget/v/fluentproblemdetails.svg)](https://www.nuget.org/packages/FluentProblemDetails/)
[![Nuget](https://img.shields.io/nuget/dt/fluentproblemdetails)](https://www.nuget.org/packages/FluentProblemDetails/)

**FluentProblemDetails is a lightweight .NET library developed to resolve the gap  between results in domain and problemdetails in presentation. **

You can install [FluentProblemDetails with NuGet](https://www.nuget.org/packages/FluentProblemDetails/):

```
Install-Package FluentProblemDetails
```

## Key Features

- *****

```csharp
builder.Services.AddAutoMapper(typeof(TodoManagerMarker));
builder.Services
    .AddProblemDetails(x => ConfigureProblemDetails(x, builder.Environment))
    .AddProblemDetailsConventions()
    .AddResultProblemDetails(x => 
    {
        //should recursive show errors and create sub problemdetails??
        x.Recursive = true;

        x.GetTypeMap = (error, errorType) =>
        {
            return $"https://todomanager.com/errors/{errorType.Name.ToLower()}";
        };

        //create your own error and statuscode mappings...
        x.ErrorTypeToStatusCodeMap.Add(typeof(ConflictError), (int)HttpStatusCode.Conflict);
        //... or u can go with fallback:A
        //x.ErrorTypeToStatusCodeMapFallback = (errorType) => { return 418; }; //im teapot
    }
    );
```

```csharp
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
```

```csharp
[HttpPost("create")]
public async Task<ActionResult<Core.Models.Todo>> Create(
    [FromBody] UpsertTodoCommand command
    ) 
{
    return Ok(await todoService.Create(command));
}
```

```csharp
public class MyCustomError : Error
{
    public ConflictError(string message) : base(message)
    {
    }

    public MyCustomError(string message, IError causedBy) : base(message, causedBy)
    {
    }

    protected MyCustomError()
    {
    }
}
```


```json
{
  "reasons": [
    {
      "type": "https://todomanager.com/errors/validationitemerror",
      "title": "Item validation error",
      "detail": "Title is empty"
    },
    {
      "type": "https://todomanager.com/errors/validationitemerror",
      "title": "Item validation error",
      "detail": "Descripton is empty"
    }
  ],
  "type": "https://todomanager.com/errors/validationerror",
  "title": "Validation errors",
  "status": 400,
  "traceId": "xxxxxx00-c18822dc6345975d18efead835a471ba-c8d4fb0ecff51861-00"
}
```
