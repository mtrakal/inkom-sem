using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class StatementSequence : IStatement
    {
        public IStatement First { get; set; }
        public IStatement Second { get; set; }
    }
}
