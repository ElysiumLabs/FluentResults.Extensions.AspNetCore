using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager.Core.Commands
{
    public class UpsertTodoCommand
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public bool Completed { get; set; }
    }
}
