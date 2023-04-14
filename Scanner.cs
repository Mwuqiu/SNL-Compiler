using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompilationPrinciple {
    public class Scanner {
        public Scanner(String code) {
            lineNumber = 0;
            getWorng = false;
            tokenList = new List<Token>();
            CodeText = code;
            errorList = new List<String>();
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
                //String[] strs = Properties.Resources.Simple7.Split("\r\n");
                String[] strs = CodeText.Split("\r\n");

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
                Console.WriteLine("Exception: " + e.Message + e.StackTrace);
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
                    if (index != line.Length && IsChar(line[index])) {
                        while (index < line.Length && (IsChar(line[index]) || IsDigit(line[index]))) {
                            diBuff += line[index].ToString();
                            index++;
                        }
                        String err = "[ERROR] LINE  " + lineNumber + ", Col " + index + " NEAR TOKEN '" + diBuff + "': NUMBERS SHOULD NOT BE STATRED AS VARIABLES.";
                        Console.WriteLine(err);
                        getWorng = true;
                        errorList.Add(err);
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
                                    String err = "[ERROR] LINE  " + lineNumber + ", Col " + index + ": MISS FOUND \"=\" AFTER \":\"";
                                    Console.WriteLine(err);
                                    getWorng = true;
                                    errorList.Add(err);
                                } else {
                                    tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.ASSIGN, sem = ":=" });
                                    index++;
                                }
                            } else {
                                String err = "[ERROR] LINE " + lineNumber + ", Col " + index + ": MISS FOUND \"=\" AFTER \":\"";
                                Console.WriteLine(err);
                                getWorng = true;
                                errorList.Add(err);
                            }
                            break;
                        //There must be a '}' after the '{ ' and circled by them is the comment text
                        case '{':
                            string commentText = "";
                            while (index < line.Length && line[index] != '}') {
                                if (line[index] == '{') {
                                    index++;
                                    continue;
                                }
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
                                    tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.DOT, sem = "." });
                                    index++;
                                }
                            } else {
                                // end-of-program flag
                                tokenList.Add(new Token { line = lineNumber,column = index, lex = LexType.DOT, sem = "." });
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
                                        String err = "[ERROR] LINE " + lineNumber + ", Col " + index + ": More than one character after a single quote or  MISSing another single quote";
                                        Console.WriteLine(err);
                                        getWorng = true;
                                        errorList.Add(err);
                                    }
                                } else {
                                    String err = "[ERROR] LINE " + lineNumber + ", Col " + index + ": No characters after single quotes";
                                    Console.WriteLine(err);
                                    getWorng = true;
                                    errorList.Add(err);
                                }
                            } else {
                                String err = "[ERROR] LINE " + lineNumber + ", Col " + index + ": More than one character after a single quote or  MISSing another single quote";
                                Console.WriteLine(err);
                                getWorng = true;
                                errorList.Add(err);
                            }
                            break;
                        default:
                            if (' ' != line[index] && 32 != line[index] && 9 != line[index]) {
                                String err = "[ERROR] LINE " + lineNumber + ", Col " + index + ": Unexpected Char : " + line[index];
                                Console.WriteLine(err);
                                getWorng = true;
                                errorList.Add(err);
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

        public String outPutTokenList() {
            String res = String.Format("{0,-5} {1, -5} {2, -13} {3, -10}\r\n", "Line", "Col", "Name", "Type");
            for (int i = 0; i < 38; i++)
                res += "-";
            res += "\r\n";
            for (int i = 0; i < tokenList.Count; i++) {
                Console.WriteLine(tokenList[i].line + "  " + tokenList[i].column + "   " + Enum.GetName(typeof(LexType), tokenList[i].lex) + "  " + tokenList[i].sem);
                res += String.Format("{0,-5} {1, -5} {2, -13} {3, -15}", tokenList[i].line, tokenList[i].column, Enum.GetName(typeof(LexType), tokenList[i].lex) , tokenList[i].sem);
                res += "\r\n";
            }
            Console.Write(res);
            return res;
        }

        private int lineNumber;
        public bool getWorng { get; set; }
        public List<Token> tokenList { get; set; }
        public List<String> errorList { get; set; }
        String CodeText;
    }
}
