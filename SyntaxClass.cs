using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode.Attr;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode.Attr.ExpAttr;

namespace CompilationPrinciple {
    public class SyntaxClass {
        public class SyntaxTreeNode {
            public SyntaxTreeNode[] child { get; set; }
            // 指向子语法树节点指针，为语法树节点指针类型
            public SyntaxTreeNode? sibling { get; set; }
            // 指向兄弟语法树节点指针，为语法树节点指针类型。

            public int lineno;
            // 记录源程序行号，为整数类型


            public NodeKind nodeKind;
            // 记录语法树节点类型，取值 ProK, PheadK, TypeK, VarK,
            // ProcDecK, StmLK, DecK, StmtK, ExpK,为语法树节点类型。

            public DecKind decKind;
            //记录语法树节点的声明类型，当 nodekind = DecK 时有效，取
            //值 ArrayK, CharK, IntegerK, RecordK, IdK，为语法树节点声明
            //类型。

            public StmtKind stmtKind;
            //记录语法树节点的语句类型，当 nodekind = StmtK 时有效，
            //取值 IfK, WhileK, AssignK, ReadK, WriteK, CallK, ReturnK，为语法树节点语句类型。

            public ExpKind expKind;
            //记录语法树节点的表达式类型，当 nodekind=ExpK 时有效，
            //取值 OpK, ConstK, IdK，为语法树节点表达式类型。

            public int idnum;
            //记录一个节点中的标志符的个数

            public string[] name;
            //字符串数组，数组成员是节点中的标志符的名字

            // public addr table[]
            // TODO 标志符在符号表的入口

            public string? typeName;
            //记录类型名，当节点为声明类型，且类型是由类型标志符表示时有效。

            public class Attr {
                //记录语法树节点其他属性,为结构体类型。
                public class ArrayAttr {
                    //记录数组类型的属性。
                    public int low, up;
                    public string? childType;
                    public override string ToString() {
                        return low + "  " + up + "  " + childType;
                    }
                    public String convertTypeToName() {
                        if (childType == null)
                            return "";
                        switch (childType) {
                            case "IntegerK":
                                return "integer";
                            case "ArrayK":
                                return "array";
                            case "CharK":
                                return "char";
                        }
                        return "ERROR";
                    }
                    //记录数组的成员类型
                }
                public class ProcAttr {
                    //记录过程的属性
                    public enum ParamType {
                        Error, Valparamtype, Varparamtype
                    }
                    public ParamType paramt;
                    //记 录 过 程 的 参 数 类 型 ， 表示过程的参数是值参还是变参
                   
                    public override string ToString() {
                        return Enum.GetName(typeof(ParamType), paramt) + "  ";

                    }
                }
                public class ExpAttr {
                    //记录表达式的属性
                    public string? op;
                    //记录语法树节点的运算符单词，为单词类型。
                    //当语法树节点为“关系运算表达式”对应节点时，取
                    //值 LT,EQ；当语法树节点为“加法运算简单表达式”
                    //对应节点时，取值 PLUS, MINUS；当语法树节点为
                    //“乘法运算项”对应节点时，取值 TIMES, OVER；其
                    //它情况下无效。

