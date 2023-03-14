using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CompilationPrinciple.SyntaxClass;

namespace CompilationPrinciple {

    internal class LL1SyntaxAnalysis {

        public LL1SyntaxAnalysis(List<Token> tokenList) {
            // get tokenlist from scanner
            this.tokenList = tokenList;
           
            //cheate the root of syntax tree
            root = new SyntaxTreeNode(NodeKind.ProK);

            symbalStack = new Stack<LexType>();
            syntaxTreeStack = new Stack<SyntaxTreeNode>();
            opStack = new Stack<SyntaxTreeNode>();
            numStack = new Stack<SyntaxTreeNode>();

            //pressing into the syntax tree stack in reverse order
            syntaxTreeStack.Push(root.child[2]);
            syntaxTreeStack.Push(root.child[1]);
            syntaxTreeStack.Push(root.child[0]);
        }

        List<Token> tokenList;
        SyntaxTreeNode root;
        SyntaxTreeNode currentP; 
        Stack<LexType> symbalStack;// token sequencce
        Stack<SyntaxTreeNode> syntaxTreeStack; //Generate syntax trees for declaration and statement parts        
        Stack<SyntaxTreeNode> opStack; //Generate the expression part
        Stack<SyntaxTreeNode> numStack;
        
        
        //returns the priority of the operator,the higher the return value the higher the priority
        int priosity(LexType op) {
            int pri;
            switch (op) {
                case LexType.END:
                    pri = 0;
                    break;
                // < and = share the same priority
                case LexType.LT:
                case LexType.EQ:
                    pri = 1;
                    break;
                //+ and - share the same priority
                case LexType.PLUS:
                case LexType.MINUS:
                    pri = 2;
                    break;
                case LexType.TIMES:
                case LexType.DIVIDE:
                    pri = 3;
                    break;
                default:
                    Console.WriteLine("ERROR : Unknown Operators");
                    pri = -1;
                    break;
            }
            return pri;
        }


        //Select the code to be executed according to the generator number
        void process(int num) {
            switch (num) { 
                //<Program> ::= ProgramHead DeclarePart ProgramBody .
                case 1:
                    symbalStack.Push(LexType.DOT);
                    symbalStack.Push(LexType.ProgramBody);
                    symbalStack.Push(LexType.DeclarePart);
                    symbalStack.Push(LexType.ProgramHead);
                    break;
                //<ProgramHead> ::= PROGRAM ProgramName
                case 2:
                    symbalStack.Push(LexType.ProgramName);
                    symbalStack.Push(LexType.PROGRAM);
                    //create program header node
                    currentP = new SyntaxTreeNode(NodeKind.PheadK);
                    //the SON node of the root node point to the Program Head Node                    
                    SyntaxTreeNode topNode = syntaxTreeStack.Pop();
                    //topNode = currentP;
                    break;
                //<ProgramName> ::= ID;
                case 3:
                    symbalStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.idnum++;
                    break;
                //<DeclarePart> ::= TypeDec VarDec FuncDec
                case 4:
                    symbalStack.Push(LexType.ProcDec);
                    symbalStack.Push(LexType.VarDec);
                    symbalStack.Push(LexType.TypeDec);
                    break;
                //< TypeDec > ::= ε
                case 5:
                    break;
                //<TypeDec> ::= TypeDeclaration
                case 6:
                    symbalStack.Push(LexType.TypeDeclaration);
                    break;
                //<TypeDeclaration> ::= TYPE TypeDecList 
                case 7:
                    symbalStack.Push(LexType.TypeDecList);
                    symbalStack.Push(LexType.TYPE);

                    //Create Type Declarations Nodes
                    currentP = new SyntaxTreeNode(NodeKind.TypeK);
                    topNode = syntaxTreeStack.Pop();
                    topNode = currentP;
                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<TypeDecList> ::= TypeId = TypeDef ; TypeDecMore
                case 8:
                    symbalStack.Push(LexType.TypeDecMore);
                    symbalStack.Push(LexType.SEMI);
                    symbalStack.Push(LexType.TypeName);
                    symbalStack.Push(LexType.EQ);
                    symbalStack.Push(LexType.TypeId);

                    //Create Declarations Nodes
                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    topNode = syntaxTreeStack.Pop();
                    topNode = currentP;
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<TypeDecMore> ::= ε
                case 9:
                    //pop the sibling node of the last type declaration node , complete the type section
                    syntaxTreeStack.Pop();
                    break;
                //<TypeDecMore> ::= TypeDecList 
                case 10:
                    symbalStack.Push(LexType.TypeDecList);
                    break;
                //<TypeId > ::= ID
                case 11:
                    symbalStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.idnum++;
                    break;
                //<TypeDef> ::= BaseType
                case 12:
                    symbalStack.Push(LexType.BaseType);
                    DecKind temp = currentP.decKind;
                    break;
                //<TypeDef > ::= StructureType 
                case 13:
                    symbalStack.Push(LexType.StructureType);
                    break;
                //<TypeDef> ::= ID
                case 14:
                    symbalStack.Push(LexType.ID);
                    currentP.decKind = DecKind.IdK;
                    currentP.typeName = tokenList.First().sem;
                    break;
                //<BaseType> ::= INTEGER 
                case 15:
                    symbalStack.Push(LexType.INTEGER_T);
                    temp = DecKind.IntegerK;
                    break;

                case 16:
                    break;


            }
        }


        //the main function of LL1 syntax analysis , returns the generated syntax analysis tree 
        public void parse() {

            bool worng = false;

            //bring in the ll1 table
            LL1Supporter lL1Supporter = new LL1Supporter();
            
            //push the start symbal
            symbalStack.Push(LexType.Program);
                                   
            //when the symbal stack is not empty
            while(symbalStack.Count != 0) {
                if(tokenList.Count == 0) {
                    Console.WriteLine("Tokenlist is empty !");
                }

                //if the top of symbal stack is a non-ultimate
                if (Enum.IsDefined(typeof(NonUltimate), symbalStack.Peek())){ 
                    LexType lexType = (LexType)symbalStack.Pop();
                    Token token = tokenList.First();
                    int generNumber = lL1Supporter.LL1Table[(int)lexType, (int)token.lex];
                    // the conversion is legal
                    if (generNumber >= 1 && generNumber <= 103) {
                        process(generNumber);
                    } else {
                        Console.WriteLine("there is not a convert from : " + lexType + "to :" + Enum.GetName(typeof(LexType), token.lex));
                        worng = true;
                    }
                }

                //if the top of symbal stack is a ultimate
                if (Enum.IsDefined(typeof(Ultimate), symbalStack.Peek())) {
                    LexType lexType = (LexType)symbalStack.Peek();
                    Token token = tokenList.First();
                    
                    if(lexType == token.lex) {
                        // match !!
                        symbalStack.Pop();
                        tokenList.RemoveAt(0);
                    } else {
                        Console.WriteLine("unexpected token : " + Enum.GetName(typeof(LexType), token.lex) + "in line" + token.line);
                        //remove the wrong token and continue to match
                        tokenList.RemoveAt(0);
                        worng = true;
                    }
                }                
            }

            //If there are no errors throughout
            if (!worng) {
                Console.WriteLine("LL1 match success ! ");
            }           
        }
    
    }
}