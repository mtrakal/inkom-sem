using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ExpressionStringLiteral : IExpression
    {
        public ExpressionStringLiteral()
        {

        }
        public ExpressionStringLiteral(String value)
        {
            this.Value = value;
        }
        public String Value { get; set; }
        public override string ToString()
        {
            return Value;
        }
    }
}
