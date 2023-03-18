using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompilationPrinciple {
    public class Scanner {
        public Scanner() {
            lineNumber = 0;
            getWorng = false;
            tokenList = new List<Token>();
        }

        static bool IsChar(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        static bool IsDigit(char c) => c >= '0' && c <= '9';
        static bool IsSingleSep(char c) => c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')' ||
        c == ';' || c == '[' || c == ']' || c == '=' || c == '<' || c == ',';

        Token createIDToken(String str,int index) {
            Token token = new Token();

            token.line = lineNumber;
            token.column = index;

            token.sem = str;

            Dictionarys dictionarys = new Dictionarys();

            //Determine if an identifier is a reserved word
            if (dictionarys.reservedWords.ContainsKey(str)) {
                token.lex = dictionarys.reservedWords[str];
            }
            //not a reserved word
            else {
                token.lex = LexType.ID;
            }
            return token;
        }

        //Direct steering method to achieve lexical analysis
        public void DirectSteeringMethod() {
            //Read input by line
            String line;
            try {
                //StreamReader sr = new StreamReader(Properties.Resources.SimpleExample);
                String[] strs = Properties.Resources.Simple8.Split("\r\n");
                //Read the first line of text
                //line = sr.ReadLine();
                for (int i = 0; i < strs.Length; i++) {
                    line = strs[i];
                    lineNumber++;
                    //Each line of characters is processed here
                    Console.WriteLine(line);
                    DSLine(line);
                }
                tokenList.Add(new Token {line = lineNumber+1, column = 0, lex = LexType.ENDFILE, sem = "" });
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
            } finally {
                Console.WriteLine("Executing finally block.");
            }

        }

        void DSLine(string line) {
            int index = 0;
            while (index < line.Length && !getWorng) {
                //Identifiers
                if (IsChar(line[index])) {
                    int startIndex = index;
                    string idBuff = line[index++].ToString();
                    while (index < line.Length && (IsChar(line[index]) || IsDigit(line[index]))) {
                        idBuff += line[index].ToString();
                        index++;
                    }
                    tokenList.Add(createIDToken(idBuff, startIndex));
                }
                //Digital
                else if (IsDigit(line[index])) {
                    int startIndex = index;
                    string diBuff = line[index++].ToString();
                    while (index < line.Length && IsDigit(line[index])) {
                        diBuff += line[index].ToString();
                        index++;
                    }
                    tokenList.Add(new Token { line = lineNumber,column = startIndex, lex = LexType.INTC_VAL, sem = diBuff });
                }
                //Single-byte separator
                else if (IsSingleSep(line[index])) {
                    Token token = new Token();
                    token.line = lineNumber;
                    token.column = index;
                    token.sem = line[index].ToString();
                    // get lex type from separatorWords dictionary
                    Dictionarys dictionarys = new Dictionarys();
                    token.lex = dictionarys.separatorWords[line[index]];

                    tokenList.Add(token);
                    index++;
                }
                //Special characters processing
                else {
                    switch (line[index]) {
                        //Determine if an assignment symbol can be combined with "="
                        case ':':
                            if (index + 1 < line.Length) {
                                char nextChar = line[++index];
                                if (nextChar != '=') {
                                    Console.WriteLine("ERROR FOUND IN LINE " + lineNumber + "MISS FOUND \"=\" AFTER \":\"");
                                    getWorng = true;
                                } else {
                                    tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.ASSIGN, sem = ":=" });
                                    index++;
                                }
                            } else {
                                Console.WriteLine("ERROR FOUND IN LINE " + lineNumber + "MISS FOUND \"=\" AFTER \":\"");
                                getWorng = true;
                            }
                            break;
                        //There must be a '}' after the '{ ' and circled by them is the comment text
                        case '{':
                            string commentText = "";
                            while (index < line.Length && line[index] != '}') {
                                if (line[index] == '{')
                                    continue;
                                commentText += line[index].ToString();
                                index++;
                            }
                            index++;
                            //The comment text is not needed for subsequent compilation so the token sequence is not added
                            break;
                        //determine whether it is '..' (array lower bound flag) or '.' (end-of-program flag) or 'd.x' (member symbols flag)
                        case '.':
                            if (index + 1 < line.Length) {
                                char nextChar = line[index + 1];
                                if (nextChar == '.') {
                                    //array lower bound flag
                                    tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.UNDERRANGE, sem = ".." });
                                    index += 2;
                                } else {
                                    //member symbols
                                    tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.DOT, sem = ".（member symbols flag)" });
                                    index++;
                                }
                            } else {
                                // end-of-program flag
                                tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.DOT, sem = ".（end-of-program flag)" });
                                index++;
                            }
                            break;
                        //If the word is ' , the next character read in will be CHAR
                        case '\'':
                            if (index + 2 < line.Length) {
                                char nextChar = line[index + 1];
                                if (IsChar(nextChar) || IsDigit(nextChar)) {
                                    char nexNextChar = line[index + 2];
                                    if (nexNextChar == '\'') {
                                        tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.CHAR_T, sem = nextChar.ToString() });
                                        index += 3;
                                    } else {
                                        Console.WriteLine("ERROE : More than one character after a single quote or missing another single quote");
                                        getWorng = true;
                                    }
                                } else {
                                    Console.WriteLine("No characters after single quotes");
                                    getWorng = true;
                                }
                            } else {
                                Console.WriteLine("ERROE : More than one character after a single quote or missing another single quote");
                                getWorng = true;
                            }
                            break;
                        default:
                            if (' ' != line[index] && 32 != line[index] && 9 != line[index]) {
                                Console.WriteLine("Unexpected Char : " + line[index] + "in line : " + lineNumber);
                                getWorng = true;
                            } else {
                                index++;
                            }
                            break;
                    }

                }
            }
        }

        //State transition table method to achieve lexical analysis
        public void StateTransitionTableMethod() {

        }

        public void outPutTokenList() {
            for (int i = 0; i < tokenList.Count; i++) {
                Console.WriteLine(tokenList[i].line + "  " + tokenList[i].column + "   " + Enum.GetName(typeof(LexType), tokenList[i].lex) + "  " + tokenList[i].sem);
            }
        }

        private int lineNumber;
        bool getWorng;
        public List<Token> tokenList { get; set; }
    }
}
