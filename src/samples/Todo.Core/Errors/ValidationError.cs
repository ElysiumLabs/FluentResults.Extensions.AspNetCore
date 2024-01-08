using FluentResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager.Core.Errors
{
    [Description("Validation errors")]
    public class ValidationError : Error
    {
        public ValidationError() : base()
        {

        }

        public ValidationError(string message) : base(message)
        {

        }

        public ValidationError(IEnumerable<IError> errors) : this()
        {
            foreach (var error in errors)
            {
                CausedBy(error);
            }
        }
    }

    [Description("Item validation error")]
    public class ValidationItemError : Error
    {
        public ValidationItemError() : base()
        {

        }

        public ValidationItemError(string message) : base(message)
        {

        }
    }
}
