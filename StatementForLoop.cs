using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class StatementForLoop : IStatement
    {
        public String Identificator { get; set; }
        public IExpression From { get; set; }
        public IExpression To { get; set; }
        public IStatement Body { get; set; }
        public override string ToString()
        {
            return "cyklus";
        }
    }
}
