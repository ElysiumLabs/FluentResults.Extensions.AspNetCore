using FluentResults;
using FluentResults.Extensions.AspNetCore;
using FluentResults.Extensions.AspNetCore.Http;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using TodoManager.Core.Errors;
using TodoManager.Core.Services;
using TodoManager.Core.Utils;

namespace TodoManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //todomanager sample app items
            builder.Services.AddSingleton<TodoService>();

            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(TodoManagerMarker));
            builder.Services
                .AddProblemDetails(x => ConfigureProblemDetails(x, builder.Environment))
                .AddProblemDetailsConventions()
                .AddFluentResultExtensionsCore(
                    options => 
                    {
                        //should recursive show errors and create sub problemdetails??
                        options.ErrorRecursive = true;

                        options.ErrorGetTypeMap = (error, errorType) =>
                        {
                            return $"https://todomanager.com/errors/{errorType.Name.ToLower()}";
                        };

                        //create your own error and statuscode mappings...
                        options.ErrorTypeToStatusCodeMap.Add(typeof(ConflictError), (int)HttpStatusCode.Conflict);
                        //... or u can go with fallback:A
                        //x.ErrorTypeToStatusCodeMapFallback = (errorType) => { return 418; }; //im teapot

                        //options.SuccessValueMap = (result, value) => new { Success = result.IsSuccess, Value = value };
                    }
                );

            builder.Services.AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

         

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseProblemDetails();
            app.UseRouting();
            app.UseAuthorization();

            var global = app
             .MapGroup(string.Empty)
             .AddEndpointFilter<ResultEndpointFilter>();

            global.MapControllers();

            global.MapGet("/customer/{id}", (int id) =>
            {
                IResultBase result = null;

                if (id == 0)
                {
                    result = new Result().WithError(new ConflictError("Conflito?"));
                }
                else
                {
                    result = new Result<object>().WithValue(new { Id = id });
                }

                return result;
            });

            app.Run();
        }

        //ProblemDetails Defaults
        private static void ConfigureProblemDetails(Hellang.Middleware.ProblemDetails.ProblemDetailsOptions options, IHostEnvironment environment)
        {
            // Only include exception details in a development environment. There's really no need
            // to set this as it's the default behavior. It's just included here for completeness :)
            options.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();

            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
            options.Rethrow<NotSupportedException>();

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

            options.GetTraceId = (ctx) => { return "xxxxxx" + Activity.Current?.Id ?? ctx.TraceIdentifier; };
        }
    }
}
