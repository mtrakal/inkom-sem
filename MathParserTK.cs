namespace ConsoleApplication1
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	#region License agreement

	//MathParserTK — mathematical parser designed as a light, fast and simple to understand
	//module (class), which receives as input a mathematical expression (string) and 
	//return the output value (double). 
	//Copyright (C) 2012 Yerzhan Kalzhani , kirnbas@gmail.com

	//This program is free software: you can redistribute it and/or modify
	//it under the terms of the GNU General Public License as published by
	//the Free Software Foundation, either version 3 of the License, or
	//(at your option) any later version.

	//This program is distributed in the hope that it will be useful,
	//but WITHOUT ANY WARRANTY; without even the implied warranty of
	//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	//GNU General Public License for more details.

	//You should have received a copy of the GNU General Public License
	//along with this program.  If not, see <http://www.gnu.org/licenses/> 

	#endregion

	#region Example usage:

	//public static void Main()
	//{     
	//        MathParserTK parser = new MathParserTK();
	//        string s = "sin(90)+1*3";
	//        bool isRadians = false; 
	//        double d = parser.Parse(s, isRadians);    
	//}   

	#endregion

	/// <summary>
	/// Mathematical parser
    /// http://www.codeproject.com/Tips/381509/Math-Parser-NET-Csharp
	/// </summary>
	public class MathParser
	{
		#region Instance fields

		private Dictionary<string, string> operators;
		private Dictionary<string, string> constants;
		private readonly char decimalSeparator;
		private bool isRadians;
		private char numberMarker = '#';
		private char operatorMarker = '$';

		#region Operators

		private const string plus = "plus";
		private const string minus = "minus";
		private const string unPlus = "unPlus";
		private const string unMinus = "unMinus";
		private const string multiply = "multiply";
		private const string division = "division";
		private const string leftParent = "leftParnt";
		private const string rightParent = "rightParnt";
		private const string degree = "degree";
		private const string sqrt = "sqrt";
		private const string sqrt2 = "√";
		private const string sin = "sin";
		private const string cos = "cos";
		private const string tg = "tg";
		private const string ctg = "ctg";
		private const string sh = "sh";
		private const string ch = "ch";
		private const string th = "th";
		private const string log = "log";
		private const string ln = "ln";
		private const string exp = "exp";
		private const string abs = "abs";


		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// The class constructor initializes the dictionary and reads the decimal separator from the regional settings in OS
		/// </summary>
		public MathParser()
		{
			operators = new Dictionary<string, string>(50);
			constants = new Dictionary<string, string>(10);

			//dictionary supported math. operators and constants (left -- token, right -- input value)
			/*--------------------------------------------------------Operators------------------------------------------------*/
			operators.Add(plus, "+");
			operators.Add(minus, "-");
			operators.Add(unPlus, "+");
			operators.Add(unMinus, "-");
			operators.Add(multiply, "*");
			operators.Add(division, "/");
			operators.Add(leftParent, "(");
			operators.Add(rightParent, ")");
			operators.Add(degree, "^");
			operators.Add(sqrt2, "√");
			operators.Add(sqrt, "sqrt");
			operators.Add(sin, "sin");
			operators.Add(cos, "cos");
			operators.Add(tg, "tg");
			operators.Add(ctg, "ctg");
			operators.Add(sh, "sh");
			operators.Add(ch, "ch");
			operators.Add(th, "th");
			operators.Add(log, "log");
			operators.Add(ln, "ln");
			operators.Add(exp, "exp");
			operators.Add(abs, "abs");
			/*-----------------------------------------------------------------------------------------------------------------*/
			/*--------------------------------------------------------Constants------------------------------------------------*/
			constants.Add(Math.E.ToString(), "e");
			constants.Add(Math.PI.ToString(), "pi");
			/*-----------------------------------------------------------------------------------------------------------------*/
			/*
			* If you want to add new operator, then add new record in operators and edit methods such as
			* getPrioritet, getNumberArguments, isFunction, calculating.
			* if you want to add new constants, then add new record in constants and check the compatibility with other
			* operators in method lexicalAnalyzator in convert expression to reverse polish notation.
			*/

			try
			{
				decimalSeparator = Char.Parse(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			}
			catch (FormatException)
			{
				throw new FormatException("Error: can't read char decimal separator from system, check your regional settings.");
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Parse mathematical expression
		/// </summary>
		/// <param name="s">Input math expression in infix notation</param>
		/// <param name="isRadians">Flag defines the argument of trigonometric functions is in radians, otherwise it's in degrees</param>
		/// <returns>Return the calculated value of the input expression</returns>
		public double Parse(string s, bool isRadians)
		{
			double result = 0;
			this.isRadians = isRadians;

			if (String.IsNullOrEmpty(s) || !CheckParent(s))
			{
				throw new FormatException();
			}

			// Convert all symbols in input string to lower case and remove whitespaces
			s = s.ToLower().Replace(" ", "");

			try
			{
				s = ConvertToRPN(s);
				result = Calculate(s);
			}
			catch (DivideByZeroException)
			{
				throw;
			}
			catch (FormatException)
			{
				throw;
			}
			catch (InvalidOperationException)
			{
				throw;
			}
			catch (ArgumentOutOfRangeException)
			{
				throw;
			}
			catch (ArgumentException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}

			return result;
		}

		/// <summary>
		/// Checks the number of parenthesis in expression
		/// </summary>
		/// <param name="s">Input expression</param>
		/// <returns>Returns true if the number of opening and closing parentheses correspond to each other</returns>       
		private bool CheckParent(string s)
		{
			int count = 0;

			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '(')
				{
					count++;
				}
				else if (s[i] == ')')
				{
					count--;
				}
			}

			return count == 0;
		}

		/// <summary>
		/// Returns priority of operator
		/// </summary>
		/// <param name="op">Operator</param>
		private int GetPrioritet(string op)
		{
			switch (op)
			{
				case leftParent:
					return 0;
				case rightParent:
					return 1;
				case plus:
				case minus:
					return 2;
				case unPlus:
				case unMinus:
					return 6;
				case multiply:
				case division:
					return 4;
				case degree:
				case sqrt:
				case sqrt2:
					return 8;
				case sin:
				case cos:
				case tg:
				case ctg:
				case sh:
				case ch:
				case th:
				case log:
				case ln:
				case exp:
				case abs:
					return 10;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Returns true if input string is function
		/// </summary>
		/// <param name="s">Token</param>
		private bool IsFunction(string s)
		{
			switch (s)
			{
				case sqrt:
				case sqrt2:
				case sin:
				case cos:
				case tg:
				case ctg:
				case sh:
				case ch:
				case th:
				case log:
				case ln:
				case exp:
				case abs:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Returns the number of operator arguments
		/// </summary>
		/// <param name="op">Operator</param>
		private int GetNumberArguments(string op)
		{
			switch (op)
			{
				case plus:
				case minus:
				case multiply:
				case division:
				case degree:
				case log:
					return 2;
				case unMinus:
				case unPlus:
				case sqrt:
				case sqrt2:
				case tg:
				case sh:
				case ch:
				case th:
				case ln:
				case ctg:
				case sin:
				case cos:
				case exp:
				case abs:
					return 1;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Converts input expression to Reverse Polish Notation
		/// </summary>
		/// <param name="inputS">Expression in infix notation</param>
		/// <returns>Returns expression in postfix notation</returns>
		private string ConvertToRPN(string inputS)
		{
			int pos = 0; // Position on input string
			string token = string.Empty;
			Stack<string> stack = new Stack<string>();
			StringBuilder outputS = new StringBuilder();

			// Parse infix expression
			while (pos < inputS.Length)
			{
				token = LexicalAnalyzeInfixExp(inputS, ref pos, token); // Lexical analyze
								
				// Adds received token to output string
				if (operators.ContainsKey(token))
				{
					if (token == rightParent)
					{
						while (stack.Peek() != leftParent)
						{
							outputS.Append(operatorMarker + stack.Pop());							
						}
						stack.Pop();
					}
					else if (stack.Count == 0 || token == leftParent)
					{
						stack.Push(token);
					}
					else if (GetPrioritet(token) <= GetPrioritet(stack.Peek()))
					{
						outputS.Append(operatorMarker + stack.Pop());
						while (stack.Count > 0 && GetPrioritet(token) <= GetPrioritet(stack.Peek()))
						{
							outputS.Append(operatorMarker + stack.Pop());							
						}
						stack.Push(token);
					}
					else
					{
						stack.Push(token);
					}
				}
				else
				{
					outputS.Append(numberMarker + token);					
				}
			}

			// Pushing out the remainder of the stack to the output string
			if (stack.Count > 0)
			{
				foreach (string s in stack)
				{
					outputS.Append(operatorMarker + s);
				}
			}

			return outputS.ToString();
		}

		/// <summary>
		/// Lexical analyzer for math expression in infix notation
		/// </summary>
		/// <param name="inputS">Expression in infix notation</param>
		/// <param name="pos">Current position in the expressions</param>
		/// <param name="preToken">Previous token</param>
		/// <returns>token</returns>                
		private string LexicalAnalyzeInfixExp(string inputS, ref int pos, string preToken)
		{
			StringBuilder token = new StringBuilder();
			bool isUnary = false; // Flag if operator unary

			if (pos == 0 || preToken == leftParent)
			{
				isUnary = true;
			}
						
			if (Char.IsLetter(inputS[pos]))	// Operator or constant
			{
				for (; pos < inputS.Length && Char.IsLetter(inputS[pos]); pos++)
				{
					token.Append(inputS[pos]);
				}

				if (operators.ContainsValue(token.ToString()))
				{					
					string s = GetKeyInOperators(token.ToString());

					if (pos < inputS.Length && inputS[pos] != '(' && IsFunction(s))
					{
						throw new ArgumentException("Function must be with parenthesis");
					}

					return s; // return operator
				}
				else if (constants.ContainsValue(token.ToString()))
				{
					return GetKeyInConstants(token.ToString()); // return constant value
				}
				else
				{
					throw new FormatException("Unknown operator or constant: " + token);
				}
			}
			else if (!Char.IsDigit(inputS[pos]) && inputS[pos] != decimalSeparator)	// Operator
			{
				if (operators.ContainsValue(inputS[pos].ToString()))
				{
					string s = GetKeyInOperators(inputS[pos++].ToString());

					if (isUnary)
					{
						switch (s)
						{
							case minus: return unMinus;
							case plus: return unPlus;
						}
					}
					else if (pos < inputS.Length && inputS[pos] != '(' && IsFunction(s))
					{
						throw new ArgumentException("Function must be with parenthesis");
					}

					return s; // return operator
				}
				else
					throw new FormatException("Unknown operator: " + inputS[pos]);				
			}
			else // Number
			{
				if (Char.IsDigit(inputS[pos]))
				{
					for (; pos < inputS.Length && Char.IsDigit(inputS[pos]); pos++)
					{
						token.Append(inputS[pos]);
					}
				}

				if (pos < inputS.Length && inputS[pos] == decimalSeparator)
				{
					token.Append(inputS[pos++]);

					for (; pos < inputS.Length && Char.IsDigit(inputS[pos]); pos++)
					{
						token.Append(inputS[pos]);
					}
				}

				// Read scientific notation
				if (pos + 1 < inputS.Length && inputS[pos] == 'e' 
					&& (Char.IsDigit(inputS[pos + 1]) 
						|| (pos + 2 < inputS.Length && (inputS[pos + 1] == '+' || inputS[pos + 1] == '-') && Char.IsDigit(inputS[pos + 2])))) 
				{
					token.Append(inputS[pos++]); // e

					if (inputS[pos] == '+' || inputS[pos] == '-')
						token.Append(inputS[pos++]); // sign

					for (; pos < inputS.Length && Char.IsDigit(inputS[pos]); pos++)
					{
						token.Append(inputS[pos]); // power
					} 

					token = new StringBuilder(Convert.ToDouble(token.ToString()).ToString()); // Convert number from scientific notation
				}	
			
				return token.ToString(); // return number
			}				
		}

		/// <summary>
		/// Returns key of input value from operators dictionary
		/// </summary>
		/// <returns>Returns key of input token</returns>        
		private string GetKeyInOperators(string token)
		{
			foreach (string key in operators.Keys)
				if (token == operators[key])
					return key;

			return "";
		}

		/// <summary>
		/// Returns key of input value from constants dictionary
		/// </summary>
		/// <returns>Returns key of input token</returns>  
		private string GetKeyInConstants(string token)
		{
			foreach (string key in constants.Keys)
				if (token == constants[key])
					return key;

			return "";
		}

		/// <summary>
		/// Calculate the expression in Reverse Polish Notation
		/// </summary>
		/// <param name="rpnExp">Expression in RPN</param>
		/// <returns>Result</returns>        
		private double Calculate(string rpnExp)
		{
			Stack<double> stack = new Stack<double>();
			int pos = 0;

			// Parse math expression in RPN	
			while (pos < rpnExp.Length)
			{
				string token = LexicalAnalyzePostfixExp(rpnExp, ref pos); //lexical analyse
				
				if (operators.ContainsKey(token))
				{
					if (GetNumberArguments(token) == 1)
					{
						double arg = stack.Pop();

						switch (token)
						{
							case unMinus: stack.Push((-1 * arg)); break;
							case unPlus: stack.Push(arg); break;
							case sqrt:
							case sqrt2:
								stack.Push(Math.Sqrt(arg)); break;
							case sin:
								if (isRadians)
									stack.Push(Math.Sin(arg));
								else
									stack.Push(Math.Sin(arg * Math.PI / 180)); break;
							case cos:
								if (isRadians)
									stack.Push(Math.Cos(arg));
								else
									stack.Push(Math.Cos(arg * Math.PI / 180)); break;
							case tg:
								if (isRadians)
									stack.Push(Math.Tan(arg));
								else
									stack.Push(Math.Tan(arg * Math.PI / 180)); break;
							case ctg:
								if (isRadians)
									stack.Push(1 / Math.Tan(arg));
								else
									stack.Push(1 / Math.Tan(arg * Math.PI / 180)); break;
							case sh: stack.Push(Math.Sinh(arg)); break;
							case ch: stack.Push(Math.Cosh(arg)); break;
							case th: stack.Push(Math.Tanh(arg)); break;
							case ln: stack.Push(Math.Log(arg)); break;
							case exp: stack.Push(Math.Exp(arg)); break;
							case abs: stack.Push(Math.Abs(arg)); break;
						}
					}
					else
					{
						double arg2 = stack.Pop();
						double arg1 = stack.Pop();

						switch (token)
						{
							case plus: stack.Push(arg1 + arg2); break;
							case minus: stack.Push(arg1 - arg2); break;
							case multiply: stack.Push(arg1 * arg2); break;
							case division:
								if (arg2 == 0)
									throw new DivideByZeroException();
								stack.Push(arg1 / arg2); break;
							case degree: stack.Push(Math.Pow(arg1, arg2)); break;
							case log: stack.Push(Math.Log(arg2) / Math.Log(arg1)); break;
						}
					}
				}
				else
					stack.Push(Convert.ToDouble(token));
			}

			if (stack.Count > 1)
				throw new ArgumentException();

			return stack.Peek();
		}

		/// <summary>
		/// Lexical analyzer for math expression in Reverse Polish Notation
		/// </summary>
		/// <returns>Token</returns>   
		private string LexicalAnalyzePostfixExp(string rpnExp, ref int pos)
		{
			StringBuilder token = new StringBuilder();

			if (rpnExp[pos] == numberMarker || rpnExp[pos] == operatorMarker)
				pos++;

			while (pos < rpnExp.Length && rpnExp[pos] != numberMarker && rpnExp[pos] != operatorMarker)
				token.Append(rpnExp[pos++]);

			return token.ToString();
		}

		#endregion
	}
}