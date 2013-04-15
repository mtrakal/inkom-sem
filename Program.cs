using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// http://msdn.microsoft.com/en-us/magazine/cc136756.aspx
namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            String FileName;
            Logger logger = Logger.GetInstance();

            //try
            //{

            if (args.Count() != 1)
            {
                logger.Log("Chybi druhy parametr.\r\nPouziti: \"" + System.AppDomain.CurrentDomain.FriendlyName + " zdrojak.mt\"", Logger.Type.ERROR);
                //Console.ReadLine();
                return;
            }

            logger.Log(args[0].ToString());
            FileName = args[0];
            TextReader tr = new StreamReader(FileName);
            Scanner scanner = new Scanner(tr);

            Parser parser = new Parser(scanner.Tokens);
            tr.Close();
            Compiler compiler = new Compiler(parser.Statement, Path.GetFileNameWithoutExtension(args[0]) + ".exe");


            //Console.ReadLine();
            //}
            //catch (Exception e)
            //{
            //    logger.Log(e.ToString(), Logger.Type.ERROR, e.Message);
            //    while (e.InnerException != null)
            //    {
            //        e = e.InnerException;
            //        logger.Log(e.ToString(), Logger.Type.ERROR, e.Message);
            //    }

            //    //Console.ReadLine();
            //}
        }
    }
}
