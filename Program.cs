using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ConsoleApplication1
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/cc136756.aspx
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Logger logger = Logger.GetInstance();

            if (args.Count() == 1)
            {
                if (args[0].ToString() == "-GUI")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
                else
                {
                    String FileName;
                    try
                    {
                        logger.Log(args[0].ToString());
                        FileName = args[0];
                        TextReader tr = new StreamReader(FileName);
                        Scanner scanner = new Scanner(tr);

                        Parser parser = new Parser(scanner.Tokens);
                        tr.Close();
                        Compiler compiler = new Compiler(parser.Statement, Path.GetFileNameWithoutExtension(args[0]) + ".exe");


                        //Console.ReadLine();
                    }
                    catch (Exception e)
                    {
                        logger.Log(e.ToString(), Logger.Type.ERROR, e.Message);
                        while (e.InnerException != null)
                        {
                            e = e.InnerException;
                            logger.Log(e.ToString(), Logger.Type.ERROR, e.Message);
                        }

                        //Console.ReadLine();
                    }
                }
            }
            else
            {
                logger.Log("Chybi druhy parametr.\r\nPouziti: \"" + System.AppDomain.CurrentDomain.FriendlyName + " zdrojak.mt\"" +"\r\nZapnuti GUI: \"" + System.AppDomain.CurrentDomain.FriendlyName + " -GUI\"",Logger.Type.ERROR);
                //Console.ReadLine();
                return;
            }
        }
    }
}
