using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class TokenNumber : TokenBase
    {
        public TokenNumber(int data)
        {
            Type = TokenType.NUMBER;
            Data = data;
        }
    }
}
