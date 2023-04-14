using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CompilationPrinciple.SyntaxClass;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode.Attr.ExpAttr;

namespace CompilationPrinciple {
    internal class LL1SyntaxAnalysis {
        public LL1SyntaxAnalysis(List<Token> tokenList) {
            // get tokenlist from scanner
            this.tokenList = tokenList;

            //cheate the root of syntax tree
            root = new SyntaxTreeNode(NodeKind.ProK);
            symbolStack = new Stack<LexType>();
            syntaxTreeStack = new Stack<SyntaxTreeNode>();
            opStack = new Stack<SyntaxTreeNode>();
            numStack = new Stack<SyntaxTreeNode>();

            //initialize three childs of root
            root.child[0] = new SyntaxTreeNode();
            root.child[1] = new SyntaxTreeNode();
            root.child[2] = new SyntaxTreeNode();

            //pressing into the syntax tree stack in reverse order
            syntaxTreeStack.Push(root.child[2]);
            syntaxTreeStack.Push(root.child[1]);
            syntaxTreeStack.Push(root.child[0]);
        }

        List<Token> tokenList;

        //the root of syntax tree
        public SyntaxTreeNode root;

        //save the node poped for syntax stack
        SyntaxTreeNode currentP;
        SyntaxTreeNode varibleNode;
        SyntaxTreeNode saveP;
        Stack<LexType> symbolStack;

        //Generate syntax trees for declaration and statement parts
        Stack<SyntaxTreeNode> syntaxTreeStack;

        //Generate the expression part
        Stack<SyntaxTreeNode> opStack;
        Stack<SyntaxTreeNode> numStack;

        //used for bracket match
        int expflag = 0;

        bool getExpresult = true;
        bool getExpresult2 = false;

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
                    symbolStack.Push(LexType.DOT);
                    symbolStack.Push(LexType.ProgramBody);
                    symbolStack.Push(LexType.DeclarePart);
                    symbolStack.Push(LexType.ProgramHead);
                    break;
                //<ProgramHead> ::= PROGRAM ProgramName
                case 2:
                    symbolStack.Push(LexType.ProgramName);
                    symbolStack.Push(LexType.PROGRAM);
                    //the SON node of the root node point to the Program Head Node                    
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.PheadK;
                    Debug.WriteLine("pop at case 2");
                    break;
                //<ProgramName> ::= ID;
                case 3:
                    symbolStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;                    
                    currentP.idnum++;
                    break;
                //<DeclarePart> ::= TypeDec VarDec FuncDec
                case 4:
                    symbolStack.Push(LexType.ProcDec);
                    symbolStack.Push(LexType.VarDec);
                    symbolStack.Push(LexType.TypeDec);
                    break;
                //< TypeDec > ::= ε
                case 5:
                    break;
                //<TypeDec> ::= TypeDeclaration
                case 6:
                    symbolStack.Push(LexType.TypeDeclaration);
                    break;
                //<TypeDeclaration> ::= TYPE TypeDecList 
                case 7:
                    symbolStack.Push(LexType.TypeDecList);
                    symbolStack.Push(LexType.TYPE);
                    currentP = syntaxTreeStack.Pop();
                    //Type Declarations Nodes
                    currentP.nodeKind = NodeKind.TypeK;
                    currentP.sibling = new SyntaxTreeNode();
                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[0]);
                    Debug.WriteLine("pop at case 7");
                    break;
                //<TypeDecList> ::= TypeId = TypeDef ; TypeDecMore
                case 8:
                    symbolStack.Push(LexType.TypeDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.TypeName);
                    symbolStack.Push(LexType.EQ);
                    symbolStack.Push(LexType.TypeId);

                    //Create Declarations Nodes
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.DecK;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    Debug.WriteLine("pop at case 8");

