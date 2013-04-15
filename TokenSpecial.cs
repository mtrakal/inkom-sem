using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class TokenSpecial : TokenBase
    {
        public TokenSpecial(SpecialChars data)
        {
            Type = TokenType.SPECIAL;
            this.Data = data;
            Logger.GetInstance().Log("Special: " + data.ToString());
        }
        public TokenSpecial(MathOperators data)
        {
            Type = TokenType.MATHOPERATOR;
            this.Data = data;
            Logger.GetInstance().Log("MathOperator: " + data.ToString());
        }
    }
}
