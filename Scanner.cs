using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Scanner
    {
        public IList<IToken> Tokens { get; private set; }
        TextReader file;
        Logger logger = Logger.GetInstance();


        public Scanner(TextReader file)
        {
            Tokens = new List<IToken>();
            this.file = file;
            logger.Log("Probiha scanovani souboru.", Logger.Type.INFO);
            if (file.ReadLine() != "# eMTe")
            {
                String s = "Soubor nema spravny format. Musi zacinat prvnim radkem: \"# eMTe\"!";
                logger.Log(s, Logger.Type.ERROR);
                throw new IOException(s);
            }
            Scan();

        }
        private void Scan()
        {
            while (file.Peek() != -1)
            {
                char ch = (char)file.Peek();
                DoAnalyzeChar(ch);
            }
        }

        private void DoAnalyzeChar(char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                file.Read();
                return;
            }
            else if (char.IsLetter(ch))
            {
                KeywordParser(ch);
            }
            else if (ch == '"')
            {
                WordParser(ch);
            }
            else if (char.IsNumber(ch))
            {
                NumberParser(ch);
            }
            else if (ch == '#')
            {
                CommentParser(ch);
            }
            else
            {
                SpecialKeyParser(ch);
            }
        }

        private void SpecialKeyParser(char ch)
        {
            switch (ch)
            {
                case '+':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(MathOperators.Addition));
                    break;
                case '-':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(MathOperators.Subtraction));
                    break;
                case '*':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(MathOperators.Multiplication));
                    break;
                case '/':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(MathOperators.Division));
                    break;
                case '(':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(MathOperators.LeftBracket));
                    break;
                case ')':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(MathOperators.RightBracket));
                    break;
                case '=':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(SpecialChars.Equals));
                    break;

                case ';':
                    file.Read();
                    this.Tokens.Add(new TokenSpecial(SpecialChars.Semicolon));
                    break;
                default:
                    logger.Log("Neznamy literal (byl preskocen): " + ch, Logger.Type.WARNING);
                    file.Read();
                    break;
            }
        }

        /// <summary>
        /// Přečte řádek s komentářem a zahodí ho.
        /// </summary>
        /// <param name="ch"></param>
        private void CommentParser(char ch)
        {
            String s = file.ReadLine();
            logger.Log("Komentar: " + s);
        }

        /// <summary>
        /// Vytvoří identifikátor ze znaků postupným čtením zdrojového souboru
        /// </summary>
        /// <param name="ch">znak</param>
        private void KeywordParser(char ch)
        {
            StringBuilder word = new StringBuilder();

            while (char.IsLetter(ch))
            {
                word.Append(ch);
                file.Read();

                if (file.Peek() == -1)
                {
                    break;
                }

                ch = (char)file.Peek();
            }
            if (Enum.GetNames(typeof(Keywords)).Contains(word.ToString().ToUpper()))
            {
                logger.Log("Keyword: " + word.ToString());
                this.Tokens.Add(new TokenWord(word.ToString(), TokenType.KEYWORD));
            }
            else
            {
                logger.Log("Variable: " + word.ToString());
                this.Tokens.Add(new TokenWord(word.ToString(), TokenType.VARIABLE));
            }
        }
        /// <summary>
        /// Přečte řetězec textu a uloží ho
        /// </summary>
        /// <param name="ch"></param>
        private void WordParser(char ch)
        {
            // string literal
            StringBuilder word = new StringBuilder();

            file.Read(); // skip the '"'

            if (file.Peek() == -1)
            {
                throw new System.Exception("unterminated string literal");
            }

            while ((ch = (char)file.Peek()) != '"')
            {
                word.Append(ch);
                file.Read();

                if (file.Peek() == -1)
                {
                    throw new System.Exception("unterminated string literal");
                }
            }

            // skip the terminating "
            file.Read();
            logger.Log("Word: " + word.ToString());
            this.Tokens.Add(new TokenWord(word.ToString()));
        }
        private void NumberParser(char ch)
        {
            // numeric literal

            StringBuilder digit = new StringBuilder();

            while (char.IsDigit(ch))
            {
                digit.Append(ch);
                file.Read();

                if (file.Peek() == -1)
                {
                    break;
                }

                ch = (char)file.Peek();
            }
            logger.Log("Number: " + digit.ToString());
            this.Tokens.Add(new TokenNumber(int.Parse(digit.ToString())));
        }
    }
}