                    break;
                //<TypeDecMore> ::= ε
                case 9:
                    //pop the sibling node of the last type declaration node and set null
                    syntaxTreeStack.Pop();
                    Debug.WriteLine("pop at case 9");
                    break;
                //<TypeDecMore> ::= TypeDecList 
                case 10:
                    symbolStack.Push(LexType.TypeDecList);
                    break;
                //<TypeId > ::= ID
                case 11:
                    symbolStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;
                    break;
                //<TypeDef> ::= BaseType
                case 12:
                    symbolStack.Push(LexType.BaseType);
                    break;
                //<TypeDef > ::= StructureType 
                case 13:
                    symbolStack.Push(LexType.StructureType);
                    break;
                //<TypeDef> ::= ID
                case 14:
                    symbolStack.Push(LexType.ID);
                    currentP.decKind = DecKind.IdK;
                    currentP.typeName = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    break;
                //<BaseType> ::= INTEGER 
                case 15:
                    symbolStack.Push(LexType.INTEGER_T);                    
                    if(currentP.decKind == DecKind.ArrayK ) {
                        if(currentP.attr == null) {
                            currentP.attr = new SyntaxTreeNode.Attr("array");
                        }
                        currentP.attr.arrayAttr.childType = DecKind.IntegerK;
                    } else {
                        currentP.decKind = DecKind.IntegerK;
                    }                    
                    break;
                //< BaseType > ::= CHAR
                case 16:
                    symbolStack.Push(LexType.CHAR_T);
                    currentP.decKind = DecKind.CharK;
                    break;
                //<StructureType> ::= ArrayType 
                case 17:
                    symbolStack.Push(LexType.ArrayType);
                    break;
                //<StructureType> ::= RecType
                case 18:
                    symbolStack.Push(LexType.RecType);
                    break;
                //<ArrayType> ::= ARRAY [low..top ] OF BaseType
                case 19:
                    symbolStack.Push(LexType.BaseType);
                    symbolStack.Push(LexType.OF);
                    symbolStack.Push(LexType.RMIDPAREN);
                    symbolStack.Push(LexType.Top);
                    symbolStack.Push(LexType.UNDERRANGE);
                    symbolStack.Push(LexType.Low);
                    symbolStack.Push(LexType.LMIDPAREN);
                    symbolStack.Push(LexType.ARRAY);

