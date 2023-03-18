#pragma warning disable CS8601


using Microsoft.VisualBasic;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static CompilationPrinciple.SyntaxClass;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode.Attr;



namespace CompilationPrinciple {
    /*public class SyntaxException : Exception {
        string stackTrace = "";
        public SyntaxException(string message) : base(message) {
            stackTrace = new StackTrace().ToString();
        }
    }*/
    public class SyntaxAnalysiser {
        public List<Token> tokenList;
        public int index;
        public List<string> errorList;
        public List<string> warningList;
        public SyntaxAnalysiser(List<Token> tokens) {
            tokenList = tokens;
            errorList = new List<string>();
            warningList = new List<string>();
            index = 0;
        }
        public Token GetCurrent() {
            if (index >= tokenList.Count) {
                string msg = "[Error] Type #2: Index out of range.";
                throw new Exception(msg);
            }
            return tokenList[index];
        }
        public void Match(LexType lexType) {
            if (GetCurrent().lex == lexType) {
                string res = GetCurrent().sem;
                index++;
            } else {
                string msg = "[Error] Type #1: Match Wrong at line "
                    + GetCurrent().line
                    + ", col "
                    + GetCurrent().column
                    + ". Expected lexType \'"
                    + Enum.GetName(typeof(LexType), lexType)
                    + "\', but found \'"
                    + GetCurrent().sem
                    + "\' instead.";
                errorList.Add(msg);
                index++;
            }

        }
        public SyntaxTreeNode? Parse() {
            try {
                SyntaxTreeNode? t = SyntaxProgram();
                Match(LexType.ENDFILE);
                // 如果当前和ENDFILE匹配, 则正常结束.
                // 否则报错
                return t;
            } catch (Exception e) {
                errorList.Add(e.Message);
                return null;
            }

        }

