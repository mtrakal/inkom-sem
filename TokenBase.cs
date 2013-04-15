using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class TokenBase : IToken
    {
        TokenType type = new TokenType();
        public TokenType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public object Data { get; set; }
    }
}