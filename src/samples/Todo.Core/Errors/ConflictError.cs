using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager.Core.Errors
{
    public class ConflictError : Error
    {
        public ConflictError(string message) : base(message)
        {
        }

        public ConflictError(string message, IError causedBy) : base(message, causedBy)
        {
        }

        protected ConflictError()
        {
        }
    }
}
