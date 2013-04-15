using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class ExpressionMathOperator : IExpression
    {
        public MathOperators Operator { get; private set; }
        public ExpressionMathOperator(MathOperators @operator)
        {
            Operator = @operator;
        }
    }
}