        SyntaxTreeNode? SyntaxProgram() {

            SyntaxTreeNode root = new SyntaxTreeNode(NodeKind.ProK);
            root.child[0] = ProgramHead();
            root.child[1] = DeclarePart();
            root.child[2] = ProgramBody();
            Match(LexType.DOT);
            return root;
        }
        SyntaxTreeNode ProgramHead() {
            SyntaxTreeNode pheadK = new SyntaxTreeNode(NodeKind.PheadK);
            Match(LexType.PROGRAM);
            pheadK.name[pheadK.idnum++] = GetCurrent().sem;
            Match(LexType.ID);
            return pheadK;
        }
        SyntaxTreeNode? DeclarePart() {
            /*注意, 为null不算错误*/
            SyntaxTreeNode? typeP = new SyntaxTreeNode(NodeKind.TypeK);
            SyntaxTreeNode pp = typeP;

            typeP.child[0] = TypeDec();
            if (typeP.child[0] == null)
                typeP = null;

            SyntaxTreeNode? varP = new SyntaxTreeNode(NodeKind.VarK);
            varP.child[0] = VarDec();
            if (varP.child[0] == null)
                varP = null;

            SyntaxTreeNode s = ProcDec();
            if (varP == null)
                varP = s;
            if (typeP == null)
                pp = typeP = varP;
            if (typeP != varP)
                typeP.sibling = varP;
            if (varP != s)
                varP.sibling = s;
            // 考虑typeP, varP, s为空的情形
            // 以sibling 按顺序连接非空的 typeP, varP, s
            return pp;
        }
        SyntaxTreeNode? TypeDec() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.TYPE:
                    t = TypeDeclaration();
                    break;
                case LexType.VAR:
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词，跳过此单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode? TypeDeclaration() {
            Match(LexType.TYPE);
            SyntaxTreeNode? t = TypeDecList();
            if (t == null) {
                //显示提示信息, 不是报错
                warningList.Add("[Warning] Type #3: TypeDeclaration is empty.");
            }
            return t;
        }
        public SyntaxTreeNode? TypeDecList() {
            SyntaxTreeNode decK = new SyntaxTreeNode(NodeKind.DecK);
            TypeId(decK);
            Match(LexType.EQ);
            TypeName(decK);
            Match(LexType.SEMI);
            SyntaxTreeNode? p = TypeDecMore();
            if (p != null)
                decK.sibling = p;

            return decK;
        }
        public void TypeId(SyntaxTreeNode t) {
            if (GetCurrent().lex == LexType.ID) {
                t.name[t.idnum++] = GetCurrent().sem;
            }
            Match(LexType.ID);
        }
        public void TypeName(SyntaxTreeNode t) { // TypeDef 
            if (t == null)
                return;
            switch (GetCurrent().lex) {
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                    BaseType(t);
                    break;
                case LexType.ARRAY:
                case LexType.RECORD:
                    StrutureType(t);
                    break;
                case LexType.ID:
                    t.decKind = DecKind.IdK;
                    // t.name[t.idnum++] = GetCurrent().sem;
                    t.typeName = GetCurrent().sem;
                    Match(LexType.ID);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                         + GetCurrent().line
                         + ", col "
                         + GetCurrent().column
                         + ".");
                    index++;
                    //读入下一个单词  ?
                    break;
            }
        }
        public void BaseType(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.INTEGER_T:
                    Match(LexType.INTEGER_T);
                    t.decKind = DecKind.IntegerK;
                    break;
                case LexType.CHAR_T:
                    Match(LexType.CHAR_T);
                    t.decKind = DecKind.CharK;
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
        }
        public void StrutureType(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.ARRAY:
                    ArrayType(t);
                    break;
                case LexType.RECORD:
                    t.decKind = DecKind.RecordK;
                    RecType(t);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
        }
        public void ArrayType(SyntaxTreeNode t) {
            Match(LexType.ARRAY);
            Match(LexType.LMIDPAREN);
            if (GetCurrent().lex == LexType.INTC_VAL) {
                t.attr = new SyntaxTreeNode.Attr("array");
                t.attr.arrayAttr.low = int.Parse(GetCurrent().sem);
            }
            Match(LexType.INTC_VAL);
            Match(LexType.UNDERRANGE);
            if (GetCurrent().lex == LexType.INTC_VAL) {
                t.attr.arrayAttr.up = int.Parse(GetCurrent().sem);
            }
            Match(LexType.INTC_VAL);
            Match(LexType.RMIDPAREN);
            Match(LexType.OF);
            BaseType(t);
            t.attr.arrayAttr.childType = Enum.GetName(typeof(DecKind), t.decKind);
            t.decKind = DecKind.ArrayK;
        }
        public void RecType(SyntaxTreeNode t) {
            Match(LexType.RECORD);
            SyntaxTreeNode p = FieldDecList();
            if (p != null) {
                t.child[0] = p;
                Match(LexType.END);
            } else {
                //错误信息提示
                errorList.Add("[Error] Type #4: A record body is REQUESTED at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                index++;
                Match(LexType.END);
                // throw (new Exception("错误: 在 line=" + GetCurrent().line));
            }
        }
        public SyntaxTreeNode FieldDecList() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.DecK);
            SyntaxTreeNode? p = null;
            switch (GetCurrent().lex) {
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                    BaseType(t);
                    IdList(t);
                    Match(LexType.SEMI);
                    p = FieldDecMore();
                    break;

                case LexType.ARRAY:
                    ArrayType(t);
                    IdList(t);
                    Match(LexType.SEMI);
                    p = FieldDecMore();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 (?)
                    break;
            }
            t.sibling = p;
            return t;
        }
        public void IdList(SyntaxTreeNode t) {
            if (GetCurrent().lex == LexType.ID) {
                t.name[t.idnum++] = GetCurrent().sem;
                Match(LexType.ID);
            }
            IdMore(t);
        }
        public void IdMore(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.SEMI:
                    break;
                case LexType.COMMA:
                    Match(LexType.COMMA);
                    IdList(t);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 (?)
                    break;
            }
        }
        public SyntaxTreeNode FieldDecMore() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.ID:
                    break;
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                case LexType.ARRAY:
                    t = FieldDecList();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 (?)
                    break;
            }
            return t;
        }
        public SyntaxTreeNode? TypeDecMore() {
            switch (GetCurrent().lex) {
                case LexType.ID:
                    return TypeDecList();
                case LexType.VAR:
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 (?)
                    break;
            }
            return null;
        }

        public SyntaxTreeNode VarDec() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                case LexType.VAR:
                    t = VarDeclaration();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode VarDeclaration() {
            Match(LexType.VAR);
            SyntaxTreeNode t = VarDecList();
            if (t == null) {
                // 错误提示
                errorList.Add("[Error] Type #5: A var declaration body is EXPECTED at line "
                    + GetCurrent().line
                    + ", col "
                    + GetCurrent().column
                    + ".");
            }
            return t;
        }
        public SyntaxTreeNode VarDecList() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.DecK);
            TypeName(t);
            VarIdList(t);
            Match(LexType.SEMI);
            t.sibling = VarDecMore();
            return t;
        }
        public SyntaxTreeNode VarDecMore() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                case LexType.ARRAY:
                case LexType.RECORD:
                case LexType.ID:
                    t = VarDecList();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
            return t;
        }
        public void VarIdList(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.ID:
                    t.name[t.idnum++] = GetCurrent().sem;
                    Match(LexType.ID);
                    break;
                default:
                    errorList.Add("[Error] Type #6: A var id is EXPECTED at line "
                    + GetCurrent().line
                    + ", col "
                    + GetCurrent().column
                    + ".");
                    break;
            }
            VarIdMore(t);
        }
        public void VarIdMore(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.SEMI:
                    break;
                case LexType.COMMA:
                    Match(LexType.COMMA);
                    VarIdList(t);
                    break;
                default:
                    // 读入下一个单词 ?
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    break;
            }
        }

        //   TO DO
        //   过程声明
        //   25-34
        public SyntaxTreeNode ProcDec() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.BEGIN:
                    break;
                case LexType.PROCEDURE:
                    t = ProcDeclaration();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode ProcDeclaration() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.ProcDecK);
            Match(LexType.PROCEDURE);
            if (GetCurrent().lex == LexType.ID) {
                t.name[t.idnum++] = GetCurrent().sem;
                Match(LexType.ID);
            }
            Match(LexType.LPAREN);
            ParamList(t);
            Match(LexType.RPAREN);
            Match(LexType.SEMI);
            t.child[1] = ProcDecPart();
            t.child[2] = ProcBody();
            t.sibling = ProcDec();
            return t;
        }
        public void ParamList(SyntaxTreeNode t) {
            SyntaxTreeNode? p;
            switch (GetCurrent().lex) {
                case LexType.RPAREN:
                    break;
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                case LexType.ARRAY:
                case LexType.RECORD:
                case LexType.ID:
                case LexType.VAR:
                    p = ParamDecList();
                    t.child[0] = p;
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
        }
        public SyntaxTreeNode ParamDecList() {
            SyntaxTreeNode t = Param();
            t.sibling = ParamMore();
            return t;
        }
        public SyntaxTreeNode? ParamMore() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.RPAREN:
                    break;
                case LexType.SEMI:
                    Match(LexType.SEMI);
                    t = ParamDecList();
                    if (t == null) {
                        errorList.Add("[Error] Type #7: A Param declaration is EXPECTED at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                        // ERROR: Param needs a declaration
                    }
                    break;
                default:
                    // 读入下一个单词 ?
                    break;

            }
            return t;
        }
        public SyntaxTreeNode Param() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.DecK);
            t.attr = new SyntaxTreeNode.Attr("proc");
            switch (GetCurrent().lex) {
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                case LexType.ARRAY:
                case LexType.RECORD:
                case LexType.ID:
                    t.attr.procAttr.paramt = ProcAttr.ParamType.Valparamtype;
                    TypeName(t);
                    FormList(t);
                    break;
                case LexType.VAR:
                    Match(LexType.VAR);
                    t.attr.procAttr.paramt = ProcAttr.ParamType.Varparamtype;
                    TypeName(t);
                    FormList(t);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
            return t;
        }
        public void FormList(SyntaxTreeNode t) {
            if (GetCurrent().lex == LexType.ID) {
                t.name[t.idnum++] = GetCurrent().sem;
                Match(LexType.ID);
            }
            FidMore(t);
        }
        public void FidMore(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.SEMI:
                case LexType.RPAREN:
                    break;
                case LexType.COMMA:
                    Match(LexType.COMMA);
                    FormList(t);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词 ?
                    break;
            }
        }
        public SyntaxTreeNode? ProcDecPart() {
            return DeclarePart();
        }

        public SyntaxTreeNode? ProcBody() {
            SyntaxTreeNode t = ProgramBody();
            if (t == null) {
                errorList.Add("[Error] Type #8: A program body is REQUESTED at line "
                    + GetCurrent().line
                    + ", col "
                    + GetCurrent().column
                    + ".");
                // 报错 program needs a body
            }
            return t;
        }
        // 
        public SyntaxTreeNode ProgramBody() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmLK);
            Match(LexType.BEGIN);
            t.child[0] = StmList();
            Match(LexType.END);
            return t;
        }

        public SyntaxTreeNode StmList() {
            SyntaxTreeNode t = Stm();
            t.sibling = StmMore();
            return t;
        }
        public SyntaxTreeNode StmMore() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.END:
                    break;
                case LexType.ENDWH:
                    Match(LexType.ENDWH);
                    break;
                case LexType.SEMI:
                    Match(LexType.SEMI);
                    t = StmList();
                    break;
            }
            return t;
        }
        public SyntaxTreeNode Stm() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.IF:
                    t = ConditionalStm();
                    break;
                case LexType.WHILE:
                    t = LoopStm();
                    break;
                case LexType.RETURN:
                    t = ReturnStm();
                    break;
                case LexType.READ:
                    t = InputStm();
                    break;
                case LexType.WRITE:
                    t = OutputStm();
                    break;
                case LexType.ID:
                    string tmp = GetCurrent().sem;
                    Match(LexType.ID);
                    t = AssCall(tmp);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 跳过当前单词，读入下一个单词
                    break;
            }
            return t;
        }
        public SyntaxTreeNode AssCall(string tmp) {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.ASSIGN:
                case LexType.LMIDPAREN:
                case LexType.DOT:
                    t = AssignmentRest(tmp);
                    break;
                case LexType.LPAREN:
                    t = CallStmRest(tmp);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 读入下一个单词
                    break;
            }
            return t;
        }
        public SyntaxTreeNode AssignmentRest(string tmp) {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.AssignK
            };
            SyntaxTreeNode child0 = SyntaxTreeNode.NewExpKindIdK();
            child0.name[child0.idnum++] = tmp;
            VariMore(child0);
            t.child[0] = child0;
            Match(LexType.ASSIGN);
            t.child[1] = Exp();
            return t;
        }
        public SyntaxTreeNode ConditionalStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.IfK
            };
            Match(LexType.IF);
            t.lineno = GetCurrent().line;
            t.child[0] = Exp(); // IF语句的条件表达式
            Match(LexType.THEN);
            t.child[1] = StmList();
            //条件为真的处理语句
            if (GetCurrent().lex == LexType.ELSE) {
                Match(LexType.ELSE);
                t.child[2] = StmList();
                //条件为假的处理语句
            }
            Match(LexType.FI);
            return t;
        }

        public SyntaxTreeNode LoopStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.WhileK
            };
            Match(LexType.WHILE);
            t.child[0] = Exp(); // WHILE语句的条件表达式
            Match(LexType.DO);
            t.child[1] = StmList(); // 循环语句部分
            return t;
        }

        public SyntaxTreeNode InputStm() {
            // READ
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.ReadK
            };
            Match(LexType.READ);
            Match(LexType.LPAREN);
            t.name[t.idnum++] = GetCurrent().sem;
            Match(LexType.ID);
            Match(LexType.RPAREN);
            return t;
        }
        public SyntaxTreeNode OutputStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.WriteK
            };
            Match(LexType.WRITE);
            Match(LexType.LPAREN);
            t.child[0] = Exp();
            Match(LexType.RPAREN);
            return t;
        }
        public SyntaxTreeNode ReturnStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.ReturnK
            };
            Match(LexType.RETURN);
            return t;
        }
        public SyntaxTreeNode CallStmRest(string tmp) {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK) {
                stmtKind = StmtKind.CallK
            };
            Match(LexType.LPAREN);
            /*函数名为Exp结点*/
            t.child[0] = new SyntaxTreeNode(NodeKind.ExpK) {
                expKind = ExpKind.IdK
            };
            t.child[0].name[t.child[0].idnum++] = tmp;
            t.child[0].attr = new SyntaxTreeNode.Attr("exp");
            t.child[0].attr.expAttr.varKind = ExpAttr.VarKind.IdV;
            t.child[1] = ActParamList();
            Match(LexType.RPAREN);
            return t;
        }

        public SyntaxTreeNode ActParamList() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.RPAREN:
                    break;
                case LexType.ID:
                case LexType.INTC_VAL:
                    t = Exp();
                    if (t != null)
                        t.sibling = ActParamMore();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode ActParamMore() {
            SyntaxTreeNode? t = null;
            switch (GetCurrent().lex) {
                case LexType.RPAREN:
                    break;
                case LexType.COMMA:
                    Match(LexType.COMMA);
                    t = ActParamList();
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode Exp() {
            SyntaxTreeNode t = Simple_exp();
            LexType lex = GetCurrent().lex;
            if (lex == LexType.LT || lex == LexType.EQ) {
                SyntaxTreeNode p = new SyntaxTreeNode(NodeKind.ExpK) {
                    expKind = ExpKind.OpK,
                    attr = new SyntaxTreeNode.Attr("exp")
                };
                p.attr.expAttr = new ExpAttr();
                p.child[0] = t; // 运算表达式的左运算简式
                p.attr.expAttr.op = GetCurrent().sem;
                t = p;
                index++; // 匹配当前单词LT/EQ
                if (t != null) {
                    t.child[1] = Simple_exp(); // 右运算简式
                }
            }
            return t;
        }
        public SyntaxTreeNode Simple_exp() {
            SyntaxTreeNode t = Term();
            while (true) {
                LexType lex = GetCurrent().lex;
                if (lex == LexType.PLUS || lex == LexType.MINUS) {
                    SyntaxTreeNode p = new SyntaxTreeNode(NodeKind.ExpK) {
                        expKind = ExpKind.OpK,
                        attr = new SyntaxTreeNode.Attr("exp")
                    };
                    p.attr.expAttr = new ExpAttr();
                    p.child[0] = t; // 左运算项
                    p.attr.expAttr.op = GetCurrent().sem;
                    t = p;
                    index++; // 匹配当前单词 + -
                    t.child[1] = Term(); // 右运算项
                } else {
                    break;
                }
            }
            return t;
        }
        public SyntaxTreeNode Term() {
            SyntaxTreeNode t = Factor();
            while (true) {
                LexType lex = GetCurrent().lex;
                if (lex == LexType.TIMES || lex == LexType.DIVIDE) {
                    SyntaxTreeNode p = new SyntaxTreeNode(NodeKind.ExpK) {
                        expKind = ExpKind.OpK,
                        attr = new SyntaxTreeNode.Attr("exp")
                    };
                    p.attr.expAttr = new ExpAttr();
                    p.child[0] = t; // 左运算项
                    p.attr.expAttr.op = GetCurrent().sem;
                    t = p;
                    index++; // 匹配当前单词 * /
                    t.child[1] = Factor(); // 右运算项
                } else {
                    break;
                }
            }
            return t;
        }

        public SyntaxTreeNode Factor() {
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.INTC_VAL:
                    t = new SyntaxTreeNode(NodeKind.ExpK);
                    t.attr = new SyntaxTreeNode.Attr("exp");
                    t.expKind = ExpKind.ConstK;
                    t.attr.expAttr.val = int.Parse(GetCurrent().sem);
                    Match(LexType.INTC_VAL);
                    break;
                case LexType.ID:
                    t = Variable();
                    break;
                case LexType.LPAREN:
                    Match(LexType.LPAREN);
                    t = Exp();
                    Match(LexType.RPAREN);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //非期望单词错误,显示出错单词和信息.并读入下一单词
                    break;
            }
            return t;
        }
        public SyntaxTreeNode Variable() {
            SyntaxTreeNode t = SyntaxTreeNode.NewExpKindIdK();

            if (GetCurrent().lex == LexType.ID) {
                t.name[t.idnum++] = GetCurrent().sem;
                Match(LexType.ID);
                VariMore(t);
            }
            return t;
        }
        public void VariMore(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.ASSIGN:
                case LexType.TIMES:
                case LexType.EQ:
                case LexType.LT:
                case LexType.PLUS:
                case LexType.MINUS:
                case LexType.DIVIDE:
                case LexType.RPAREN:
                case LexType.RMIDPAREN:
                case LexType.SEMI:
                case LexType.COMMA:
                case LexType.THEN:
                case LexType.ELSE:
                case LexType.FI:
                case LexType.DO:
                case LexType.ENDWH:
                case LexType.END:
                    break;
                case LexType.LMIDPAREN:
                    Match(LexType.LMIDPAREN);
                    t.child[0] = Exp();
                    t.attr.expAttr.varKind = ExpAttr.VarKind.ArrayMembV;
                    t.child[0].attr.expAttr.varKind = ExpAttr.VarKind.IdV;
                    Match(LexType.RMIDPAREN);
                    break;
                case LexType.DOT: // 为 .
                    Match(LexType.DOT);
                    t.child[0] = FieldVar();

                    t.attr.expAttr.varKind = ExpAttr.VarKind.ArrayMembV;
                    t.child[0].attr.expAttr.varKind = ExpAttr.VarKind.IdV;
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    // 错误信息, 读入下一个token
                    break;
            }
        }
        public SyntaxTreeNode FieldVar() {
            SyntaxTreeNode t = SyntaxTreeNode.NewExpKindIdK();
            t.name[t.idnum++] = GetCurrent().sem;
            t.lineno = GetCurrent().line;
            Match(LexType.ID);
            FieldvarMore(t);
            return t;
        }
        void FieldvarMore(SyntaxTreeNode t) {
            switch (GetCurrent().lex) {
                case LexType.ASSIGN:
                case LexType.TIMES:
                case LexType.EQ:
                case LexType.LT:
                case LexType.PLUS:
                case LexType.MINUS:
                case LexType.DIVIDE:
                case LexType.RPAREN:
                case LexType.RMIDPAREN:
                case LexType.SEMI:
                case LexType.COMMA:
                case LexType.THEN:
                case LexType.ELSE:
                case LexType.FI:
                case LexType.DO:
                case LexType.ENDWH:
                case LexType.END:
                    break;
                case LexType.LMIDPAREN:
                    Match(LexType.LMIDPAREN);
                    t.child[0] = Exp();
                    t.child[0].attr.expAttr.varKind = ExpAttr.VarKind.ArrayMembV;
                    Match(LexType.RMIDPAREN);
                    break;
                default:
                    errorList.Add("[Error] Type #3: Unexcepted token at line "
                        + GetCurrent().line
                        + ", col "
                        + GetCurrent().column
                        + ".");
                    index++;
                    //读入下一个token， 并提示错误信息
                    break;
            }

        }
    }
}
