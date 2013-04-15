using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class TokenWord : TokenBase
    {
        public TokenWord(String data)
        {
            this.Type = TokenType.WORD;
            this.Data = data;
        }
        public TokenWord(String data, TokenType type)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
