using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ExpressionMath : IExpression
    {
        public LinkedList<IExpression> MathExpression { get; private set; }
        public ExpressionMath(LinkedList<IExpression> expression)
        {
            MathExpression = expression;
        }
        public override string ToString()
        {
            StringBuilder resultString = new StringBuilder();
            foreach (IExpression item in MathExpression)
            {
                resultString.Append(item.ToString()+" ");
            }
            return resultString.ToString();
        }
    }
}
