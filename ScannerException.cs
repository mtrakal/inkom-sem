using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ScannerException : Exception
    {
        Logger logger = Logger.GetInstance();
        public ScannerException()
        {
            logger.Log("Neznama chyba ve scanneru!", Logger.Type.ERROR);
        }
        public ScannerException(String message)
            : base(message)
        {
            logger.Log(message, Logger.Type.ERROR);
        }
    }
}
