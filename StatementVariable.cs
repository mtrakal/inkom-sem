using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class StatementVariable : IStatement
    {
        public string Identificator { get; set; }
        public IExpression Expression { get; set; }
        public override string ToString()
        {
            return "promenna '" + Identificator.ToString() + "' s hodnotou '" + Expression.ToString() + "'";
        }
    }
}
