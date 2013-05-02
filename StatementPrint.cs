using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class StatementPrint : IStatement
    {
        public IExpression Expression { get; set; }
        public override string ToString()
        {
            return "vypis promenne, nebo textu.";
        }
    }
}
