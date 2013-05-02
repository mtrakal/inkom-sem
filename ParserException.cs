using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class ParserException : Exception
    {
        Logger logger = Logger.GetInstance();

        public ParserException()
        {
            logger.Log("Neznama chyba v parseru!", Logger.Type.ERROR);
        }
        public ParserException(String message)
            : base(message)
        {
            logger.Log(message, Logger.Type.ERROR);
        }
    }
}