                    public int val;
                    //记录语法树节点的数值,当语法树节点为“数字
                    //因子”对应的语法树节点时有效,为整数类型。
                    public enum VarKind {
                        Error, IdV, ArrayMembV, FieldMembV
                    }
                    public VarKind varKind;
                    public string? type;
                    //记 录 语 法 树 节 点 的 检 查 类 型 ， 取 值 Void,
                    //Integer,Boolean,为类型检查 ExpType 类型。

                }
                public ArrayAttr? arrayAttr;
                public ProcAttr? procAttr;
                public ExpAttr? expAttr;
                public Attr(string attrType) {
                    switch (attrType) {
                        case "array":
                            arrayAttr = new ArrayAttr();
                            break;
                        case "proc":
                            procAttr = new ProcAttr();
                            break;
                        case "exp":
                            expAttr = new ExpAttr();
                            break;
                        default:
                            throw new Exception("错误, 未知的名字 " + attrType);
                    }
                }
            }
            public Attr? attr;
            public SyntaxTreeNode() {
                // 私有, 防止没有设置NodeKind
                idnum = 0;
                child = new SyntaxTreeNode[3];
                name = new string[10];
            }
            public SyntaxTreeNode(NodeKind n) : this() {
                nodeKind = n;
            }
            static public SyntaxTreeNode NewExpKindIdK() {
                SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.ExpK) {
                    expKind = ExpKind.IdK,
                    attr = new Attr("exp")
                };
                Debug.Assert(t.attr.expAttr != null);
                t.attr.expAttr.varKind = VarKind.IdV;
                return t;
            }
            public String PrintTree(int tab) {
                String res = "";
                for (int i = 0; i < tab; i++) {
                    res += "\t";
                    Console.Write("\t");
                }
                Console.Write(Enum.GetName(typeof(NodeKind), nodeKind) + "  ");
                res += Enum.GetName(typeof(NodeKind), nodeKind) + "  ";

                switch (nodeKind) {
                    case NodeKind.DecK:
                        Console.Write(Enum.GetName(typeof(DecKind), decKind) + "  ");
                        res += Enum.GetName(typeof(DecKind), decKind) + "  ";
                        break;
                    case NodeKind.StmtK:
                        Console.Write(Enum.GetName(typeof(StmtKind), stmtKind) + "  ");
                        res += Enum.GetName(typeof(StmtKind), stmtKind) + "  ";
                        break;
                    case NodeKind.ExpK:
                        Console.Write(Enum.GetName(typeof(ExpKind), expKind) + "  ");
                        res += Enum.GetName(typeof(ExpKind), expKind) + "  ";
                        break;
                }

                if (nodeKind == NodeKind.DecK && decKind == DecKind.IdK) {
                    Console.Write(typeName + "  ");
                    res += typeName + "  ";
                }

                for (int i = 0; i < idnum; i++) {
                    Console.Write(name[i] + "  ");
                    res += name[i] + "  ";
                }
                if (attr != null) {
                    if (attr.arrayAttr != null) {
                        Console.Write(attr.arrayAttr);
                        res += attr.arrayAttr;
                    }
                    if (attr.procAttr != null) {
                        Console.Write(attr.procAttr);
                        res += attr.procAttr;
                    }
                    if (attr.expAttr != null) {
                        switch (expKind) {
                            case ExpKind.OpK:
                                Console.Write(attr.expAttr.op + "  ");
                                res += attr.expAttr.op + "  ";
                                break;
                            case ExpKind.ConstK:
                                Console.Write(attr.expAttr.val + "  ");
                                res += attr.expAttr.val + "  ";
                                break;
                            case ExpKind.IdK:
                                Console.Write(Enum.GetName(typeof(VarKind), attr.expAttr.varKind) + "  ");
                                res += Enum.GetName(typeof(VarKind), attr.expAttr.varKind) + "  ";
                                break;
                        }
                    }
                }

                Console.WriteLine();
                res += "\r\n";
                for (int i = 0; i < 3; i++) {
                    if (child[i] != null) {
                        if (child[i].nodeKind == NodeKind.Error) {
                            child[i] = null;
                        } else {
                            res += child[i].PrintTree(tab + 1);
                        }
                    }
                }
                if (sibling != null) {
                    if (sibling.nodeKind == NodeKind.Error) {
                        sibling = null;
                    } else {
                        res += sibling.PrintTree(tab);
                    }
                }
                return res;
            }
            public String GenerateCode(int tab, SyntaxTreeNode? fa) {
                String res = "";
                String tabSpace = "";
                for (int i = 0; i < tab; i++) {
                    tabSpace += "    ";
                }
                res += tabSpace;
                Console.Write(tabSpace);
                SyntaxTreeNode? c = null;
                switch (nodeKind) {
                    case NodeKind.ProK:
                        bool flag = false;
                        for(int i = 0; i < 3; i++) {
                            if (child[i] != null) {
                                c = child[i];
                                while(c != null) {
                                    if (!flag) {
                                        flag = true;
                                    } else {
                                        res += "\r\n";
                                        Console.WriteLine();
                                    }
                                    res += c.GenerateCode(0, this);
                                    c = c.sibling;
                                }
                            }
                        }
                        res += ".";
                        Console.Write(".");
                        break;

                    case NodeKind.PheadK:
                        res += "program " + name[0];
                        Console.Write("program " + name[0]);
                        break;
                    case NodeKind.TypeK:
                        res += "type";
                        Console.Write("type");
                        c = child[0];
                        while(c != null) {
                            Console.WriteLine();
                            res += "\r\n" + c.GenerateCode(tab + 1, this);
                            c = c.sibling;
                        }
                        break;
                    case NodeKind.VarK:
                        res += "var";
                        Console.Write("var");
                        c = child[0];
                        while(c != null) {
                            Console.WriteLine();
                            res += "\r\n" + c.GenerateCode(tab + 1, this);
                            c = c.sibling;
                        }
                        break;
                    case NodeKind.DecK:
                        Debug.Assert(fa != null);
                        switch (fa.nodeKind) {
                            case NodeKind.TypeK:
                                switch (decKind) {
                                    case DecKind.ArrayK:
                                        Debug.Assert(attr != null && attr.arrayAttr != null);
                                        String arr = attr.arrayAttr.low + ".." + attr.arrayAttr.up;
                                        res += PrintName() + " = array[" + arr + "] of " + attr.arrayAttr.convertTypeToName() + ";";
                                        Console.Write(PrintName() + " = array[" + arr + "] of " + attr.arrayAttr.convertTypeToName() + ";");
                                        break;
                                    case DecKind.CharK:
                                        res += PrintName() + " = char;";
                                        Console.Write(PrintName() + " = char;");
                                        break;
                                    case DecKind.IntegerK:
                                        res += PrintName() + " = integer;";
                                        Console.Write(PrintName() + " = integer;");
                                        break;
                                    case DecKind.RecordK:
                                        String pre = new String(' ', (PrintName() + " = ").Length);
                                        res += PrintName() + " = record\r\n";
                                        Console.Write(PrintName() + " = record\r\n");
                                        c = child[0];
                                        while (c != null) {
                                            res += c.GenerateCode(tab + 1 + pre.Length / 4, this) + "\r\n";
                                            Console.WriteLine();
                                            c = c.sibling;
                                        }
                                        res += tabSpace + pre + "end;";
                                        Console.Write(tabSpace + pre + "end;");
                                        break;
                                    case DecKind.IdK:
                                        res += PrintName() + " = " + typeName + ";";
                                        Console.Write(PrintName() + " = " + typeName + ";");
                                        break;
                                }
                                break;
                            case NodeKind.VarK:
                            case NodeKind.DecK:
                            case NodeKind.ProcDecK:
                                // 函数参数列表中的变参
                                if(fa.nodeKind == NodeKind.ProcDecK && attr != null && attr.procAttr != null && attr.procAttr.paramt == ProcAttr.ParamType.Varparamtype) {
                                    res += "var ";
                                    Console.Write("var "); 
                                }
                                switch (decKind) {
                                    case DecKind.ArrayK:
                                        Debug.Assert(attr != null && attr.arrayAttr != null);
                                        String arr = attr.arrayAttr.low + ".." + attr.arrayAttr.up;
                                        res += "array[" + arr + "] of " + attr.arrayAttr.childType + " " + PrintName();
                                        Console.Write("array[" + arr + "] of " + attr.arrayAttr.convertTypeToName() + " " + PrintName());
                                        break;
                                    case DecKind.CharK:
                                        res += "char " + PrintName();
                                        Console.Write("char " + PrintName());
                                        break;
                                    case DecKind.IntegerK:
                                        res += "integer " + PrintName();
                                        Console.Write("integer " + PrintName());
                                        break;
                                    case DecKind.RecordK:
                                        if (fa.nodeKind != NodeKind.ProcDecK) {
                                            res += "record\r\n";
                                            Console.Write("record\r\n");
                                            c = child[0];
                                            while (c != null) {
                                                res += c.GenerateCode(tab + 1, this) + "\r\n";
                                                Console.WriteLine();
                                                if (c.sibling != null) {
                                                    Console.Write(";");
                                                    res += ";";
                                                }
                                                c = c.sibling;
                                            }
                                            res += tabSpace + "end " + PrintName();
                                            Console.Write(tabSpace + "end " + PrintName());
                                        } else {
                                            res += "record ";
                                            Console.Write("record ");
                                            c = child[0];
                                            while (c != null) {
                                                res += c.GenerateCode(0, this) + " ";
                                                Console.Write(" ");
                                                c = c.sibling;
                                            }
                                            res += "end " + PrintName();
                                            Console.Write("end " + PrintName());
                                        }

                                        break;
                                    case DecKind.IdK:
                                        res += typeName + " " + PrintName();
                                        Console.Write(typeName +  " " + PrintName());
                                        break;
                                }
                                if(fa.nodeKind != NodeKind.ProcDecK) {
                                    res += ";";
                                    Console.Write(";");
                                    // 正常语句, 非参数列表
                                }
                                break;
                        }
                        break;

                   case NodeKind.ProcDecK:
                        res += "procedure " + name[0] + "(";
                        Console.Write("procedure " + name[0] + "(");
                        c = child[0];
                        if(c != null) {
                            if (c.nodeKind == NodeKind.DecK) {
                                while(c != null) {
                                    res += c.GenerateCode(0, this);
                                    if (c.sibling == null) {
                                        res += ");";
                                        Console.Write(");");
                                    }
                                    else {
                                        res += "; ";
                                        Console.Write("; ");
                                    }
                                    c = c.sibling;
                                }
                            }else {
                                res += ");";
                                Console.Write(");");
                            }
                        } 
                        for(int i= 1; i < 3; i++) {
                            Console.Write("\r\n");
                            res += "\r\n" + child[i].GenerateCode(tab + 1, this);
                        }
                        break;
                    case NodeKind.StmLK:
                        res += "begin";
                        Console.Write("begin");
                        c = child[0];
                        if(c != null) {
                            while(c != null) {
                                Console.Write("\r\n");
                                res += "\r\n" + c.GenerateCode(tab + 1, this);
                                if(c.sibling != null) {
                                    Console.Write(";");
                                    res += ";";
                                }
                                c = c.sibling;
                            }
                        }
                        res += "\r\n" + tabSpace + "end";
                        Console.Write("\r\n" + tabSpace + "end");
                        break;
                    case NodeKind.StmtK:
                        switch (stmtKind) {
                            case StmtKind.IfK:
                                Console.Write("if ");
                                String tmp = child[0].GenerateCode(0, this);
                                Console.Write(" then");
                                res += "if " + tmp + " then";
                                c = child[1];
                                if(c != null) {
                                    while(c != null) {
                                        Console.Write("\n");
                                        res += "\n";
                                        c.GenerateCode(tab + 1, this);
                                        if(c.sibling != null) {
                                            Console.Write(";");
                                            res += ";";
                                        }
                                        c = c.sibling;
                                    }
                                }
                                c = child[2];
                                if (c != null) {
                                    Console.Write("\r\n" + tabSpace + "else");
                                    res += "\r\n" +tabSpace + "else";
                                    while (c != null) {
                                        Console.Write("\n");
                                        res += "\n";
                                        c.GenerateCode(tab + 1, this);
                                        if (c.sibling != null) {
                                            Console.Write(";");
                                            res += ";";
                                        }
                                        c = c.sibling;
                                    }
                                }
                                Console.Write("\r\n" + tabSpace + "fi");
                                res += "\r\n" + tabSpace + "fi";
                                break;
                            case StmtKind.WhileK:
                                Console.Write("while ");
                                res += "while " + child[0].GenerateCode(0, this) + " do";
                                Console.Write(" do");
                                c = child[1];
                                while(c != null) {
                                    Console.WriteLine();
                                    res += "\r\n" + c.GenerateCode(tab + 1, this);
                                    if (c.sibling != null) {
                                        Console.Write(";");
                                        res += ";";
                                    }
                                    c = c.sibling;
                                }
                                Console.Write("\r\n" + tabSpace + "endwh");
                                break;
                            case StmtKind.AssignK:
                                res += child[0].GenerateCode(0, this);
                                Console.Write(" := ");
                                res += " := ";
                                res += child[1].GenerateCode(0, this);
                                break;
                            case StmtKind.ReadK:
                                res += "read(" + PrintName() + ")";
                                Console.Write("read(" + PrintName() + ")");
                                break;
                            case StmtKind.WriteK:
                                Console.Write("write(");
                                res += "write(" + child[0].GenerateCode(0, this) + ")";
                                Console.Write(")");
                                break;
                            case StmtKind.CallK:
                                res += child[0].GenerateCode(0, this);
                                Console.Write("(");
                                res += child[1].GenerateCode(0, this);
                                Console.Write(")");
                                break;
                            case StmtKind.ReturnK:
                                break;
                        }
                        break;
                    case NodeKind.ExpK:
                        switch (expKind) {
                            case ExpKind.OpK:
                                SyntaxTreeNode ls = child[0];
                                SyntaxTreeNode rs = child[1];
                                String l = "", r = "";
                                if (LsonNeedParen()) {
                                    Console.Write("(");
                                    res += "(" + ls.GenerateCode(0, this) + ")";
                                    Console.Write(")");
                                } else {
                                    res += ls.GenerateCode(0, this);
                                }
                                res += " " + attr.expAttr.op + " ";
                                Console.Write(" " + attr.expAttr.op + " ");
                                if (RsonNeedParen()) {
                                    Console.Write("(");
                                    res += "(" + rs.GenerateCode(0, this) + ")";
                                    Console.Write(")");
                                } else {
                                    res += rs.GenerateCode(0, this);
                                }
                                break;
                            case ExpKind.ConstK:
                                Console.Write(attr.expAttr.val);
                                res += attr.expAttr.val;
                                break;
                            case ExpKind.IdK:
                                switch (attr.expAttr.varKind) {
                                    case VarKind.IdV:
                                        res += name[0];
                                        Console.Write(name[0]);
                                        break;
                                    case VarKind.ArrayMembV:
                                        Console.Write(name[0] + "[");
                                        res += name[0] + "[" + child[0].GenerateCode(0, this) + "]";
                                        Console.Write("]");
                                        break;
                                    case VarKind.FieldMembV:
                                        Console.Write(name[0] + ".");
                                        res += name[0] + "." + child[0].GenerateCode(0, this);
                                        break;
                                }
                                break;
                        }
                        break;
                }
                /*if(fa != null && fa.nodeKind != NodeKind.ProcDecK && sibling != null && nodeKind != NodeKind.StmtK) {
                    Console.WriteLine();
                    res += "\r\n" + sibling.GenerateCode(tab, fa);
                }*/
                return res;

            }

