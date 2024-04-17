using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MvcProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace FluentResults.Extensions.AspNetCore.ProblemDetails
{
    public class ResultErrorProblemDetails : MvcProblemDetails
    {
        // Workarround while problemDetails.WithExceptionDetails() is not open and based only in exception
        [JsonIgnore]
        [IgnoreDataMember]
        public Exception Exception { get; set; }

        [JsonPropertyName("reasons")]
        public List<ResultErrorProblemDetails> Reasons { get; set; }

        public ResultErrorProblemDetails()
        {
        }
    }
}
