﻿using System;
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
            if (this.tokens == null || this.tokens.Count == 0)
            {
                throw new ParserException("Nebyly nalezeny zadne tokeny, ktere by se mely parsovat!");
            }
            this.Statement = ParseStatement(TokenType.ERROR);

            if (this.index != this.tokens.Count)
            {
                throw new ParserException("Ocekavan konec souboru!");
            }
            logger.Log("Parsovani dokonceno.", Logger.Type.INFO);
        }

        private IStatement ParseStatement(TokenType beforeType)
        {
            IStatement result = null;

            if (this.index == this.tokens.Count)
            {
                throw new ParserException("Ocekavan vyraz, obdrzen konec souboru." + GetCharsNearError(this.index));
            }

            if (this.tokens[this.index].Type == TokenType.KEYWORD)
            {
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

                    if (this.index < this.tokens.Count && this.tokens[this.index].Type == TokenType.VARIABLE)
                    {
                        stateVar.Identificator = (string)this.tokens[this.index].Data;
                    }
                    else
                    {
                        throw new ParserException("Ocekavan identifikator za klicovym slovem 'promenna'" + GetCharsNearError(this.index));
                    }

                    this.index++;

                    if (this.index == this.tokens.Count || this.tokens[this.index].Type != TokenType.SPECIAL)
                    {
                        throw new ParserException("Ocekavano '=' obdrzen typ: " + this.tokens[this.index].Type + "." + GetCharsNearError(this.index));
                    }
                    if (((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) != SpecialChars.Equals)
                    {
                        throw new ParserException("Ocekavano '=' za identifikatorem." + GetCharsNearError(this.index));
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
                        throw new ParserException("Ocekavan identifikator za 'nactiInt'" + GetCharsNearError(this.index));
                    }
                }
                else if (this.tokens[this.index].Data.Equals("pro"))
                {
                    TokenType tt = this.tokens[this.index].Type;
                    this.index++;
                    StatementForLoop forLoop = new StatementForLoop();

                    if (this.index < this.tokens.Count && this.tokens[this.index].Data is string)
                    {
                        forLoop.Identificator = (string)this.tokens[this.index].Data;
                    }
                    else
                    {
                        throw new ParserException("Ocekavan identifikator za 'pro'" + GetCharsNearError(this.index));
                    }

                    this.index++;

                    if (this.index == this.tokens.Count || ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) != SpecialChars.Equals)
                    {
                        throw new ParserException("Za identifikatorem ocekavano '='" + GetCharsNearError(this.index));
                    }

                    this.index++;

                    forLoop.From = this.ParseExpression();

                    if (this.index == this.tokens.Count || !this.tokens[this.index].Data.Equals("do"))
                    {
                        throw new ParserException("Ocekavano 'do' za klicovym slovem 'pro'" + GetCharsNearError(this.index));
                    }

                    this.index++;

                    forLoop.To = this.ParseExpression();

                    if (this.index == this.tokens.Count || !this.tokens[this.index].Data.Equals("delej"))
                    {
                        throw new ParserException("Ocekavano 'delej' za klicovym slovem 'do'" + GetCharsNearError(this.index));
                    }

                    this.index++;

                    forLoop.Body = this.ParseStatement(tt);
                    result = forLoop;

                    if (this.index == this.tokens.Count || !this.tokens[this.index].Data.Equals("konec"))
                    {
                        throw new ParserException("Neukonceny 'pro' cyklus, ocekavano klicove slovo 'konec' za telem cyklu." + GetCharsNearError(this.index));
                    }

                    this.index++;
                }
                else if (this.tokens[this.index].Data is string)
                {
                    throw new ParserException("Nemelo by nastat :)" + GetCharsNearError(this.index));
                }
            }
            else if (beforeType == TokenType.ERROR)
            {
                throw new ParserException("ocekavano klicove slovo. Obdrzen datovy typ " + this.tokens[this.index].Type + GetCharsNearError(this.index));
            }
            //FIXME: nikdy se neprovede, asi zbytečný
            else if (this.tokens[this.index].Data is string && (beforeType != TokenType.WORD))
            {
                // assignment

                StatementAssign assign = new StatementAssign();
                assign.Identificator = (string)this.tokens[this.index++].Data;

                // FIXME očekáváno klíčové slovo, ale dostane string... nedokáže přetypovat
                if (this.index == this.tokens.Count || ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) != SpecialChars.Equals)
                {
                    throw new ParserException("ocekavano '='" + GetCharsNearError(this.index));
                }

                this.index++;

                assign.Expression = this.ParseExpression();
                result = assign;
            }
            else
            {
                throw new ParserException("Bylo ocekavano kliceove slovo, nebo retezec. Chyba parsovani na tokenu " + this.index + ": " + this.tokens[this.index] + GetCharsNearError(this.index));
            }

            if (this.tokens[this.index].Type != TokenType.SPECIAL)
            {
                throw new ParserException("Byl ocekavan ukoncovaci znak ';'" + GetCharsNearError(this.index));
            }
            if (this.index < this.tokens.Count && ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) == SpecialChars.Semicolon)
            {
                this.index++;

                if (this.index < this.tokens.Count && !this.tokens[this.index].Data.Equals("konec"))
                {
                    StatementSequence sequence = new StatementSequence();
                    sequence.First = result;
                    sequence.Second = this.ParseStatement(this.tokens[this.index].Type);
                    if (sequence.Second == null)
                    {
                        return null;
                    }
                    result = sequence;
                }
            }

            //logger.Log("Statement: " + result.ToString());
            return result;
        }

        private String GetCharsNearError(int index)
        {
            StringBuilder sb = new StringBuilder("\r\nChyba v okoli: '");

            for (int i = index - 10; i < index; i++)
            {
                if (i < 0)
                {
                    continue;
                }
                if (i >= this.tokens.Count)
                {
                    break;
                }
                sb.Append(this.tokens[i].Data.ToString()).Append(" ");
            }
            sb.Append("'");
            return sb.ToString();
        }

        private IExpression ParseExpression()
        {
            if (this.index >= this.tokens.Count - 1)
            {
                throw new ParserException("Za vyrazem ocekavan ukoncovaci znak ';', obdrzen konec souboru." + GetCharsNearError(this.index));
            }
            else if (
                ((this.tokens[this.index].Type == TokenType.NUMBER || this.tokens[this.index].Type == TokenType.VARIABLE) && (this.tokens[this.index + 1].Type == TokenType.MATHOPERATOR))
                || this.tokens[this.index].Type == TokenType.MATHOPERATOR
                )
            { //matematický výraz
                LinkedList<IExpression> mathExpressionList = new LinkedList<IExpression>();

                while (true)
                {
                    if (this.tokens[this.index].Type == TokenType.NUMBER)
                    {
                        mathExpressionList.AddLast(new ExpressionIntLiteral((int)((TokenNumber)this.tokens[this.index++]).Data));
                    }
                    else if (this.tokens[this.index].Type == TokenType.VARIABLE)
                    {
                        mathExpressionList.AddLast(new ExpressionVariable((String)((TokenWord)this.tokens[this.index++]).Data));
                    }
                    else if (this.tokens[this.index].Type == TokenType.MATHOPERATOR)
                    {
                        mathExpressionList.AddLast(new ExpressionMathOperator((MathOperators)((TokenSpecial)this.tokens[this.index++]).Data));
                    }

                    if (this.tokens[this.index].Type == TokenType.SPECIAL && ((SpecialChars)((TokenSpecial)this.tokens[this.index]).Data) == SpecialChars.Semicolon)
                    {
                        break;
                    }
                    if (this.index == this.tokens.Count)
                    {
                        throw new ParserException("Za vyrazem ocekavan ';', obdrzen konec souboru." + GetCharsNearError(this.index));
                    }
                }
                ExpressionMath expressionMathOutput = new ExpressionMath(mathExpressionList);
                logger.Log("Expression: " + TokenType.NUMBER.ToString() + ": " + expressionMathOutput.ToString());
                return expressionMathOutput;
            }
            else if (this.tokens[this.index].Type == TokenType.NUMBER)
            {
                logger.Log("Expression: " + TokenType.NUMBER.ToString() + ": " + this.tokens[this.index].Data.ToString());
                return new ExpressionIntLiteral((int)((TokenNumber)this.tokens[this.index++]).Data);
            }
            else if (this.tokens[this.index].Type == TokenType.WORD)
            {
                logger.Log("Expression: " + TokenType.WORD.ToString() + ": " + this.tokens[this.index].Data.ToString());
                return new ExpressionStringLiteral((String)((TokenWord)this.tokens[this.index++]).Data);
            }
            else if (this.tokens[this.index].Type == TokenType.VARIABLE)
            {
                logger.Log("Expression: " + TokenType.VARIABLE.ToString() + ": " + this.tokens[this.index].Data.ToString());
                return new ExpressionVariable((String)((TokenWord)this.tokens[this.index++]).Data);
            }
            else
            {
                throw new ParserException("Ocekavan retezec, cislo nebo promenna." + GetCharsNearError(this.index));
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
