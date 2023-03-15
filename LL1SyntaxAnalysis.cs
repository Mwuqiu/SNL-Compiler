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

            root.child[0] = new SyntaxTreeNode();
            root.child[1] = new SyntaxTreeNode();
            root.child[2] = new SyntaxTreeNode();

            //pressing into the syntax tree stack in reverse order
            syntaxTreeStack.Push(root.child[2]);
            syntaxTreeStack.Push(root.child[1]);
            syntaxTreeStack.Push(root.child[0]);
        }

        List<Token> tokenList;
        SyntaxTreeNode root;
        SyntaxTreeNode currentP;
        SyntaxTreeNode saveP;
        Stack<LexType> symbolStack;// token sequencce
        Stack<SyntaxTreeNode> syntaxTreeStack; //Generate syntax trees for declaration and statement parts        
        Stack<SyntaxTreeNode> opStack; //Generate the expression part
        Stack<SyntaxTreeNode> numStack;

        //used for bracket match
        int expflag = 0;


        bool getExpresult = false;
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
                    symbolStack.Push(LexType.DOT);
                    symbolStack.Push(LexType.ProgramBody);
                    symbolStack.Push(LexType.DeclarePart);
                    symbolStack.Push(LexType.ProgramHead);
                    break;
                //<ProgramHead> ::= PROGRAM ProgramName
                case 2:
                    symbolStack.Push(LexType.ProgramName);
                    symbolStack.Push(LexType.PROGRAM);
                    //create program header node
                    currentP = new SyntaxTreeNode(NodeKind.PheadK);

                    //the SON node of the root node point to the Program Head Node                    
                    SyntaxTreeNode topNode = syntaxTreeStack.Pop();
                    Debug.WriteLine("pop at case 2");
                    topNode.deepCopy(currentP);
                    break;
                //<ProgramName> ::= ID;
                case 3:
                    symbolStack.Push(LexType.ID);
                    currentP.name[0] = tokenList.First().sem;
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

                    //Create Type Declarations Nodes
                    currentP = new SyntaxTreeNode(NodeKind.TypeK);
                    topNode = syntaxTreeStack.Pop();
                    topNode.deepCopy(currentP);

                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<TypeDecList> ::= TypeId = TypeDef ; TypeDecMore
                case 8:
                    symbolStack.Push(LexType.TypeDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.TypeName);
                    symbolStack.Push(LexType.EQ);
                    symbolStack.Push(LexType.TypeId);

                    //Create Declarations Nodes
                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    topNode = syntaxTreeStack.Pop();
                    topNode.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<TypeDecMore> ::= ε
                case 9:
                    //pop the sibling node of the last type declaration node , complete the type section
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
                    break;
                //<BaseType> ::= INTEGER 
                case 15:
                    symbolStack.Push(LexType.INTEGER_T);
                    currentP.decKind = DecKind.IntegerK;
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
                    currentP.attr.arrayAttr.low = int.Parse(tokenList.First().sem);
                    break;
                //<Top> ::= INTC
                case 21:
                    symbolStack.Push(LexType.INTC_VAL);
                    currentP.attr.arrayAttr.up = int.Parse(tokenList.First().sem);
                    break;
                //<RecType > ::= RECORD FieldDecList END
                case 22:
                    symbolStack.Push(LexType.END);
                    symbolStack.Push(LexType.FieldDecList);
                    symbolStack.Push(LexType.RECORD);

                    currentP.decKind = DecKind.RecordK;

                    saveP = currentP;

                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<FieldDecList > ::= BaseType IdList ; FieldDecMore 
                case 23:
                    symbolStack.Push(LexType.FieldDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.IdList);
                    symbolStack.Push(LexType.BaseType);

                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    SyntaxTreeNode temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);
                    break;
                //<FieldDecList> ::= ArrayType IdList ; FieldDecMore
                case 24:
                    symbolStack.Push(LexType.FieldDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.IdList);
                    symbolStack.Push(LexType.ArrayType);
                    SyntaxTreeNode t = syntaxTreeStack.Pop();
                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    t.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
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

                    currentP = new SyntaxTreeNode(NodeKind.VarK);
                    temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //<VarDecList > ::= TypeDef VarIdList ; VarDecMore
                case 33:
                    symbolStack.Push(LexType.VarDecMore);
                    symbolStack.Push(LexType.SEMI);
                    symbolStack.Push(LexType.VarIdList);
                    symbolStack.Push(LexType.TypeName);

                    temp = syntaxTreeStack.Pop();
                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
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

                    currentP = new SyntaxTreeNode(NodeKind.ProcDecK);
                    temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    syntaxTreeStack.Push(currentP.child[0]);
                    syntaxTreeStack.Push(currentP.child[1]);
                    syntaxTreeStack.Push(currentP.child[2]);
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
                    temp = syntaxTreeStack.Pop();

                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    currentP.attr.procAttr.paramt = "valparamType";

                    temp.deepCopy(currentP);

                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Param> ::= VAR TypeDef FormList
                case 51:
                    symbolStack.Push(LexType.FormList);
                    symbolStack.Push(LexType.TypeName);
                    symbolStack.Push(LexType.VAR);

                    temp = syntaxTreeStack.Pop();

                    currentP = new SyntaxTreeNode(NodeKind.DecK);
                    currentP.attr.procAttr.paramt = "varparamType";
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);

                    break;
                //<FormList> ::= ID FidMore 
                case 52:
                    symbolStack.Push(LexType.FidMore);
                    symbolStack.Push(LexType.ID);

                    currentP.name[currentP.idnum] = tokenList.First().sem;
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

                    temp = syntaxTreeStack.Pop();
                    currentP = new SyntaxTreeNode(NodeKind.StmLK);
                    temp.deepCopy(currentP);
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
                    currentP = new SyntaxTreeNode(NodeKind.StmtK);
                    currentP.stmtKind = StmtKind.IfK;

                    temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= LoopStm
                case 62:
                    symbolStack.Push(LexType.LoopStm);
                    currentP = new SyntaxTreeNode(NodeKind.StmtK);
                    currentP.stmtKind = StmtKind.WhileK;
                    temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= InputStm
                case 63:
                    symbolStack.Push(LexType.InputStm);
                    temp = syntaxTreeStack.Pop();
                    currentP = new SyntaxTreeNode(NodeKind.StmLK);
                    currentP.stmtKind = StmtKind.ReadK;
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= OutputStm 
                case 64:
                    symbolStack.Push(LexType.OutputStm);
                    temp = syntaxTreeStack.Pop();
                    currentP = new SyntaxTreeNode(NodeKind.StmLK);
                    currentP.stmtKind = StmtKind.WriteK;
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //< Stm > ::= ReturnStm
                case 65:
                    symbolStack.Push(LexType.ReturnStm);
                    temp = syntaxTreeStack.Pop();
                    currentP = new SyntaxTreeNode(NodeKind.StmLK);
                    currentP.stmtKind = StmtKind.ReturnK;
                    temp.deepCopy(currentP);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<Stm> ::= id AssCall
                case 66:
                    symbolStack.Push(LexType.AssCall);
                    symbolStack.Push(LexType.ID);

                    currentP = new SyntaxTreeNode(NodeKind.StmtK);
                    t = new SyntaxTreeNode(NodeKind.ExpK);
                    t.expKind = ExpKind.IdK;
                    t.name[0] = tokenList.First().sem;
                    t.idnum++;

                    currentP.child[0] = t;
                    temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);

                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<AssCall> ::= AssignmentRest
                case 67:
                    symbolStack.Push(LexType.AssignmentRest);
                    currentP.stmtKind = StmtKind.AssignK;
                    break;
                //<AssCall> ::= CallStmRest
                case 68:
                    symbolStack.Push(LexType.CallStmRest);
                    currentP.child[0].attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    currentP.stmtKind = StmtKind.CallK;
                    break;
                //<AssignmentRest> ::= VariMore := Exp 
                case 69:
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.ASSIGN);
                    symbolStack.Push(LexType.VariMore);

                    syntaxTreeStack.Push(currentP.child[1]);
                    currentP = currentP.child[0];

                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;

                    temp.attr.expAttr.op = "END";
                    opStack.Push(temp);
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
                    currentP.idnum++;
                    break;
                //<OutputStm> ::= WRITE( Exp ) 
                case 74:
                    symbolStack.Push(LexType.RPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LPAREN);
                    symbolStack.Push(LexType.WRITE);
                    syntaxTreeStack.Push(currentP.child[0]);
                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;
                    temp.attr.expAttr.op = "End";
                    opStack.Push(temp);
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
                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;
                    temp.attr.expAttr.op = "END";
                    opStack.Push(temp);
                    break;
                //<ActParamMore> ::= ε
                case 79:
                    break;
                //<ActParamMore> ::= , ActParamList
                case 80:
                    symbolStack.Push(LexType.ActParamList);
                    symbolStack.Push(LexType.COMMA);
                    syntaxTreeStack.Push(currentP.sibling);
                    break;
                //<RelExp> ::= Exp OtherRelE 
                case 81:
                    symbolStack.Push(LexType.OtherRelE);
                    symbolStack.Push(LexType.Exp);

                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;
                    temp.attr.expAttr.op = "END";
                    opStack.Push(temp);
                    getExpresult = false;
                    break;
                //<OtherRelE> ::= CmpOp Exp
                case 82:
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.CmpOp);
                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.OpK;
                    currentP.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    LexType sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    //Compare the priority of the top-of-stack operator and this operator
                    while (priosity(sTop) >= priosity(tokenList.First().lex)) {
                        temp = opStack.Pop();
                        SyntaxTreeNode Rnum = numStack.Pop();
                        SyntaxTreeNode Lnum = numStack.Pop();
                        temp.child[1] = Rnum;
                        temp.child[0] = Lnum;
                        numStack.Push(temp);
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
                            temp = opStack.Pop();
                            //Pops up tow numbers of the number stack
                            SyntaxTreeNode Rnum = numStack.Pop();
                            SyntaxTreeNode Lnum = numStack.Pop();

                            temp.child[1] = Rnum;
                            temp.child[0] = Lnum;
                            numStack.Push(temp);
                        }
                        //Pops up the left bracket
                        opStack.Pop();
                        expflag--;
                    } else {
                        if (getExpresult == true || getExpresult2) {
                            while ((LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op)
                                != LexType.END) {
                                //Pops up a contents of the operator stack
                                temp = opStack.Pop();
                                //Pops up tow numbers of the number stack
                                SyntaxTreeNode Rnum = numStack.Pop();
                                SyntaxTreeNode Lnum = numStack.Pop();

                                temp.child[1] = Rnum;
                                temp.child[0] = Lnum;
                                numStack.Push(temp);
                            }
                            //Pop up the bottom of the stack flag
                            opStack.Pop();
                            currentP = numStack.Pop();
                            temp = syntaxTreeStack.Pop();
                            temp.deepCopy(currentP);
                            //restore the flag
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

                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.OpK;

                    currentP.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    while (priosity(sTop) >= priosity(tokenList.First().lex)) {
                        temp = opStack.Pop();
                        SyntaxTreeNode Rnum = numStack.Pop();
                        SyntaxTreeNode Lnum = numStack.Pop();
                        temp.child[1] = Rnum;
                        temp.child[0] = Lnum;
                        numStack.Push(temp);
                        sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    }
                    opStack.Push(currentP);
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

                    currentP.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    sTop = (LexType)Enum.Parse(typeof(LexType), opStack.Peek().attr.expAttr.op);
                    while (priosity(sTop) >= priosity(tokenList.First().lex)) {
                        temp = opStack.Pop();
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
                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;
                    temp.attr.expAttr.op = Enum.GetName(typeof(LexType), tokenList.First().lex);
                    opStack.Push(temp);
                    expflag++;
                    break;
                //<Factor> ::= INTC 
                case 90:
                    symbolStack.Push(LexType.INTC_VAL);
                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.ConstK;
                    temp.attr.expAttr.val = int.Parse(Enum.GetName(typeof(LexType), tokenList.First().lex));
                    numStack.Push(temp);
                    break;
                //<Factor> ::= Variable 
                case 91:
                    symbolStack.Push(LexType.Variable);
                    break;
                //<Variable> ::= ID VariMore 
                case 92:
                    symbolStack.Push(LexType.VariMore);
                    symbolStack.Push(LexType.ID);
                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.name[0] = tokenList.First().sem;
                    currentP.idnum++;
                    numStack.Push(currentP);
                    break;
                //<VariMore> ::= ε
                case 93:
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    break;
                //<VariMore> ::= [ Exp ]
                case 94:
                    symbolStack.Push(LexType.RMIDPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LMIDPAREN);
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.ArrayMembV;
                    syntaxTreeStack.Push(currentP.child[0]);
                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;
                    temp.attr.expAttr.op = "END";
                    opStack.Push(temp);
                    getExpresult2 = true;
                    break;
                //<VariMore> ::= . FieldVar
                case 95:
                    symbolStack.Push(LexType.FieldVar);
                    symbolStack.Push(LexType.DOT);
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.FieldMembV;
                    syntaxTreeStack.Push(currentP.child[0]);
                    break;
                //< FieldVar > ::= ID FieldVarMore
                case 96:
                    symbolStack.Push(LexType.FieldVarMore);
                    symbolStack.Push(LexType.ID);
                    currentP = new SyntaxTreeNode(NodeKind.ExpK);
                    currentP.expKind = ExpKind.IdK;
                    currentP.name[0] = tokenList.First().sem;
                    currentP.idnum++;
                    temp = syntaxTreeStack.Pop();
                    temp.deepCopy(currentP);
                    break;
                //<FieldVarMore> ::= ε
                case 97:
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.IdV;
                    break;
                //<FieldVarMore> ::= [ Exp ]
                case 98:
                    symbolStack.Push(LexType.RMIDPAREN);
                    symbolStack.Push(LexType.Exp);
                    symbolStack.Push(LexType.LMIDPAREN);
                    currentP.attr.expAttr.varKind = SyntaxTreeNode.Attr.ExpAttr.VarKind.ArrayMembV;
                    syntaxTreeStack.Push(currentP.child[0]);
                    temp = new SyntaxTreeNode(NodeKind.ExpK);
                    temp.expKind = ExpKind.OpK;
                    temp.attr.expAttr.op = "END";
                    opStack.Push(temp);
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
                    if (generNumber >= 1 && generNumber <= 103) {
                        process(generNumber);
                    } else {
                        Console.WriteLine("there is not a convert from : " + lexType + "to :" + Enum.GetName(typeof(LexType), token.lex));
                        worng = true;
                    }
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