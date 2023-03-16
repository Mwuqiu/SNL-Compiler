﻿using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode.Attr;
using static CompilationPrinciple.SyntaxClass.SyntaxTreeNode.Attr.ExpAttr;

namespace CompilationPrinciple {
    public class SyntaxClass {
        public class SyntaxTreeNode {
            public SyntaxTreeNode?[] child { get; set; }
            // 指向子语法树节点指针，为语法树节点指针类型
            public SyntaxTreeNode? sibling { get; set; }
            // 指向兄弟语法树节点指针，为语法树节点指针类型。

            public int lineno;
            // 记录源程序行号，为整数类型


            public NodeKind nodeKind;
            // 记录语法树节点类型，取值 ProK, PheadK, TypeK, VarK,
            // ProcDecK, StmLK, DecK, StmtK, ExpK,为语法树节点类型。

            public DecKind  decKind;
            //记录语法树节点的声明类型，当 nodekind = DecK 时有效，取
            //值 ArrayK, CharK, IntegerK, RecordK, IdK，为语法树节点声明
            //类型。

            public StmtKind stmtKind;
            //记录语法树节点的语句类型，当 nodekind = StmtK 时有效，
            //取值 IfK, WhileK, AssignK, ReadK, WriteK, CallK, ReturnK，为语法树节点语句类型。

            public ExpKind  expKind;
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
                    //记录数组的成员类型
                }
                public class ProcAttr {
                    //记录过程的属性
                    public string? paramt;
                    //记 录 过 程 的 参 数 类 型 ， 值 为 枚 举 类 型
                    //valparamtype 或者 varparamtype，表示过程的参数
                    //是值参还是变参。
                    public override string ToString() {
                        if (paramt == null)
                            return "";
                        return paramt.ToString();
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
            public SyntaxTreeNode(NodeKind n) : this(){
                nodeKind = n;
            }
            static public SyntaxTreeNode NewExpKindIdK() {
                SyntaxTreeNode t = new SyntaxTreeNode(NodeKind.ExpK) {
                    expKind = ExpKind.IdK,
                    attr = new Attr("exp")
                };
                t.attr.expAttr.varKind = VarKind.IdV;
                return t;
            }
            public void PrintTree(int space) {

                for (int i = 0; i < space; i++)
                    Console.Write(" ");
                Console.Write(Enum.GetName(typeof(NodeKind), nodeKind) + "  ");
                switch (nodeKind) {
                    case NodeKind.DecK:
                        Console.Write(Enum.GetName(typeof(DecKind), decKind) + "  ");
                        break;
                    case NodeKind.StmtK:
                        Console.Write(Enum.GetName(typeof(StmtKind), stmtKind) + "  ");
                        break;
                    case NodeKind.ExpK:
                        Console.Write(Enum.GetName(typeof(ExpKind), expKind) + "  ");
                        break;
                }
                if (nodeKind == NodeKind.DecK && expKind == ExpKind.IdK) {
                    Console.Write(typeName + "  ");
                }
                for(int i = 0; i< idnum; i++) {
                    Console.Write(name[i] + "  ");
                }
                if(attr != null) {
                    if(attr.arrayAttr != null) {
                        Console.Write(attr.arrayAttr);
                    }
                    if (attr.procAttr != null) {
                        Console.Write(attr.procAttr);
                    }
                    if (attr.expAttr != null) {
                        switch (expKind) {
                            case ExpKind.OpK:
                                Console.Write(attr.expAttr.op + "  ");
                                break;
                            case ExpKind.ConstK:
                                Console.Write(attr.expAttr.val + "  ");
                                break;
                            case ExpKind.IdK:
                                Console.Write(Enum.GetName(typeof(VarKind), attr.expAttr.varKind) + "  ");
                                break;
                        }
                    }
                }
                
                Console.WriteLine();
                for(int i = 0; i < 3; i++) {
                    if (child[i] != null) {
                        child[i].PrintTree(space + 4);
                    }
                }
                if (sibling != null)
                    sibling.PrintTree(space);
            }
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
