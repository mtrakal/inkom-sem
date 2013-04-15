using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Parser
    {
        IList<IToken> tokens;
        public IStatement Statement { get; private set; }
        int index = 0;
        Logger logger = Logger.GetInstance();

        public Parser(IList<IToken> tokens)
        {
            this.tokens = tokens;
            logger.Log("Probiha parsovani souboru.", Logger.Type.INFO);
            this.Statement = ParseStatement();

            if (this.index != this.tokens.Count)
            {
                throw new System.Exception("expected EOF");
            }
        }

        private IStatement ParseStatement()
        {
            IStatement result = null;

            if (this.index == this.tokens.Count)
            {
                throw new System.Exception("expected statement, got EOF");
            }


            if (this.tokens[this.index].Data.Equals("vypis"))
            {
                this.index++;
                StatementPrint sp = new StatementPrint();
                sp.Expression = ParseExpression();
                result = sp;
            }
            else if (this.tokens[this.index].Data.Equals("promenna"))
            {
                this.index++;
                StatementVariable stateVar = new StatementVariable();

                if (this.index < this.tokens.Count && this.tokens[this.index].Data is string)
                {
                    stateVar.Identificator = (string)this.tokens[this.index].Data;
                }
                else
                {
                    throw new System.Exception("expected variable name after 'promenna'");
                }

                this.index++;

                if (this.index == this.tokens.Count || ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) != SpecialChars.Equals)
                {
                    throw new System.Exception("expected = after 'promenna'");
                }

                this.index++;

                stateVar.Expression = this.ParseExpression();
                result = stateVar;
            }
            else if (this.tokens[this.index].Data.Equals("nactiInt"))
            {
                this.index++;
                StatementReadInt readInt = new StatementReadInt();

                if (this.index < this.tokens.Count &&
                    this.tokens[this.index].Data is string)
                {
                    readInt.Identificator = (string)this.tokens[this.index++].Data;
                    result = readInt;
                }
                else
                {
                    throw new System.Exception("expected variable name after 'nactiInt'");
                }
            }
            else if (this.tokens[this.index].Data.Equals("pro"))
            {
                this.index++;
                StatementForLoop forLoop = new StatementForLoop();

                if (this.index < this.tokens.Count && this.tokens[this.index].Data is string)
                {
                    forLoop.Identificator = (string)this.tokens[this.index].Data;
                }
                else
                {
                    throw new System.Exception("expected identifier after 'pro'");
                }

                this.index++;

                if (this.index == this.tokens.Count || ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) != SpecialChars.Equals)
                {
                    throw new System.Exception("for missing '='");
                }

                this.index++;

                forLoop.From = this.ParseExpression();

                if (this.index == this.tokens.Count || !this.tokens[this.index].Data.Equals("do"))
                {
                    throw new System.Exception("expected 'do' after pro");
                }

                this.index++;

                forLoop.To = this.ParseExpression();

                if (this.index == this.tokens.Count || !this.tokens[this.index].Data.Equals("delej"))
                {
                    throw new System.Exception("expected 'do' after from expression in for loop");
                }

                this.index++;

                forLoop.Body = this.ParseStatement();
                result = forLoop;

                if (this.index == this.tokens.Count || !this.tokens[this.index].Data.Equals("konec"))
                {
                    throw new System.Exception("unterminated 'for' loop body");
                }

                this.index++;
            }
            else if (this.tokens[this.index].Data is string)
            {
                // assignment

                StatementAssign assign = new StatementAssign();
                assign.Identificator = (string)this.tokens[this.index++].Data;

                if (this.index == this.tokens.Count || ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) != SpecialChars.Equals)
                {
                    throw new System.Exception("expected '='");
                }

                this.index++;

                assign.Expression = this.ParseExpression();
                result = assign;
            }
            else
            {
                throw new System.Exception("parse error at token " + this.index + ": " + this.tokens[this.index]);
            }

            if (this.index < this.tokens.Count && ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) == SpecialChars.Semicolon)
            {
                this.index++;

                if (this.index < this.tokens.Count && !this.tokens[this.index].Data.Equals("konec"))
                {
                    StatementSequence sequence = new StatementSequence();
                    sequence.First = result;
                    sequence.Second = this.ParseStatement();
                    result = sequence;
                }
            }

            logger.Log("Statement: " + result.ToString());
            return result;
        }

        private IExpression ParseExpression()
        {
            if (this.index == this.tokens.Count)
            {
                throw new System.Exception("expected expression, got EOF");
            }
            else if (
                ((this.tokens[this.index].Type == TokenType.NUMBER || this.tokens[this.index].Type == TokenType.KEYWORD)
                && (this.tokens[this.index + 1].Type == TokenType.MATHOPERATOR))
                || this.tokens[this.index].Type == TokenType.MATHOPERATOR
                )
            { //matematický výraz
                LinkedList<IExpression> mathExpressionList = new LinkedList<IExpression>();

                while (true)
                {
                    if (this.tokens[this.index].Type == TokenType.SPECIAL && ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) == SpecialChars.Semicolon)
                    {
                        break;
                    }
                    if (this.index == this.tokens.Count)
                    {
                        logger.Log("expected ;, got EOF", Logger.Type.ERROR);
                        throw new System.Exception("expected ;, got EOF");
                    }

                    if (this.tokens[this.index].Type == TokenType.NUMBER)
                    {
                        mathExpressionList.AddLast(new ExpressionIntLiteral((int)((TokenNumber)this.tokens[this.index++]).Data));
                    }
                    else if (this.tokens[this.index].Type == TokenType.KEYWORD)
                    {
                        mathExpressionList.AddLast(new ExpressionVariable((String)((TokenWord)this.tokens[this.index++]).Data));
                    }
                    else if (this.tokens[this.index].Type == TokenType.MATHOPERATOR)
                    {
                        mathExpressionList.AddLast(new ExpressionMathOperator((MathOperators)((TokenSpecial)this.tokens[this.index++]).Data));
                    }
                }
                logger.Log(mathExpressionList.ToString());
                return new ExpressionMath(mathExpressionList);
            }
            else if (this.tokens[this.index].Type == TokenType.NUMBER)
            {
                logger.Log("Expression: " + TokenType.NUMBER.ToString());
                return new ExpressionIntLiteral((int)((TokenNumber)this.tokens[this.index++]).Data);
            }
            else if (this.tokens[this.index].Type == TokenType.WORD)
            {
                logger.Log("Expression: " + TokenType.WORD.ToString());
                return new ExpressionStringLiteral((String)((TokenWord)this.tokens[this.index++]).Data);
            }
            else if (this.tokens[this.index].Type == TokenType.KEYWORD)
            {
                logger.Log("Expression: " + TokenType.KEYWORD.ToString());
                return new ExpressionVariable((String)((TokenWord)this.tokens[this.index++]).Data);
            }
            else
            {
                throw new System.Exception("expected string literal, int literal, or variable");
            }
        }
        private IExpression ParseExpressionOLD()
        {
            if (this.index == this.tokens.Count)
            {
                throw new System.Exception("expected expression, got EOF");
            }
            if (this.tokens[this.index].Type == TokenType.WORD)
            {
                logger.Log("Expression: " + TokenType.WORD.ToString());
                return new ExpressionStringLiteral((String)((TokenWord)this.tokens[this.index++]).Data);
            }
            else if (this.tokens[this.index].Type == TokenType.KEYWORD)
            {
                logger.Log("Expression: " + TokenType.KEYWORD.ToString());
                return new ExpressionVariable((String)((TokenWord)this.tokens[this.index++]).Data);
            }
            else if (this.tokens[this.index].Type == TokenType.NUMBER)
            {
                int value = (int)((TokenNumber)this.tokens[this.index++]).Data;
                logger.Log("Expression: " + TokenType.NUMBER.ToString());

                while (true)
                {
                    if (this.tokens[this.index].Type == TokenType.SPECIAL && ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) == SpecialChars.Semicolon)
                    {
                        break;
                    }
                    if (this.index == this.tokens.Count)
                    {
                        logger.Log("expected ;, got EOF", Logger.Type.ERROR);
                        throw new System.Exception("expected ;, got EOF");
                    }
                    if (this.tokens[this.index].Type != TokenType.MATHOPERATOR)
                    {
                        logger.Log("expected math operator, got " + this.tokens[this.index].Type.ToString(), Logger.Type.ERROR);
                        throw new System.Exception("expected math operator, got " + this.tokens[this.index].Type.ToString());
                    }
                    if (this.tokens[this.index + 1].Type != TokenType.NUMBER)
                    {
                        logger.Log("expected number, got " + this.tokens[this.index + 1].Type.ToString(), Logger.Type.ERROR);
                        throw new System.Exception("expected number, got " + this.tokens[this.index + 1].Type.ToString());
                    }

                    switch (((MathOperators)((TokenSpecial)this.tokens[this.index]).Data))
                    {
                        case MathOperators.Addition:
                            value += (int)this.tokens[this.index + 1].Data;
                            break;
                        case MathOperators.Subtraction:
                            value -= (int)this.tokens[this.index + 1].Data;
                            break;
                        case MathOperators.Multiplication:
                            value *= (int)this.tokens[this.index + 1].Data;
                            break;
                        case MathOperators.Division:
                            value /= (int)this.tokens[this.index + 1].Data;
                            break;
                        default:
                            break;
                    }
                    this.index += 2; // special char + number
                }
                return new ExpressionIntLiteral(value);
            }

            else
            {
                throw new System.Exception("expected string literal, int literal, or variable");
            }
        }


    }
}
