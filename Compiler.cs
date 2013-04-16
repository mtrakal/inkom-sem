using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ConsoleApplication1
{
    class Compiler
    {
        Logger logger = Logger.GetInstance();
        ILGenerator il = null;
        Dictionary<string, LocalBuilder> symbolTable;
        Dictionary<String, int> variableValues = new Dictionary<String, int>();

        public Compiler(IStatement stmt, string moduleName)
        {
            logger.Log("Zacina kompilace souboru.", Logger.Type.INFO);

            if (Path.GetDirectoryName(moduleName) != null && Path.GetDirectoryName(moduleName) != String.Empty)
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(moduleName));
            }
            //if (Path.GetFileName(moduleName) != moduleName)
            //{
            //    throw new System.Exception("can only output into current directory!");
            //}

            AssemblyName name = new AssemblyName(Path.GetFileNameWithoutExtension(moduleName));
            AssemblyBuilder asmb = System.AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            ModuleBuilder modb = asmb.DefineDynamicModule(moduleName);
            TypeBuilder typeBuilder = modb.DefineType("Foo");

            MethodBuilder methb = typeBuilder.DefineMethod("Main", MethodAttributes.Static, typeof(void), System.Type.EmptyTypes);

            // CodeGenerator
            this.il = methb.GetILGenerator();
            this.symbolTable = new Dictionary<string, LocalBuilder>();

            // Go Compile!
            this.GenStmt(stmt);

            il.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            modb.CreateGlobalFunctions();
            asmb.SetEntryPoint(methb);

            asmb.Save(Path.GetFileName(moduleName));
            this.symbolTable = null;
            this.il = null;

            logger.Log("Kompilace dokoncena. Nazev souboru: " + name + ".exe.", Logger.Type.INFO);
        }

        private void GenStmt(IStatement stmt)
        {
            if (stmt is StatementSequence)
            {
                StatementSequence seq = (StatementSequence)stmt;
                this.GenStmt(seq.First);
                this.GenStmt(seq.Second);
            }

            else if (stmt is StatementVariable)
            {
                // declare a local
                StatementVariable declare = (StatementVariable)stmt;
                this.symbolTable[declare.Identificator] = this.il.DeclareLocal(this.TypeOfExpr(declare.Expression));

                // set the initial value
                StatementAssign assign = new StatementAssign();
                assign.Identificator = declare.Identificator;
                assign.Expression = declare.Expression;
                this.GenStmt(assign);
            }

            else if (stmt is StatementAssign)
            {
                StatementAssign assign = (StatementAssign)stmt;
                if (!variableValues.ContainsKey(assign.Identificator))
                {
                    if (assign.Expression is ExpressionVariable)
                    {
                        String key = ((ExpressionVariable)(assign.Expression)).Identificator.ToString();
                        variableValues.Add(assign.Identificator, variableValues[key]);
                    }
                    else if (assign.Expression is ExpressionMath)
                    {
                        variableValues.Add(assign.Identificator, Calculate(assign.Expression));
                    }
                    else if (assign.Expression is ExpressionIntLiteral)
                    {
                        variableValues.Add(assign.Identificator, ((ExpressionIntLiteral)assign.Expression).Value);
                    }
                }
                else
                {
                    if (assign.Expression is ExpressionVariable)
                    {
                        String key = ((ExpressionVariable)(assign.Expression)).Identificator.ToString();
                        variableValues[assign.Identificator] = variableValues[key];
                    }
                    else if (assign.Expression is ExpressionMath)
                    {
                        variableValues[assign.Identificator] = Calculate(assign.Expression);
                    }
                    else if (assign.Expression is ExpressionIntLiteral)
                    {
                        variableValues[assign.Identificator] = ((ExpressionIntLiteral)assign.Expression).Value;
                    }
                }
                this.GenExpr(assign.Expression, this.TypeOfExpr(assign.Expression));
                this.Store(assign.Identificator, this.TypeOfExpr(assign.Expression));
            }
            else if (stmt is StatementPrint)
            {
                // the "print" statement is an alias for System.Console.WriteLine. 
                // it uses the string case
                this.GenExpr(((StatementPrint)stmt).Expression, typeof(string));
                this.il.Emit(OpCodes.Call, typeof(System.Console).GetMethod("WriteLine", new System.Type[] { typeof(string) }));
            }

            else if (stmt is StatementReadInt)
            {
                this.il.Emit(OpCodes.Call, typeof(System.Console).GetMethod("ReadLine", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new System.Type[] { }, null));
                this.il.Emit(OpCodes.Call, typeof(int).GetMethod("Parse", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new System.Type[] { typeof(string) }, null));
                this.Store(((StatementReadInt)stmt).Identificator, typeof(int));
            }
            else if (stmt is StatementForLoop)
            {
                // example: 
                // for x = 0 to 100 do
                //   print "hello";
                // end;

                // x = 0
                StatementForLoop forLoop = (StatementForLoop)stmt;
                StatementAssign assign = new StatementAssign();
                assign.Identificator = forLoop.Identificator;
                assign.Expression = forLoop.From;
                this.GenStmt(assign);
                // jump to the test
                Label test = this.il.DefineLabel();
                this.il.Emit(OpCodes.Br, test);

                // statements in the body of the for loop
                Label body = this.il.DefineLabel();
                this.il.MarkLabel(body);
                this.GenStmt(forLoop.Body);

                // to (increment the value of x)
                this.il.Emit(OpCodes.Ldloc, this.symbolTable[forLoop.Identificator]);
                this.il.Emit(OpCodes.Ldc_I4, 1);
                this.il.Emit(OpCodes.Add);
                this.Store(forLoop.Identificator, typeof(int));

                // **test** does x equal 100? (do the test)
                this.il.MarkLabel(test);
                this.il.Emit(OpCodes.Ldloc, this.symbolTable[forLoop.Identificator]);
                this.GenExpr(forLoop.To, typeof(int));
                this.il.Emit(OpCodes.Blt, body);
            }
            else
            {
                throw new System.Exception("don't know how to gen a " + stmt.GetType().Name);
            }
        }

        private void Store(string name, System.Type type)
        {
            if (this.symbolTable.ContainsKey(name))
            {
                LocalBuilder locb = this.symbolTable[name];

                if (locb.LocalType == type)
                {
                    this.il.Emit(OpCodes.Stloc, this.symbolTable[name]);
                }
                else
                {
                    throw new System.Exception("'" + name + "' is of type " + locb.LocalType.Name + " but attempted to store value of type " + type.Name);
                }
            }
            else
            {
                throw new System.Exception("undeclared variable '" + name + "'");
            }
        }

        private void GenExpr(IExpression expr, System.Type expectedType)
        {
            System.Type deliveredType;

            if (expr is ExpressionStringLiteral)
            {
                deliveredType = typeof(string);
                this.il.Emit(OpCodes.Ldstr, ((ExpressionStringLiteral)expr).Value);
            }
            else if (expr is ExpressionIntLiteral)
            {
                deliveredType = typeof(int);
                this.il.Emit(OpCodes.Ldc_I4, ((ExpressionIntLiteral)expr).Value);
            }
            else if (expr is ExpressionVariable)
            {
                string ident = ((ExpressionVariable)expr).Identificator;
                deliveredType = this.TypeOfExpr(expr);

                if (!this.symbolTable.ContainsKey(ident))
                {
                    throw new System.Exception("undeclared variable '" + ident + "'");
                }

                this.il.Emit(OpCodes.Ldloc, this.symbolTable[ident]);
            }
            else if (expr is ExpressionMath)
            {
                string ident = ((ExpressionMath)expr).MathExpression.ToString();
                deliveredType = this.TypeOfExpr(expr);

                int result = Calculate(expr);

                this.il.Emit(OpCodes.Ldc_I4, result);
            }
            else
            {
                throw new System.Exception("don't know how to generate " + expr.GetType().Name);
            }

            if (deliveredType != expectedType)
            {
                if (deliveredType == typeof(int) &&
                    expectedType == typeof(string))
                {
                    this.il.Emit(OpCodes.Box, typeof(int));
                    this.il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                }
                else
                {
                    throw new System.Exception("can't coerce a " + deliveredType.Name + " to a " + expectedType.Name);
                }
            }
        }

        private int Calculate(IExpression expr)
        {
            MathParser calc = new MathParser();
            StringBuilder sb = new StringBuilder();
            foreach (IExpression item in ((ExpressionMath)expr).MathExpression)
            {
                if (item is ExpressionVariable)
                {
                    if (!this.variableValues.ContainsKey(((ExpressionVariable)item).Identificator))
                    {
                        throw new System.Exception("undeclared variable in variableValues'" + ((ExpressionVariable)item).Identificator + "'");
                    }
                    sb.Append(this.variableValues[(((ExpressionVariable)item).Identificator)]);

                }
                else if (item is ExpressionIntLiteral)
                {
                    sb.Append(((ExpressionIntLiteral)item).Value);
                }
                else
                {
                    switch (((ExpressionMathOperator)item).Operator)
                    {
                        case MathOperators.Addition:
                            sb.Append("+");
                            break;
                        case MathOperators.Subtraction:
                            sb.Append("-");
                            break;
                        case MathOperators.Multiplication:
                            sb.Append("*");
                            break;
                        case MathOperators.Division:
                            sb.Append("/");
                            break;
                        case MathOperators.LeftBracket:
                            sb.Append("(");
                            break;
                        case MathOperators.RightBracket:
                            sb.Append(")");
                            break;
                    }
                }
            }
            String s = sb.ToString();
            return int.Parse(Math.Round(calc.Parse(s, false)).ToString());
        }

        private System.Type TypeOfExpr(IExpression expr)
        {
            if (expr is ExpressionStringLiteral)
            {
                return typeof(string);
            }
            else if (expr is ExpressionIntLiteral)
            {
                return typeof(int);
            }
            else if (expr is ExpressionVariable)
            {
                ExpressionVariable var = (ExpressionVariable)expr;
                if (this.symbolTable.ContainsKey(var.Identificator))
                {
                    LocalBuilder locb = this.symbolTable[var.Identificator];
                    return locb.LocalType;
                }
                else
                {
                    throw new System.Exception("undeclared variable '" + var.Identificator + "'");
                }
            }
            else if (expr is ExpressionMath)
            {
                return typeof(int);
            }
            else
            {
                throw new System.Exception("don't know how to calculate the type of " + expr.GetType().Name);
            }
        }
    }
}
