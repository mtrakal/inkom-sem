using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ExpressionVariable : IExpression
    {
        public ExpressionVariable()
        {

        }
        public ExpressionVariable(String identificator)
        {
            this.Identificator = identificator;
        }
        public String Identificator { get; set; }
    }
}
