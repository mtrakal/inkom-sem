using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class CompilerException : Exception
    {
        Logger logger = Logger.GetInstance();
        public CompilerException()
        {
            logger.Log("Neznama chyba ve scanneru!", Logger.Type.ERROR);
        }
        public CompilerException(String message)
            : base(message)
        {
            logger.Log(message, Logger.Type.ERROR);
        }
    }
}
