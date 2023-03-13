using System;
using System.Configuration;
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
    public class SyntaxAnalysiser {
        public List<Token> tokenList;
        public int index;
        public SyntaxAnalysiser(List<Token> tokens) {
            tokenList = tokens;
            index = 0;
        }
        public Token GetCurrent() {
            return tokenList[index];
        }
        public string Match(LexType lexType) {
            if (GetCurrent().lex == lexType) {
                string res = GetCurrent().sem;
                index++;
                return res;
            } else
                throw new Exception("Match错误: 在 line=" + GetCurrent().line + " 匹配失败");
        }
        public SyntaxTreeNode? Parse() {
            try {
                SyntaxTreeNode t = SyntaxProgram();
                // 如果当前和ENDFILE匹配, 则正常结束.
                // 否则报错
                return t;
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }

        }

        SyntaxTreeNode SyntaxProgram() {
            try {
                SyntaxTreeNode root = new SyntaxTreeNode(NodeKind.ProK);
                root.child[0] = ProgramHead();
                root.child[1] = DeclarePart();
                root.child[2] = ProgramBody();
                Match(LexType.DOT);
                return root;
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }

        }
        SyntaxTreeNode ProgramHead() {
            SyntaxTreeNode pheadK = new SyntaxTreeNode(NodeKind.PheadK);
            Match(LexType.PROGRAM);
            pheadK.name[pheadK.idnum++] = Match(LexType.ID);
            return pheadK;
        }
        SyntaxTreeNode? DeclarePart() {
            /*注意, 为null不算错误*/
            SyntaxTreeNode typeP = new SyntaxTreeNode(NodeKind.TypeK);
            typeP.child[0] = TypeDec();
            SyntaxTreeNode varP = new SyntaxTreeNode(NodeKind.VarK);
            varP.child[0] = VarDec();
            typeP.sibling = varP;
            // todo
            return typeP;
        }
        SyntaxTreeNode? TypeDec() {
            switch (GetCurrent().lex) {
                case LexType.TYPE:
                    SyntaxTreeNode t = TypeDeclaration();
                    return t;
                    break;
                case LexType.VAR:
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                default:
                    //读入下一个单词，跳过此单词 ?
                    break;
            }
            return null;
        }
        public SyntaxTreeNode? TypeDeclaration() {
            Match(LexType.TYPE);
            SyntaxTreeNode t = TypeDecList();
            if (t == null) {
                //显示提示信息, 不是报错
            }
            return t;
        }
        public SyntaxTreeNode? TypeDecList() {
            SyntaxTreeNode decK = new SyntaxTreeNode(NodeKind.DecK);
            TypeId(decK);
            Match(LexType.EQ);
            TypeName(decK);
            Match(LexType.SEMI);
            SyntaxTreeNode p = TypeDecMore();
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
                    t.name[t.idnum++] = GetCurrent().sem;
                    Match(LexType.ID);
                    break;
                default:
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
                    // 读入下一个单词 ?
                    break;
            }
        }
        public void ArrayType(SyntaxTreeNode t) {
            Match(LexType.ARRAY);
            Match(LexType.LMIDPAREN);
            if (GetCurrent().lex == LexType.INTC_VAL) {
                t.attr = new SyntaxTreeNode.Attr();
                t.attr.arrayAttr.low = int.Parse(GetCurrent().sem);
            }
            Match(LexType.INTC_VAL);
            Match(LexType.UNDERRANGE);
            if (GetCurrent().lex == LexType.INTC_VAL) {
                t.attr = new SyntaxTreeNode.Attr();
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
                Match(LexType.END);
                // throw (new Exception("错误: 在 line=" + GetCurrent().line));
            }
        }
        public SyntaxTreeNode FieldDecList() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.DecK);
            SyntaxTreeNode p = null;
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
                    //读入下一个单词 (?)
                    break;
            }
        }
        public SyntaxTreeNode FieldDecMore() {
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.ID:
                    break;
                case LexType.INTEGER_T:
                case LexType.CHAR_T:
                case LexType.ARRAY:
                    t = FieldDecList();
                    break;
                default:
                    //读入下一个单词 (?)
                    break;
            }
            return t;
        }
        public SyntaxTreeNode TypeDecMore() {
            switch (GetCurrent().lex) {
                case LexType.ID:
                    return TypeDecList();
                case LexType.VAR:
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                default:
                    //读入下一个单词 (?)
                    break;
            }
            return null;
        }

        public SyntaxTreeNode VarDec() {
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.PROCEDURE:
                case LexType.BEGIN:
                    break;
                case LexType.VAR:
                    t = VarDeclaration();
                    break;
                default:
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
                throw new Exception("VarDeclaration错误");
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
            SyntaxTreeNode t = null;
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
                    throw new Exception("VarIdList错误");
                    // 错误提示
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
                    break;
            }
        }

        //   TO DO 过程声明 25-34


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
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.END:
                case LexType.ENDWH:
                    break;
                case LexType.SEMI:
                    Match(LexType.SEMI);
                    t = StmList();
                    break;
            }
            return t;
        }
        public SyntaxTreeNode Stm() {
            SyntaxTreeNode t = null;
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
                    t = AssCall(tmp);
                    break;
                default:
                    // 跳过当前单词，读入下一个单词
                    break;
            }
            return t;
        }
        public SyntaxTreeNode AssCall(string tmp) {
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.ASSIGN:
                    t = AssignmentRest(tmp);
                    break;
                case LexType.LPAREN:
                    t = CallStmRest(tmp);
                    break;
                default:
                    // 读入下一个单词
                    break;
            }
            return t;
        }
        public SyntaxTreeNode AssignmentRest(string tmp) {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.AssignK;
            Match(LexType.EQ);
            t.child[0] = Exp();
            t.name[t.idnum++] = tmp;
            return t;
        }
        public SyntaxTreeNode ConditionalStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.IfK;
            Match(LexType.IF);
            t.child[0] = Exp(); // IF语句的条件表达式
            Match(LexType.THEN);
            t.child[1] = StmL();
            //条件为真的处理语句
            if (GetCurrent().lex == LexType.ELSE) {
                Match(LexType.ELSE);
                t.child[2] = StmL();
                //条件为假的处理语句
            }
            Match(LexType.FI);
            return t;
        }

        public SyntaxTreeNode LoopStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.WhileK;
            Match(LexType.WHILE);
            t.child[0] = Exp(); // WHILE语句的条件表达式
            Match(LexType.DO);
            t.child[1] = StmList(); // 循环语句部分
            return t;
        }

        public SyntaxTreeNode InputStm() {
            // READ
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.ReadK;
            Match(LexType.READ);
            Match(LexType.RPAREN);
            t.name[t.idnum++] = GetCurrent().sem;
            Match(LexType.ID);
            Match(LexType.LPAREN);
            return t;
        }
        public SyntaxTreeNode OutputStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.WriteK;
            Match(LexType.WRITE);
            Match(LexType.LPAREN);
            t.child[0] = Exp();
            return t;
        }
        public SyntaxTreeNode ReturnStm() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.ReturnK;
            Match(LexType.RETURN);
            return t;
        }
        public SyntaxTreeNode CallStmRest(string tmp) {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.StmtK);
            t.stmtKind = StmtKind.CallK;
            Match(LexType.LPAREN);
            t.child[0] = ActParamList();
            t.name[t.idnum++] = tmp;
            Match(LexType.RPAREN);
            return t;
        }

        public SyntaxTreeNode ActParamList() {
            SyntaxTreeNode t = null;
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
                    //读入下一个单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode ActParamMore() {
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.RPAREN:
                    break;
                case LexType.COMMA:
                    Match(LexType.COMMA);
                    t = ActParamList();
                    break;
                default:
                    //读入下一个单词 ?
                    break;
            }
            return t;
        }
        public SyntaxTreeNode Exp() {
            SyntaxTreeNode t = simple_exp();
            LexType lex = GetCurrent().lex;
            if (lex == LexType.LT || lex == LexType.EQ) {
                SyntaxTreeNode p = new SyntaxTreeNode(NodeKind.ExpK);
                p.expKind = ExpKind.OpK;
                p.attr = new SyntaxTreeNode.Attr();
                p.attr.expAttr = new ExpAttr();
                p.child[0] = t; // 运算表达式的左运算简式
                p.attr.expAttr.op = GetCurrent().sem;
                t = p;
                index++; // 匹配当前单词LT/EQ
                if (t != null) {
                    t.child[1] = simple_exp(); // 右运算简式
                }
            }
            return t;
        }
        public SyntaxTreeNode simple_exp() {
            SyntaxTreeNode t = term();
            while (true) {
                LexType lex = GetCurrent().lex;
                if (lex == LexType.PLUS || lex == LexType.MINUS) {
                    SyntaxTreeNode p = new SyntaxTreeNode(NodeKind.ExpK);
                    p.expKind = ExpKind.OpK;
                    p.attr = new SyntaxTreeNode.Attr();
                    p.attr.expAttr = new ExpAttr();
                    p.child[0] = t; // 左运算项
                    p.attr.expAttr.op = GetCurrent().sem;
                    t = p;
                    index++; // 匹配当前单词 + -
                    t.child[1] = term(); // 右运算项
                } else {
                    break;
                }
            }
            return t;
        }
        public SyntaxTreeNode term() {
            SyntaxTreeNode t = factor();
            while (true) {
                LexType lex = GetCurrent().lex;
                if (lex == LexType.TIMES || lex == LexType.DIVIDE) {
                    SyntaxTreeNode p = new SyntaxTreeNode(NodeKind.ExpK);
                    p.expKind = ExpKind.OpK;
                    p.attr = new SyntaxTreeNode.Attr();
                    p.attr.expAttr = new ExpAttr();
                    p.child[0] = t; // 左运算项
                    p.attr.expAttr.op = GetCurrent().sem;
                    t = p;
                    index++; // 匹配当前单词 * /
                    t.child[1] = factor(); // 右运算项
                } else {
                    break;
                }
            }
            return t;
        }

        public SyntaxTreeNode factor() {
            SyntaxTreeNode t = null;
            switch (GetCurrent().lex) {
                case LexType.INTC_VAL:
                    t = new SyntaxTreeNode(NodeKind.ExpK);
                    t.expKind = ExpKind.ConstK;
                    break;
                case LexType.ID:
                    t = variable();
                    break;
                case LexType.LPAREN:
                    Match(LexType.LPAREN);
                    t = Exp();
                    Match(LexType.RPAREN);
                    break;
                default:
                    //非期望单词错误,显示出错单词和信息.并读入下一单词
                    break;
            }
            return t;
        }
        public SyntaxTreeNode variable() {
            SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.ExpK);
            t.expKind = ExpKind.IdK;
            if (GetCurrent().lex == LexType.ID) {
                t.name[t.idnum++] = GetCurrent().sem;
                Match(LexType.ID);
                variMore(t);
            }
            return t;
        }
        public void variMore(SyntaxTreeNode t) {
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
                    t.attr.expAttr.varKind = ExpAttr.VarKind.ArrayMembVFieldMembV;
                    t.child[0].attr.expAttr.varKind = ExpAttr.VarKind.IdV;
                    break;
                case LexType.DOT: // 为 .
                    Match(LexType.DOT);
                    //t.child[0] = fieldVar();
                    t.attr.expAttr.varKind = ExpAttr.VarKind.ArrayMembVFieldMembV;
                    t.child[0].attr.expAttr.varKind = ExpAttr.VarKind.IdV;
                    break;
                default:
                    // 错误信息, 读入下一个token
                    break;
            }
        }
    }
}
