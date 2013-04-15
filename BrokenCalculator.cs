using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /// <summary>
    /// http://www.codeproject.com/Articles/88435/Simple-Guide-to-Mathematical-Expression-Parsing
    /// </summary>
    public class BrokenCalculator
    {
        public static void Main(string[] args)
        {
            BrokenCalculator calc = new BrokenCalculator();
            Console.WriteLine(calc.ParseExpr(ref args[0]));
        }

        public int ParseExpr(ref string expr)
        {
            int op, op1;
            op = ParseFactor(ref expr);
            if (expr.Length != 0)
            {
                if (expr[0] == '+')
                {
                    expr = expr.Substring(1, expr.Length - 1);
                    op1 = ParseExpr(ref expr);
                    op += op1;
                }
                else if (expr[0] == '-')
                {
                    expr = expr.Substring(1, expr.Length - 1);
                    op1 = ParseExpr(ref expr);
                    op -= op1;
                }
            }
            return op;
        }
        public int ParseFactor(ref string expr)
        {
            int op, op1;
            op = ParseTerm(ref expr);
            if (expr.Length != 0)
            {
                if (expr[0] == '*')
                {
                    expr = expr.Substring(1, expr.Length - 1);
                    op1 = ParseFactor(ref expr);
                    op *= op1;
                }
                else if (expr[0] == '/')
                {
                    expr = expr.Substring(1, expr.Length - 1);
                    op1 = ParseFactor(ref expr);
                    op /= op1;
                }
            }
            return op;
        }
        public int ParseTerm(ref string expr)
        {
            int returnValue = 0;
            if (expr.Length != 0)
            {
                if (char.IsDigit(expr[0]))
                {
                    returnValue = ParseNumber(ref expr);
                    return returnValue;
                }
                else if (expr[0] == '(')
                {
                    expr = expr.Substring(1, expr.Length - 1);
                    returnValue = ParseExpr(ref expr);
                    expr = expr.Substring(1, expr.Length - 1); //removing closing parenthesis 
                    return returnValue;
                }
                //else if (expr[0] == '(')
                //{
                //    expr = expr.Substring(1, expr.Length - 1);
                //    returnValue = ParseExpr(ref expr);
                //    return returnValue;
                //}
                else if (expr[0] == ')')
                    expr = expr.Substring(1, expr.Length - 1);
            }
            return returnValue;
        }
        public int ParseNumber(ref string expr)
        {
            //string numberTemp = "";
            //while (expr.Length < 0 && char.IsDigit(expr[0]))
            //{
            //    numberTemp += expr[0];
            //    if (expr.Length < 0)
            //    {
            //        expr = expr.Substring(1, expr.Length - 1);
            //    }
            //    else
            //    {
            //        expr = expr.Substring(0, expr.Length - 1);
            //    }
            //}
            //return int.Parse(numberTemp);

            string numberTemp = "";
            for (int i = 0; i < expr.Length && char.IsDigit(expr[i]); i++)
            {
                if (char.IsDigit(expr[0]))
                {
                    numberTemp += expr[0];
                    expr = expr.Substring(1, expr.Length - 1);
                }
            }
            return int.Parse(numberTemp);
        }
    }
}