                    currentP.decKind = DecKind.ArrayK;
                    break;
                //<Low> ::= INTC
                case 20:
                    symbolStack.Push(LexType.INTC_VAL);
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("array");
                    }
                    currentP.attr.arrayAttr.low = int.Parse(tokenList.First().sem);
                    currentP.lineno = tokenList.First().line;
                    break;
                //<Top> ::= INTC
                case 21:
                    symbolStack.Push(LexType.INTC_VAL);
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("array");
                    }
                    currentP.attr.arrayAttr.up = int.Parse(tokenList.First().sem);
                    currentP.lineno = tokenList.First().line;
                    break;
                //<RecType > ::= RECORD FieldDecList END
                case 22:
                    symbolStack.Push(LexType.END);
                    symbolStack.Push(LexType.FieldDecList);
                    symbolStack.Push(LexType.RECORD);

                    currentP.decKind = DecKind.RecordK;

                    saveP = currentP;
                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<FieldDecList > ::= BaseType IdList ; FieldDecMore 
                case 23:
                    symbolStack.Push(LexType.FieldDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.IdList);
                    symbolStack.Push(LexType.BaseType);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.DecK;

                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);

                    Debug.WriteLine("pop at case 23");
                    break;
                //<FieldDecList> ::= ArrayType IdList ; FieldDecMore
                case 24:
                    symbolStack.Push(LexType.FieldDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.IdList);
                    symbolStack.Push(LexType.ArrayType);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.DecK;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    Debug.WriteLine("pop at case 24");
                    break;
                //<FieldDecMore > ::= ε
                case 25:
                    syntaxTreeStack.Pop();
                    Debug.WriteLine("pop at case 25");
                    currentP = saveP;
                    break;
                //<FieldDecMore> ::= FieldDecList
                case 26:
                    symbolStack.Push(LexType.FieldDecList);
                    break;
                //<IdList > ::= id IdMore
                case 27:
                    symbolStack.Push(LexType.IdMore);
                    symbolStack.Push(LexType.ID);
                    currentP.name[currentP.idnum] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;
                    break;
                //<IdMore > ::= ε
                case 28:
                    break;
                //<IdMore > ::= , IdList 
                case 29:
                    symbolStack.Push(LexType.IdList);
                    symbolStack.Push(LexType.COMMA);
                    break;
                //<VarDec> ::= ε
                case 30:
                    break;
                //<VarDec > ::= VarDeclaration
                case 31:
                    symbolStack.Push(LexType.VarDeclaration);
                    break;
                //<VarDeclaration> ::= VAR VarDecList 
                case 32:
                    symbolStack.Push(LexType.VarDecList);
                    symbolStack.Push(LexType.VAR);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.VarK;
                    currentP.lineno = tokenList.First().line;
                    currentP.sibling = new SyntaxTreeNode();
                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[0]);
                    Debug.WriteLine("pop at case 32");
                    break;
                //<VarDecList > ::= TypeDef VarIdList ; VarDecMore
                case 33:
                    symbolStack.Push(LexType.VarDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.VarIdList);
                    symbolStack.Push(LexType.TypeName);
                    currentP = syntaxTreeStack.Pop();
                    currentP.sibling = new SyntaxTreeNode();                    
                    currentP.nodeKind = NodeKind.DecK;
                    syntaxTreeStack.Push(currentP.sibling);
                    Debug.WriteLine("pop at case 33");
                    break;
                //< VarDecMore > ::= ε
                case 34:
                    syntaxTreeStack.Pop();
                    break;
                //< VarDecMore > ::= VarDecList
                case 35:
                    symbolStack.Push(LexType.VarDecList);
                    break;
                //<VarIdList > ::= ID VarIdMore 
                case 36:
                    symbolStack.Push(LexType.VarIdMore);
                    symbolStack.Push(LexType.ID);

                    currentP.name[currentP.idnum] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;
                    break;
                //<VarIdMore > ::= ε
                case 37:
                    break;
                //<VarIdMore > ::= , VarIdList 
                case 38:
                    symbolStack.Push(LexType.VarIdList);
                    symbolStack.Push(LexType.COMMA);
                    break;
                //<ProcDec> ::= ε
                case 39:
                    break;
                //<ProcDec> ::= ProcDeclaration
                case 40:
                    symbolStack.Push(LexType.ProcDeclaration);
                    break;
                /*<ProcDeclaration> ::= PROCEDURE 
                ProcName(ParamList) : BaseType;
                ProcDecPart
                ProcBody ProcDecMore*/
                case 41:
                    symbolStack.Push(LexType.ProcDecMore);
                    symbolStack.Push(LexType.ProcBody);
                    symbolStack.Push(LexType.ProcDecPart);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.RPAREN);
                    symbolStack.Push(LexType.ParamList);
                    symbolStack.Push(LexType.LPAREN);
                    symbolStack.Push(LexType.ProcName);
                    symbolStack.Push(LexType.PROCEDURE);

                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.ProcDecK;
                    currentP.sibling = new SyntaxTreeNode();
                    currentP.child[0] = new SyntaxTreeNode();
                    currentP.child[1] = new SyntaxTreeNode();
                    currentP.child[2] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[2]);
                    syntaxTreeStack.Push(currentP.child[1]);
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<ProcDecMore> ::= ε
                case 42:
                    break;
                //<ProcDecMore> ::= ProcDeclaration
                case 43:
                    symbolStack.Push(LexType.ProcDeclaration);
                    break;
                //<ProcName> ::= id
                case 44:
                    symbolStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;
                    break;
                //<ParamList> ::= ε
                case 45:
                    syntaxTreeStack.Pop();
                    break;
                //<ParamList> ::= ParamDecList 
                case 46:
                    symbolStack.Push(LexType.ParamDecList);
                    break;
                //<ParamDecList> ::= Param ParamMore
                case 47:
                    symbolStack.Push(LexType.ParamMore);
                    symbolStack.Push(LexType.Param);
                    break;
                //<ParamMore> ::= ε
                case 48:
                    syntaxTreeStack.Pop();
                    currentP = saveP;
                    //TODO : TEMP assiignment is used to store the address of a member of a function 
                    break;
                //<ParamMore> ::= ; ParamDecList
                case 49:
                    symbolStack.Push(LexType.ParamDecList);
                    symbolStack.Push(LexType.SEMI);
                    break;
                //<Param> ::= TypeDef FormList 
                case 50:
                    symbolStack.Push(LexType.FormList);
                    //there is no typedef in LexType
                    symbolStack.Push(LexType.TypeName);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.DecK;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("proc");
                    }
                    currentP.attr.procAttr.paramt = SyntaxTreeNode.Attr.ProcAttr.ParamType.Valparamtype;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Param> ::= VAR TypeDef FormList
                case 51:
                    symbolStack.Push(LexType.FormList);
                    symbolStack.Push(LexType.TypeName);
                    symbolStack.Push(LexType.VAR);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.DecK;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("proc");
                    }
                    currentP.attr.procAttr.paramt = SyntaxTreeNode.Attr.ProcAttr.ParamType.Varparamtype;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<FormList> ::= ID FidMore 
                case 52:
                    symbolStack.Push(LexType.FidMore);
                    symbolStack.Push(LexType.ID);
                    currentP.name[currentP.idnum] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;
                    break;
                //<FidMore> ::= ε 
                case 53:
                    break;
                //<FidMore> ::= , FormList 
                case 54:
                    symbolStack.Push(LexType.FormList);
                    symbolStack.Push(LexType.COMMA);
                    break;
                //<ProcDecPart> ::= DeclarePart 
                case 55:
                    symbolStack.Push(LexType.DeclarePart);
                    break;
                //<ProcBody> ::= ProgramBody 
                case 56:
                    symbolStack.Push(LexType.ProgramBody);
                    break;
                //<ProgramBody> ::= BEGIN StmList END 
                case 57:
                    symbolStack.Push(LexType.END);
                    symbolStack.Push(LexType.StmList);
                    symbolStack.Push(LexType.BEGIN);
                    syntaxTreeStack.Pop();
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind =NodeKind.StmLK;
                    currentP.lineno = tokenList.First().line;
                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<StmList> ::= Stm StmMore
                case 58:
                    symbolStack.Push(LexType.StmMore);
                    symbolStack.Push(LexType.Stm);
                    break;
                //<StmMore> ::= ε
                case 59:
                    syntaxTreeStack.Pop();
                    break;
                //< StmMore > ::= ; StmList
                case 60:
                    symbolStack.Push(LexType.StmList);
                    symbolStack.Push(LexType.SEMI);
                    break;
                //<Stm> ::= ConditionalStm
                case 61:
                    symbolStack.Push(LexType.ConditionalStm);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.StmtK;
                    currentP.stmtKind = StmtKind.IfK;
                    currentP.lineno = tokenList.First().line;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= LoopStm
                case 62:
                    symbolStack.Push(LexType.LoopStm);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.StmtK;
                    currentP.stmtKind = StmtKind.WhileK;
                    currentP.lineno = tokenList.First().line;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= InputStm
                case 63:
                    symbolStack.Push(LexType.InputStm);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.StmtK;
                    currentP.stmtKind = StmtKind.ReadK;
                    currentP.lineno = tokenList.First().line;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= OutputStm 
                case 64:
                    symbolStack.Push(LexType.OutputStm);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.StmtK;
                    currentP.stmtKind = StmtKind.WriteK;
                    currentP.lineno = tokenList.First().line;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    //currentP.sibling = new SyntaxTreeNode();
                    break;
                //< Stm > ::= ReturnStm
                case 65:
                    symbolStack.Push(LexType.ReturnStm);
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.StmLK;
                    currentP.stmtKind = StmtKind.ReturnK;
                    currentP.lineno = tokenList.First().line;
                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= id AssCall
                case 66:
                    symbolStack.Push(LexType.AssCall);
                    symbolStack.Push(LexType.ID);

                    //Create a variable expression node that Record the left part of the assignment
                    SyntaxTreeNode variableExpression = new SyntaxTreeNode(NodeKind.ExpK);
                    variableExpression.expKind = ExpKind.IdK;
