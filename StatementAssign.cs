using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class StatementAssign : IStatement
    {
        public string Identificator { get; set; }
        public IExpression Expression { get; set; }
    }
}
