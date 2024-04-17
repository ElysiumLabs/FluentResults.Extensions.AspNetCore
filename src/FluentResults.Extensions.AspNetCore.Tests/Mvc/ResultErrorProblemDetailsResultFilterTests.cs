using System;
using System.Collections.Generic;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using FluentResults.Extensions.AspNetCore.Mvc;
using FluentResults.Extensions.AspNetCore.Options;
using FluentResults.Extensions.AspNetCore.ProblemDetails;
using FluentResults.Extensions.AspNetCore.Success;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentProblemDetails.Tests.Mvc
{
    [TestClass]
    public class ResultErrorProblemDetailsResultFilterTests
    {
        
        [TestMethod]
        public void ActionFilter_ReturnsProblemDetails_WhenActionResultIsUnsuccessfulResult()
        {
            var filter = CreateActionFilter();

            var context = CreateActionContextWithResult(new ObjectResult(Result.Fail("A failed result")));
            
            filter.OnActionExecuted(context);

            var result = context.Result as ObjectResult;

            Assert.IsNotNull(result);

            var problemDetails = result.Value as ProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual("A failed result", problemDetails.Detail);
            Assert.AreEqual(400, problemDetails.Status);
        }
        

        [TestMethod]
        public void ActionFilter_ReturnsResultValue_WhenActionResultIsSuccessfulResult_WithValueTypeValue()
        {
            var filter = CreateActionFilter();

            var resultValue = 123;
            var context = CreateActionContextWithResult(new ObjectResult(Result.Ok(resultValue)));

            filter.OnActionExecuted(context);

            var result = context.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(resultValue, result.Value);
        }

        [TestMethod]
        public void ActionFilter_ReturnsResultValue_WhenActionResultIsSuccessfulResult_WithReferenceTypeValue()
        {
            var filter = CreateActionFilter();

            var resultValue = new Dictionary<string, string>
            {
                {"Key1", "Value1"},
                {"Key2", "Value2"}
            };
            var context = CreateActionContextWithResult(new ObjectResult(Result.Ok(resultValue)));

            filter.OnActionExecuted(context);

            var result = context.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(resultValue, result.Value);
        }
        
        [TestMethod]
        public void ActionFilter_ReturnsNullValue_WhenActionResultIsSuccessfulResultWithoutValue()
        {
            var filter = CreateActionFilter();

            var context = CreateActionContextWithResult(new ObjectResult(Result.Ok()));

            filter.OnActionExecuted(context);

            var result = context.Result as ObjectResult;

            Assert.IsNotNull(result);

            var value = result.Value;
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ActionFilter_ReturnsOriginalValue_WhenActionResultIsNotObjectResult()
        {
            var filter = CreateActionFilter();

            var actionResult = new ContentResult();
            var context = CreateActionContextWithResult(actionResult);

            filter.OnActionExecuted(context);

            Assert.IsNotNull(context.Result);
            Assert.AreEqual(actionResult, context.Result);
        }

        private ResultMvcFilter CreateActionFilter(Action<ResultExtensionsOptions> configure = null)
        {
            var options = new ResultExtensionsOptions();
            configure?.Invoke(options);

            var setup = new ResultExtensionsOptionsSetup();
            setup.Configure(options);

            var optionsHolder = Options.Create(options);

            return new ResultMvcFilter(new ResultInternalFilter(new ResultSuccessValueFactory(optionsHolder), new ResultErrorProblemDetailsFactory(optionsHolder)));
        }


        private ActionExecutedContext CreateActionContextWithResult(IActionResult result)
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var executedContext = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), new object())
                {
                    Result = result
                };

            return executedContext;
        }


    }
}
