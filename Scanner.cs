using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilationPrinciple
{
    public class Scanner
    {

        public Scanner()
        {
            lineNumber = 0;

            tokenList = new List<Token>();
        }

        static bool IsChar(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        static bool IsDigit(char c) { return c >= '0' && c <= '9'; }
        static bool IsSingleSep(char c) => c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')' ||
        c == ';' || c == '[' || c == ']' || c == '=' || c == '<' || c == ',';

        
        Token createIDToken(String str)
        {
            Token token = new Token();

            token.line = lineNumber;
            token.sem = str;
            
            Dictionarys dictionarys = new Dictionarys();

            //Determine if an identifier is a reserved word
            if (dictionarys.reservedWords.ContainsKey(str))
            {
                token.lex = dictionarys.reservedWords[str];
            }
            //not a reserved word
            else
            {
                token.lex = LexType.ID;
            }
            return token;
        }


        //Direct steering method to achieve lexical analysis
        public void DirectSteeringMethod()
        {
            //Read input by line
            String line;
            try
            {
                StreamReader sr = new StreamReader("C:\\SimpleExample.txt");
                //Read the first line of text
                line = sr.ReadLine();
                while (line != null)
                {
                    //Each line of characters is processed here
                    Console.WriteLine(line);
                    //DSLine(line);
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

        }
       
        void DSLine(string line)
        {
            int index = 0;
            while(index < line.Length)
            {
                //Identifiers
                if (IsChar(line[index]))
                {
                    string idBuff = line[index++].ToString();
                    while (IsChar(line[index]) || IsDigit(line[index]))
                    {
                        idBuff += line[index].ToString();
                        index++;
                    }
                    tokenList.Add(createIDToken(idBuff));                                       
                }
                //Digital
                else if (IsDigit(line[index]))
                {
                    string diBuff = line[index++].ToString();
                    while (IsDigit(line[index]))
                    {
                        diBuff += line[index].ToString();
                        index++;
                    }
                    tokenList.Add(new Token { line = lineNumber, lex = LexType.INTC_VAL,sem = diBuff});
                }
                //Single-byte separator
                else if (IsSingleSep(line[index])){
                    Token token = new Token();
                    token.line = lineNumber;
                    token.sem = line[index].ToString();
                    // get lex type from separatorWords dictionary
                    Dictionarys dictionarys = new Dictionarys();
                    token.lex =  dictionarys.separatorWords[line[index]];

                    tokenList.Add(token);
                    index++;
                }
                //
                else
                {

                }
            }
        }

        private int lineNumber;

        List<Token> tokenList;
    }
}