/*                    if(variableExpression.attr == null) {
                        variableExpression.attr = new SyntaxTreeNode.Attr("exp");
                    }                    
                    variableExpression.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;*/
                    variableExpression.name[0] = tokenList.First().sem;
                    variableExpression.lineno = tokenList.First().line;
                    variableExpression.idnum++;
                    
                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.StmtK;
                    currentP.child[0] = variableExpression;

                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<AssCall> ::= AssignmentRest
                case 67:
                    symbolStack.Push(LexType.AssignmentRest);
                    currentP.stmtKind = StmtKind.AssignK;
                    currentP.lineno = tokenList.First().line;
                    break;
                //<AssCall> ::= CallStmRest
                case 68:
                    symbolStack.Push(LexType.CallStmRest);
                    if (currentP.child[0].attr == null) {
                        currentP.child[0].attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.child[0].attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    currentP.stmtKind = StmtKind.CallK;
                    currentP.lineno = tokenList.First().line;
                    break;
                //<AssignmentRest> ::= VariMore := Exp 
                case 69:
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.ASSIGN);
                    symbolStack.Push(LexType.VariMore);

                    //Assign right child node pointer to the stack
                    currentP.child[1] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[1]);
                    //Change current node pointer to assignment Right child node                  
                    currentP = currentP.child[0];

                    //Pressing in special stack bottom flags
                    SyntaxTreeNode specialFlags = new SyntaxTreeNode(NodeKind.ExpK);
                    specialFlags.expKind = ExpKind.OpK;
                    if (specialFlags.attr == null) {
                        specialFlags.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    specialFlags.attr.expAttr.op = "END";
                    opStack.Push(specialFlags);
                    break;
                //<ConditionalStm> ::= IF RelExp THEN StmList ELSE StmList FI 
                case 70:
                    symbolStack.Push(LexType.FI);
                    symbolStack.Push(LexType.StmList);
                    symbolStack.Push(LexType.ELSE);
                    symbolStack.Push(LexType.StmList);
                    symbolStack.Push(LexType.THEN);
                    symbolStack.Push(LexType.RelExp);
                    symbolStack.Push(LexType.IF);

                    currentP.child[0] = new SyntaxTreeNode();
                    currentP.child[1] = new SyntaxTreeNode();
                    currentP.child[2] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[2]);
                    syntaxTreeStack.Push(currentP.child[1]);
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<LoopStm> ::= WHILE RelExp DO StmList ENDWH
                case 71:
                    symbolStack.Push(LexType.ENDWH);
                    symbolStack.Push(LexType.StmList);
                    symbolStack.Push(LexType.DO);
                    symbolStack.Push(LexType.RelExp);
                    symbolStack.Push(LexType.WHILE);


                    currentP.child[1] = new SyntaxTreeNode();
                    currentP.child[0] = new SyntaxTreeNode();

                    syntaxTreeStack.Push(currentP.child[1]);
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<InputStm> ::= READ ( InVar ) 
                case 72:
                    symbolStack.Push(LexType.RPAREN);
                    symbolStack.Push(LexType.InVar);
                    symbolStack.Push(LexType.LPAREN);
                    symbolStack.Push(LexType.READ);
                    break;
                //< InVar > ::= ID
                case 73:
                    symbolStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;
                    break;
                //<OutputStm> ::= WRITE( Exp ) 
                case 74:
                    symbolStack.Push(LexType.RPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LPAREN);
                    symbolStack.Push(LexType.WRITE);

                    //press the output statement's first son node pointer into syntax tree stack
                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[0]);
                    //self add .
                    currentP = currentP.child[0];

                    //Pressing in a special operator, giving it the lowest priority
                    specialFlags = new SyntaxTreeNode(NodeKind.ExpK);
                    specialFlags.expKind = ExpKind.OpK;
                    if (specialFlags.attr == null) {
                        specialFlags.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    specialFlags.attr.expAttr.op = "END";
                    opStack.Push(specialFlags);
                    break;
                //<ReturnStm> ::= RETURN
                case 75:
                    symbolStack.Push(LexType.RETURN);
                    break;
                //<CallStmRest> ::= ( ActParamList )
                case 76:
                    symbolStack.Push(LexType.RPAREN);
                    symbolStack.Push(LexType.ActParamList);
                    symbolStack.Push(LexType.LPAREN);

                    currentP.child[1] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[1]);
                    break;
                //<ActParamList> ::= ε
                case 77:
                    syntaxTreeStack.Pop();
                    break;
                //<ActParamList> ::= Exp ActParamMore
                case 78:
                    symbolStack.Push(LexType.ActParamMore);
                    symbolStack.Push(LexType.Exp);
                    specialFlags = new SyntaxTreeNode(NodeKind.ExpK);
                    specialFlags.expKind = ExpKind.OpK;
                    if (specialFlags.attr == null) {
                        specialFlags.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    specialFlags.attr.expAttr.op = "END";
                    opStack.Push(specialFlags);
                    break;
                //<ActParamMore> ::= ε
                case 79:
                    break;
                //<ActParamMore> ::= , ActParamList
                case 80:
                    symbolStack.Push(LexType.ActParamList);
                    symbolStack.Push(LexType.COMMA);

                    currentP.sibling = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<RelExp> ::= Exp OtherRelE 
                case 81:
                    symbolStack.Push(LexType.OtherRelE);
                    symbolStack.Push(LexType.Exp);

                    specialFlags = new SyntaxTreeNode(NodeKind.ExpK);
                    specialFlags.expKind = ExpKind.OpK;
                    if (specialFlags.attr == null) {

                        specialFlags.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    specialFlags.attr.expAttr.op = "END";
                    opStack.Push(specialFlags);
                    getExpresult = false;
                    break;
                //<OtherRelE> ::= CmpOp Exp
                case 82:
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.CmpOp);
                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.OpK;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    currentP.lineno = tokenList.First().line;
                    LexType sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    //Compare the priority of the top-of-stack operator and this operator
                    while (priosity(sTop) >= priosity(tokenList.First().lex)) {
                        currentP = opStack.Pop();
                        SyntaxTreeNode Rnum = numStack.Pop();
                        SyntaxTreeNode Lnum = numStack.Pop();
                        currentP.child[1] = Rnum;
                        currentP.child[0] = Lnum;
                        numStack.Push(currentP);
                        sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    }
                    opStack.Push(currentP);
                    getExpresult = true;
                    break;
                //<Exp> ::= Term OtherTerm
                case 83:
                    symbolStack.Push(LexType.OtherTerm);
                    symbolStack.Push(LexType.Term);
                    break;
                //<OtherTerm> ::= ε
                case 84:
                    // if expflag is not zero ,indicating that it is preceded by paired with a left bracket
                    if (tokenList.First().lex == LexType.RPAREN && expflag != 0) {
                        while ((LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op)
                            != LexType.LPAREN) {
                            //Pops up a contents of the operator stack
                            SyntaxTreeNode topOperator = opStack.Pop();
                            //Pops up tow numbers of the number stack
                            SyntaxTreeNode Rnum = numStack.Pop();
                            SyntaxTreeNode Lnum = numStack.Pop();

                            topOperator.child[1] = Rnum;
                            topOperator.child[0] = Lnum;
                            numStack.Push(topOperator);
                        }
                        //Pops up the left bracket
                        opStack.Pop();
                        expflag--;
                    } else {
                        if (getExpresult == true || getExpresult2 == true) {
                            while ((LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op)
                                != LexType.END) {
                                //Pops up a contents of the operator stack
                                SyntaxTreeNode topOperator = opStack.Pop();
                                //Pops up tow numbers of the number stack
                                SyntaxTreeNode Rnum = numStack.Pop();
                                SyntaxTreeNode Lnum = numStack.Pop();

                                topOperator.child[1] = Rnum;
                                topOperator.child[0] = Lnum;
                                numStack.Push(topOperator);
                            }
                            //Pop up the bottom of the stack flag
                            opStack.Pop();
                            currentP = syntaxTreeStack.Pop();
                            currentP.DeepCopy(numStack.Pop());


                            /*                            if (getExpresult2 == true) {
                                                            currentP.child[0] = syntaxTreeStack.Pop();
                                                            currentP.child[0].deepCopy(numStack.Pop());
                                                            getExpresult2 = false;
                                                        } else {
                                                            currentP = syntaxTreeStack.Pop();
                                                            currentP.deepCopy(numStack.Pop());
                                                        }*/
                            if (getExpresult2 == true) {                                
                                getExpresult2 = false;
                            }
                        }
                    }
                    break;
                //<OtherTerm> ::= AddOp Exp
                case 85:
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.AddOp);

                    SyntaxTreeNode opAddNode = new SyntaxTreeNode(NodeKind.ExpK);
                    opAddNode.expKind = ExpKind.OpK;
                    if (opAddNode.attr == null) {
                        opAddNode.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    opAddNode.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    opAddNode.lineno = tokenList.First().line;
                    sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    while (priosity(sTop) >= priosity(tokenList.First().lex)) {
                        SyntaxTreeNode temp = opStack.Pop();
                        SyntaxTreeNode Rnum = numStack.Pop();
                        SyntaxTreeNode Lnum = numStack.Pop();
                        temp.child[1] = Rnum;
                        temp.child[0] = Lnum;
                        numStack.Push(temp);
                        sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    }
                    opStack.Push(opAddNode);
                    break;
                //<Term> ::= Factor OtherFactor 
                case 86:
                    symbolStack.Push(LexType.OtherFactor);
                    symbolStack.Push(LexType.Factor);
                    break;
                //<OtherFactor> ::= ε
                case 87:
                    break;
                //<OtherFactor> ::= MultOp Term 
                case 88:
                    symbolStack.Push(LexType.Term);
                    symbolStack.Push(LexType.MultOp);

                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.OpK;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    currentP.lineno = tokenList.First().line;
                    sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    while (priosity(sTop) >= priosity(tokenList.First().lex)) {
                        SyntaxTreeNode temp = opStack.Pop();
                        SyntaxTreeNode Rnum = numStack.Pop();
                        SyntaxTreeNode Lnum = numStack.Pop();
                        temp.child[1] = Rnum;
                        temp.child[0] = Lnum;
                        numStack.Push(temp);
                        sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    }
                    opStack.Push(currentP);
                    break;
                //<Factor> ::= ( Exp ) 
                case 89:
                    symbolStack.Push(LexType.RPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LPAREN);
                    //Push the left bracket into the stack
                    SyntaxTreeNode leftBracketNode = new SyntaxTreeNode(NodeKind.ExpK);
                    leftBracketNode.expKind = ExpKind.OpK;
                    if (leftBracketNode.attr == null) {
                        leftBracketNode.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    leftBracketNode.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    leftBracketNode.lineno = tokenList.First().line;
                    opStack.Push(leftBracketNode);
                    expflag++;
                    break;
                //<Factor> ::= INTC 
                case 90:
                    symbolStack.Push(LexType.INTC_VAL);
                    SyntaxTreeNode numNode = new SyntaxTreeNode();
                    numNode.nodeKind = NodeKind.ExpK;
                    numNode.expKind = ExpKind.ConstK;
                    if (numNode.attr == null) {
                        numNode.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    numNode.attr.expAttr.val = int.Parse(tokenList.First().sem);
                    numNode.lineno = tokenList.First().line;

                    numStack.Push(numNode);
                    break;
                //<Factor> ::= Variable 
                case 91:
                    symbolStack.Push(LexType.Variable);
                    break;
                //<Variable> ::= ID VariMore 
                case 92:
                    symbolStack.Push(LexType.VariMore);
                    symbolStack.Push(LexType.ID);
                    /*             varibleNode = new SyntaxTreeNode(NodeKind.ExpK);
                                 varibleNode.expKind = ExpKind.IdK;
                                 varibleNode.name[0] = tokenList.First().sem;
                                 if (varibleNode.attr == null) {
                                     varibleNode.attr = new SyntaxTreeNode.Attr("exp");
                                     varibleNode.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                                 }
                                 varibleNode.idnum++;
                                 numStack.Push(varibleNode);*/
                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.IdK;
                    currentP.name[0] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                        currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    }
                    currentP.idnum++;
                    numStack.Push(currentP);
                    break;
                //<VariMore> ::= ε
                case 93:
                   /* if (varibleNode != null && varibleNode.attr == null) {
                        varibleNode.expKind = ExpKind.IdK;
                        varibleNode.attr = new SyntaxTreeNode.Attr("exp");
                        varibleNode.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    }*/
                    if(currentP.attr == null && (currentP.nodeKind == null || currentP.nodeKind == NodeKind.ExpK)) {
                        currentP.nodeKind = NodeKind.ExpK;
                        currentP.expKind = ExpKind.IdK;
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    if (currentP.attr != null && (currentP.attr.expAttr.varKind == null || currentP.attr.expAttr.varKind == VarKind.Error)) {
                        currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    }
                    break;
                //<VariMore> ::= [ Exp ]
                case 94:
                    symbolStack.Push(LexType.RMIDPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LMIDPAREN);

                    //currentP.child[0] = syntaxTreeStack.Pop();
                    //currentP = currentP.child[0];
                    currentP.nodeKind = NodeKind.ExpK;
                    currentP.expKind = ExpKind.IdK;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.ArrayMembV;
                    //syntaxTreeStack.Push(currentP);
                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[0]);

                    SyntaxTreeNode operatorNode = new SyntaxTreeNode(NodeKind.ExpK);
                    operatorNode.expKind = ExpKind.OpK;
                    if (operatorNode.attr == null) {
                        operatorNode.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    operatorNode.attr.expAttr.op = "END";
                    opStack.Push(operatorNode);
                    getExpresult2 = true;
                    break;
                //<VariMore> ::= . FieldVar
                case 95:
                    symbolStack.Push(LexType.FieldVar);
                    symbolStack.Push(LexType.DOT);
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.FieldMembV;

                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //< FieldVar > ::= ID FieldVarMore
                case 96:
                    symbolStack.Push(LexType.FieldVarMore);
                    symbolStack.Push(LexType.ID);

                    currentP = syntaxTreeStack.Pop();
                    currentP.nodeKind = NodeKind.ExpK;
                    currentP.expKind = ExpKind.IdK;
                    currentP.name[0] = tokenList.First().sem;
                    currentP.lineno = tokenList.First().line;
                    currentP.idnum++;

                    break;
                //<FieldVarMore> ::= ε
                case 97:
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    currentP.lineno = tokenList.First().line;
                    break;
                //<FieldVarMore> ::= [ Exp ]
                case 98:
                    symbolStack.Push(LexType.RMIDPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LMIDPAREN);
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.ArrayMembV;

                    currentP.child[0] = new SyntaxTreeNode();
                    syntaxTreeStack.Push(currentP.child[0]);

                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.OpK;
                    if (currentP.attr == null) {
                        currentP.attr = new SyntaxTreeNode.Attr("exp");
                    }
                    currentP.attr.expAttr.op = "END";
                    opStack.Push(currentP);
                    getExpresult2 = true;
                    break;
                //<CmpOp> ::= LT
                case 99:
                    symbolStack.Push(LexType.LT);
                    break;
                //<CmpOp> ::= EQ
                case 100:
                    symbolStack.Push(LexType.EQ);
                    break;
                //<AddOp> ::= PLUS
                case 101:
                    symbolStack.Push(LexType.PLUS);
                    break;
                //<AddOp> ::= MINUS
                case 102:
                    symbolStack.Push(LexType.MINUS);
                    break;
                //<MultOp> ::= TIMES
                case 103:
                    symbolStack.Push(LexType.TIMES);
                    break;
                //there is no <MultOp> ::= OVER but <MultOp> ::= DIVIDE
                case 104:
                    symbolStack.Push(LexType.DIVIDE);
                    break;
            }
        }
        //the main function of LL1 syntax analysis , returns the generated syntax analysis tree 
        public void parse() {

            bool worng = false;

            //bring in the classified symbols
            ClassifiedSymbos classifiedSymbos = new ClassifiedSymbos();

            //bring in the ll1 table
            LL1Supporter lL1Supporter = new LL1Supporter();

            //push the start symbal
            symbolStack.Push(LexType.Program);

            //when the symbal stack is not empty
            while (symbolStack.Count != 0) {
                if (tokenList.Count == 0) {
                    Console.WriteLine("Tokenlist is empty !");
                }
                String symbolName = Enum.GetName(typeof(LexType), symbolStack.Peek());
                //if the top of symbal stack is a non-ultimate                
                if (classifiedSymbos.isInNonUtimate(symbolName)) {
                    LexType lexType = symbolStack.Pop();
                    Token token = tokenList.First();
                    int generNumber = lL1Supporter.LL1Table[(int)lexType, (int)token.lex];
                    // the conversion is legal
                    if (generNumber >= 1 && generNumber <= 104) {
                        process(generNumber);
                    } else {
                        Console.WriteLine("there is not a convert from : " + lexType + "to :" + Enum.GetName(typeof(LexType), token.lex));
                        worng = true;
                    }
                    continue;
                }
                //if the top of symbal stack is a ultimate
                if (classifiedSymbos.isInUtimate(symbolName)) {
                    LexType lexType = (LexType)symbolStack.Peek();
                    Token token = tokenList.First();
                    if (lexType == token.lex) {
                        // match !!
                        symbolStack.Pop();
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