            public String PrintName() {
                String res = name[0];
                for (int i = 1;  i < idnum; i++)
                    res += ", " + name[i] ;
                return res;
            }
            public bool LsonNeedParen() {
                if(expKind == ExpKind.OpK && child[0].expKind == ExpKind.OpK) {
                    bool flag1 = attr.expAttr.op is "*" or "/";
                    bool flag2 = child[0].attr.expAttr.op is "+" or "*";
                    return flag1 && flag2;
                }
                return false;
            }
            public bool RsonNeedParen() {
                if (expKind == ExpKind.OpK && child[1].expKind == ExpKind.OpK) {
                    String op1 = attr.expAttr.op, op2 = attr.expAttr.op;
                    if (op1 == "*")
                        return op2 is "+" or "-";
                    if (op1 == "*")
                        return op2 is "+" or "-";
                    return op1 == "*";
                }
                return false;
            }
            public void DeepCopy(SyntaxTreeNode currentNode) {
                if (this.child[0] == null) {
                    this.child = currentNode.child;
                }
                if (this.attr == null) {
                    this.attr = currentNode.attr;
                }
                expKind = currentNode.expKind;
                idnum = currentNode.idnum;
                name = currentNode.name;
                nodeKind = currentNode.nodeKind;
                decKind = currentNode.decKind;
                stmtKind = currentNode.stmtKind;
                typeName = currentNode.typeName;
                lineno = currentNode.lineno;
            }
        }
        public enum NodeKind {
            Error, ProK, PheadK, TypeK, VarK,
            ProcDecK, StmLK, DecK, StmtK, ExpK
        }
        public enum DecKind {
            Error, ArrayK, CharK, IntegerK, RecordK, IdK
        }
        public enum StmtKind {
            Error, IfK, WhileK, AssignK, ReadK, WriteK, CallK, ReturnK
        }
        public enum ExpKind {
            Error, OpK, ConstK, IdK
        }
    }
    
}
