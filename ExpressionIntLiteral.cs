using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ExpressionIntLiteral : IExpression
    {
        public ExpressionIntLiteral()
        {

        }
        public ExpressionIntLiteral(int value)
        {
            this.Value = value;
        }
        public int Value { get; private set; }
    }
}
