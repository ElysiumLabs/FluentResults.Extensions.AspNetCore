using FluentResults;
using System.Collections.Generic;
using System.ComponentModel;

namespace FluentResults.Extensions.AspNetCore.Errors
{
    [Description("Multiple errors")]
    internal class MultipleError : Error
    {
        public MultipleError() : base()
        {

        }

        public MultipleError(string message) : base(message)
        {

        }

        public MultipleError(IEnumerable<IError> errors) : this()
        {
            foreach (var error in errors)
            {
                CausedBy(error);
            }
        }
    }
}
