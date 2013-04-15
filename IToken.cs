using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    interface IToken
    {
        TokenType Type { get; set; }
        object Data { get; set; }
    }
}
