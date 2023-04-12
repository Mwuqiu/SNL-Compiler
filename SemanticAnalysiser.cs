﻿using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CompilationPrinciple.SyntaxClass;

namespace CompilationPrinciple {
    internal class SemanticAnalysiser {
        int mainOff;
        int StoreNoff;
        int savedOff;

        TypeIR intPtr;
        TypeIR charPtr;
        TypeIR boolPtr;

        private int off;
        private int level;
        private int id;
        private int initOff;
        private SymTableItem[] scope;

        public SemanticAnalysiser() {
            StoreNoff = 0;
            savedOff = 0;
            mainOff = 0;
            initOff = 7;
            id = 0;
            off = 0;
            level = -1;
            scope = new SymTableItem[50];
        }

        public void CreateTable() {
            level = level + 1;    
            off = initOff;
        }

        public void analyze(SyntaxTreeNode root) {
            CreateTable();
            initialize();
            SyntaxTreeNode p = root.child[1];
            while (p != null) { 
                if(p.nodeKind == NodeKind.TypeK) {
                    TypeDecPart(p.child[0]);
                }else if(p.nodeKind == NodeKind.VarK) {
                    VarDecPart(p.child[0]);
                }else if(p.nodeKind == NodeKind.ProcDecK) {
                    ProcDecPart(p);
                } else {
                    Console.WriteLine("no this node kind in syntax tree!");
                }
                p = p.sibling;
            }
        }

        public void TypeDecPart(SyntaxTreeNode p) {
            bool present = false;
            AttributeIR attr = new AttributeIR();
            SymTableItem entry = new SymTableItem();
            attr.typeKind = IdKind.typeKind;
            while (p != null) {                               
                Enter(ref present, attr, entry , p.name[0]);
                if (present) {
                    Console.WriteLine("repetation declared!");
                    entry = new SymTableItem();
                } else {
                    entry.attrIR.idType = TypeProcess(p, p.decKind);
                }
                p = p.sibling;
            }
        }

        public void VarDecPart(SyntaxTreeNode p) {
            VarDecList(p);
        }

        public void ProcDecPart(SyntaxTreeNode t) {
            SyntaxTreeNode p = new SyntaxTreeNode();
            /*处理过程头*/
            SymTableItem entry = HeadProcess(t);
            t = t.child[1];
            while(t != null) {
                switch (t.nodeKind) {
                    case NodeKind.TypeK:
                        TypeDecPart(t.child[0]);
                        break;
                    case NodeKind.VarK:
                        VarDecPart(t.child[0]);
                        break;
                    case NodeKind.ProcDecK:
                        break;
                    default:
                        Console.WriteLine("no this node kind in syntax tree!");
                        break;
                }
                if (t.nodeKind ==NodeKind.ProcDecK)
                    break;
                else
                    t = t.sibling;
            }
            entry.attrIR.procAttr.nOff = savedOff;
            entry.attrIR.procAttr.mOff = entry.attrIR.procAttr.nOff + entry.attrIR.procAttr.level + 1;
            /*过程活动记录的长度等于nOff加上display表的长度*
                *diplay表的长度等于过程所在层数加一           */
            while(t != null) {
                ProcDecPart(t);
                t = t.sibling;
            }
            t = p;            
        }

        public void Body(SyntaxTreeNode t) { 
            if(t.nodeKind == NodeKind.StmLK) {
                SyntaxTreeNode p = t.child[0];
                while(p != null) {
                    statement(p);/*调用语句状态处理函数*/
                    p = p.sibling;/*依次读入语法树语句序列的兄弟节点*/
                }
            }
        }

        public void statement(SyntaxTreeNode t) { 
            switch(t.stmtKind) {
                case StmtKind.IfK:
                    ifstatment(t);
                    break;
                case StmtKind.WhileK:
                    whilestatement(t);
                    break;
                case StmtKind.AssignK:
                    assignstatement(t);
                    break;
                case StmtKind.ReadK:
                    readstatement(t);
                    break;
                case StmtKind.WriteK:
                    writestatement(t);
                    break;
                case StmtKind.CallK:
                    callstatement(t);
                    break;
                case StmtKind.ReturnK:
                    returnstatement(t);
                    break;
                default:
                    Console.WriteLine("statement type error");
                    break;
            }
        }

        public AttributeIR FindAttr(SymTableItem entry) {
            AttributeIR attr = entry.attrIR;
            return attr;
        }

        public bool Compat(TypeIR tp1, TypeIR tp2) {
            bool present;
            if(tp1 == tp2) {
                present = false;
            } else {
                present = true;
            }
            return present;
        }

        public TypeIR arrayVar(SyntaxTreeNode t) {
            bool present = false;
            SymTableItem entry = null;
            TypeIR Eptr0 = null;
            TypeIR Eptr1 = null;
            TypeIR Eptr = null;
            /*在符号表中查找此标识符*/
            present = FindEntry(t.name[0], entry);
            t.table[0] = entry;
            /*找到*/
            if (present) {
                if(FindAttr(entry).typeKind != IdKind.varKind) {
                    Console.WriteLine("is not variable error!");
                    Eptr = null;
                } else {
                    /*检查E的类型是否与下标类型相符*/
                    Eptr0 = entry.attrIR.idType.arrayAttr.indexTy;
                    if(Eptr0 == null) {
                        return null;
                    }
                    AccessKind Ekind = new AccessKind();
                    Eptr1 = Expr(t.child[0], Ekind,false);
                    if(Eptr == null) {
                        return null;
                    }
                    present = Compat(Eptr0, Eptr1);
                    if (present) {
                        Console.WriteLine("type is not matched with the array member error !");
                        Eptr = null;
                    } else {
                        Eptr = entry.attrIR.idType.arrayAttr.elementType;
                    }
                }
            } else {
                Console.WriteLine("is not declarations");
            }
            return Eptr;
        }

        public TypeIR recordVar(SyntaxTreeNode t) {
            bool present = false;
            bool result = true;
            SymTableItem entry = null;

            TypeIR Eptr0 = null;
            TypeIR Eptr1 = null;
            TypeIR Eptr = null;
            FieldChain currentP = null;

            present = FindEntry(t.name[0], entry);
            t.table[0] = entry;

            if (present) {
                /*Var0不是变量*/
                if(FindAttr(entry).typeKind != IdKind.varKind ) {
                    Console.WriteLine("is not variable error");
                    Eptr = null;
                } else {
                    if(FindAttr(entry).idType.typeKind != TypeKind.recordTy) {
                        Console.WriteLine("is not record variable error");
                        Eptr = null;
                    } else {
                        /*检查id是否是合法域名*/
                        Eptr0 = entry.attrIR.idType;
                        currentP = Eptr0.next;
                        while((currentP != null) && (result != false)) {
                            result = !(t.child[0].name[0] == currentP.id);
                            if(result == false) {
                                Eptr = currentP.unitType;
                            } else {
                                currentP = currentP.next;
                            }
                        }
                        if(currentP == null) {
                            if(result != false) {
                                Eptr = null;
                                Console.WriteLine("is not field type");
                            }
                        } else {
                            if (t.child[0].child[0] != null) {
                                Eptr = arrayVar(t.child[0]);
                            }
                        }
                    }
                }
            } else {
                Console.WriteLine("is not declarations");
            }
            return Eptr;
        }

        public TypeIR Expr(SyntaxTreeNode t, AccessKind Ekind, bool hasEKind) {
            bool present = false;
            SymTableItem entry = null;
            TypeIR Eptr0 = null;
            TypeIR Eptr1 = null;
            TypeIR Eptr = null;
            if(t != null) {
                switch (t.expKind) {
                    case ExpKind.ConstK:
                        Eptr = TypeProcess(t, DecKind.IntegerK);
                        Eptr.typeKind = TypeKind.intTy;
                        if(hasEKind) {
                            Ekind = AccessKind.dir;
                        }
                        break;
                    case ExpKind.IdK:
                        if (t.child[0] == null) {
                            present = FindEntry(t.name[0], entry);
                            t.table[0] = entry;

                            if (present) {
                                /*id不是变量*/
                                if (FindAttr(entry).typeKind != IdKind.varKind) {
                                    Console.WriteLine("is not variable error");
                                    Eptr = null;
                                } else {
                                    Eptr = entry.attrIR.idType;
                                    if(hasEKind) {
                                        Ekind = AccessKind.indir;
                                    }
                                }
                            } else {
                                //标识符无声明
                                Console.WriteLine("is not declarations");
                            }
                        } else {
                            if(t.attr.expAttr.varKind == SyntaxTreeNode.Attr.ExpAttr.VarKind.ArrayMembV) {
                                Eptr = arrayVar(t);
                            } else {
                                if(t.attr.expAttr.varKind == SyntaxTreeNode.Attr.ExpAttr.VarKind.FieldMembV) {
                                    Eptr = recordVar(t);
                                }
                            }
                        }
                         break;
                    case ExpKind.OpK:

                        /*递归调用儿子节点*/
                        Eptr0 = Expr(t.child[0], Ekind, false);
                        if(Eptr0 == null) {
                            return null;
                        }
                        Eptr1 = Expr(t.child[1], Ekind, false);
                        if (Eptr1 == null) {
                            return null;
                        }

                        /*类型判别*/
                        present = Compat(Eptr0, Eptr1);
                        if(present != false) {
                            switch (t.attr.expAttr.op) {
                                case "LT":
                                case "EQ":
                                    Eptr = boolPtr;
                                    break;
                                case "PLUS":
                                case "MINUS":
                                case "TIMES":
                                case "OVER":
                                    Eptr = intPtr;
                                    break;
                            }
                            if(hasEKind) {
                                Ekind = AccessKind.dir;
                            }
                        } else {
                            Console.WriteLine("operator is not compat!");
                        }
                        break;
                }
            }
            return Eptr;
        }

        public void ifstatment(SyntaxTreeNode t) {

            AccessKind Ekind = AccessKind.dir;

            TypeIR Etp = Expr(t.child[0],Ekind,false);
            if(Etp != null) {
                /*处理条件表达式*/
                if (Etp.typeKind != TypeKind.boolTy) {
                    Console.WriteLine("condition expressrion error!");
                } else {
                    SyntaxTreeNode p = t.child[1];
                    while(p!= null) {
                        statement(p);
                        p = p.sibling;
                    }
                    /*处理else语句部分*/
                    t = t.child[2];
                    while(t != null) {
                        statement(t);
                        t = t.sibling;
                    }
                }
            }
        }

        public void whilestatement(SyntaxTreeNode t) {
            AccessKind Ekind = AccessKind.dir;
            TypeIR Etp = Expr(t.child[0], Ekind, false);
            if(Etp != null) {
                Console.WriteLine("condition expression error!\n");
            } else {
                t = t.child[1];
                while(t != null) {
                    statement(t);
                    t = t.sibling;
                }
            }
        }

        public void readstatement(SyntaxTreeNode t) {
            SymTableItem entry = null;
            bool present = false;
            /*用id检查整个符号表*/
            present = FindEntry(t.name[0], entry);
            t.table[0] = entry;
            /*未查到表示变量无声明*/
            if(present == false) {
                Console.WriteLine("not declarationed !");
            } else {
                if(entry.attrIR.typeKind != IdKind.varKind) {
                    Console.WriteLine("not var name !");
                }
            }
        }

        public void writestatement(SyntaxTreeNode t) {
            AccessKind Ekind = AccessKind.dir;
            TypeIR Etp = Expr(t.child[0], Ekind, false);
            if(Etp != null) {
                if(Etp.typeKind == TypeKind.boolTy) {
                    Console.WriteLine("exprssion type error!");
                }
            }
        }

        public void returnstatement(SyntaxTreeNode t) {
            /*如果返回语句出现在主程序中，报错*/
            if (level == 0) {
                Console.WriteLine ("return statement error!");
            }
        }

        public void callstatement(SyntaxTreeNode t) {
            AccessKind Ekind = AccessKind.dir;
            bool present = false;
            SymTableItem entry = new SymTableItem();
            SyntaxTreeNode p;

            present = FindEntry(t.child[0].name[0], entry);
            t.child[0].table[0] = entry;

            if (present == false) {
                Console.WriteLine("function is not declarationed!");
            } else {
                if (FindAttr(entry).typeKind != IdKind.procKind) {
                    Console.WriteLine("is not function name!");
                } else {
                    p = t.child[1];
                    ParamTable param = FindAttr(entry).procAttr.param;
                    while ((p != null) && (param != null)) {
                        SymTableItem paraEntry = param.entry;
                        TypeIR Etp = Expr(p, Ekind, true);
                        if (FindAttr(paraEntry).varAttr.accessKind == AccessKind.indir && (Ekind == AccessKind.dir)) {
                            Console.WriteLine("param kind is not match!");
                        } else {
                            if (FindAttr(paraEntry).idType != Etp) {
                                Console.WriteLine("param type is not match!");
                            }
                        }
                        p = p.sibling;
                        param = param.next;
                        if ((p != null) || param != null) {
                            Console.WriteLine("param num is not match!");
                        }
                    }
                }
            }
        }

        public void assignstatement(SyntaxTreeNode t) {
            SymTableItem entry = new SymTableItem();
            AccessKind Ekind = AccessKind.dir;
            bool present = false;
            TypeIR ptr;
            TypeIR Eptr = null;

            SyntaxTreeNode child1;
            SyntaxTreeNode child2;

            child1 = t.child[1];
            child2 = t.child[2];

            if (child1.child[0] == null) {
                present = FindEntry(child1.name[0], entry);
                if(present != false) {
                    if(FindAttr(entry).typeKind != IdKind.varKind) {
                        Console.WriteLine("is not variable error!");
                        Eptr = null;
                    } else {
                        Eptr = entry.attrIR.idType;
                        child1.table[0] = entry;
                    }
                } else {
                    Console.WriteLine("is not declarations!");
                }
            } else {
                if(child1.attr.expAttr.varKind == SyntaxTreeNode.Attr.ExpAttr.VarKind.ArrayMembV) {
                    Eptr = arrayVar(child1);
                } else {
                    if(child1.attr.expAttr.varKind == SyntaxTreeNode.Attr.ExpAttr.VarKind.FieldMembV) {
                        Eptr = recordVar(child1);
                    }
                }
            }
            if(Eptr != null) {
                if((t.nodeKind == NodeKind.StmLK) && (t.stmtKind == StmtKind.AssignK)) {
                    ptr = Expr(child2, Ekind, false);
                    if (!Compat(ptr, Eptr)) {
                        Console.WriteLine("ass_expression error!");
                    }
                }
            }

        }

        SymTableItem HeadProcess(SyntaxTreeNode t) {
            AttributeIR attr = new AttributeIR();
            bool present = false;
            SymTableItem entry = new SymTableItem();

            attr.typeKind = IdKind.procKind;
            attr.idType = null;
            attr.procAttr.level = level + 1;

            if(t != null) {
                /*登记函数的符号表项*/
                Enter(ref present, attr, entry, t.name[0]);
                t.table[0] = entry;
                /*处理形参声明表*/
            }
            entry.attrIR.procAttr.param = ParaDecList(t);
            return entry;
        }

        public ParamTable ParaDecList(SyntaxTreeNode t) {
            SyntaxTreeNode p = new SyntaxTreeNode();
            ParamTable ptr1 = null;
            ParamTable ptr2 = null;
            ParamTable head = null;
            if (t != null) {
                if (t.child[0] != null) {
                    p = t.child[0];
                }
                CreateTable();/*进入新的局部化区*/
                off = 7;      /*子程序中的变量初始偏移设为8*/
                VarDecPart(p);/*变量声明部分*/
                SymTableItem ptr0 = scope[level];
                /*只要不为空，就访问其兄弟节点*/
                while (ptr0 != null) {
                    /*构造形参符号表，并使其连接至符号表的param项*/
                    ptr2 = NewParam();
                    if (head == null) {
                        head = ptr1 = ptr2;
                    }
                    ptr2.entry = ptr0;
                    ptr2.next = null;
                    if (ptr2 != ptr1) {
                        ptr1.next = ptr2;
                        ptr1 = ptr2;
                    }
                    ptr0 = ptr0.nextItem;
                }
            }
            /*返回形参符号表的头指针*/
            return head;
        }

        public ParamTable NewParam() {
                ParamTable ptr = new ParamTable();
                ptr.entry = null;
                ptr.next = null;
                return ptr;
         }

        public void Enter(ref bool present, AttributeIR attr, SymTableItem entry ,string id) {
            present = false;
            SymTableItem cur = scope[level];
            SymTableItem pre = scope[level];

            if (scope[level] == null) {
                cur = getTableItem();
                scope[level] = cur;
            } else {
                while (cur != null) {
                    pre = cur;
                    if (id == cur.idname) { 
                        present = true;
                        return;
                    } else {
                        cur = pre.nextItem;
                    }
                }
                if (present == false) {
                    cur = getTableItem();
                    pre.nextItem = cur;
                }
            }
            cur.idname = id;
            cur.attrIR.idType = attr.idType;
            cur.attrIR.typeKind = attr.typeKind;
            if (attr.typeKind == IdKind.typeKind) { } else if (attr.typeKind == IdKind.varKind) {
                cur.attrIR.varAttr.level = attr.varAttr.level;
                cur.attrIR.varAttr.off = attr.varAttr.off;
                cur.attrIR.varAttr.accessKind = attr.varAttr.accessKind;
            } else if (attr.typeKind == IdKind.procKind) { 
                cur.attrIR.procAttr.level = attr.procAttr.level;
                cur.attrIR.procAttr.param = attr.procAttr.param;
            }
            entry = cur;
        }

        public TypeIR TypeProcess(SyntaxTreeNode p,DecKind deckind) {
            TypeIR ptr = new TypeIR();
            switch (deckind) {
                case DecKind.IdK:
                    ptr = nameType(p);
                    break;
                case DecKind.ArrayK:
                    ptr = arrayType(p);
                    break;
                case DecKind.RecordK:
                    ptr = recordType(p);
                    break;
                case DecKind.IntegerK:
                    ptr = intPtr;
                    break;
                case DecKind.CharK:
                    ptr = charPtr;
                    break;
            }
            return ptr;
        }

        public bool FindEntry(string id,SymTableItem entry) {
            bool present = false;
            bool result = true;
            int lev = level;
            SymTableItem findentry = scope[lev];
            while((lev != -1) && (present != true)) {
                while((findentry != null) && (present != true)) {
                    result = !(id == findentry.idname);
                    if(result == false) {
                        present = true;
                    } else {
                        findentry = findentry.nextItem;
                    }
                }
                if(present == false) {
                    lev = lev - 1;
                    if (lev < 0) { 
                        findentry = null;
                    } else {
                        findentry = scope[lev];
                    }
                }
            }
            if(present == false) {
                entry = null;
            } else {
                entry = findentry;
            }
            return present;
        }

        TypeIR nameType(SyntaxTreeNode p) {
            TypeIR typeIR = new TypeIR();
            SymTableItem entry = new SymTableItem();
            bool present = FindEntry(p.typeName,entry);
            if (present == true) { 
                if(entry.attrIR.typeKind != IdKind.typeKind) {
                    Console.WriteLine("used before typed!");
                } else {
                    typeIR = entry.attrIR.idType;
                }
            } else {
                Console.WriteLine("type name is not declared!");
            }
            return typeIR;            
        }

        TypeIR arrayType(SyntaxTreeNode p) {
            TypeIR ptr0 = null;
            TypeIR ptr1 = null;
            TypeIR ptr = null;
            if((p.attr.arrayAttr.low) > (p.attr.arrayAttr.up)) {
                Console.WriteLine("array subscript error!");
            } else {
                ptr0 = TypeProcess(p, DecKind.IntegerK);
                ptr1 = TypeProcess(p, p.attr.arrayAttr.childType);
                ptr = NewTy(TypeKind.arrayTy);
                ptr.size = ((p.attr.arrayAttr.up) - ((p.attr.arrayAttr.low) + 1) * ptr1.size);
                ptr.arrayAttr.indexTy = ptr0;
                ptr.arrayAttr.elementType = ptr1;
                ptr.arrayAttr.low = p.attr.arrayAttr.low;
                ptr.arrayAttr.up = p.attr.arrayAttr.up;
            }
            return ptr;
        }

        TypeIR recordType(SyntaxTreeNode p) {
            TypeIR ptr = NewTy(TypeKind.recordTy);
            p = p.child[0];
            FieldChain ptr2 = null;
            FieldChain ptr1 = null;
            FieldChain body = null;
            while (p != null) { 
                for(int i =0;i < p.idnum; i++) {
                    ptr2 = NewBody();
                    if(body == null) {
                        body = ptr1 = ptr2;
                    }
                    ptr2.id = p.name[i];
                    ptr2.unitType = TypeProcess(p, p.decKind);
                    ptr2.next = null;
                    if(ptr2 != ptr1) {
                        ptr2.off = (ptr1.off) + (ptr1.unitType.size);
                        ptr1.next = ptr2;
                        ptr1 = ptr2;
                    }
                }
                p = p.sibling;
            }
            return ptr;
        }

        public void VarDecList(SyntaxTreeNode p) {
            AttributeIR attributeIR = new AttributeIR();
            bool present = false;
            SymTableItem symTableItem = new SymTableItem();
            while (p != null) {
                attributeIR.typeKind = IdKind.varKind;
                for(int i = 0; i < p.idnum; i++) {
                    attributeIR.idType = TypeProcess(p, p.decKind);
                    /*判断识值参还是变参acess(dir,indir)*/
                    if (p.attr != null && p.attr.procAttr != null && p.attr.procAttr.paramt == SyntaxTreeNode.Attr.ProcAttr.ParamType.Varparamtype) {
                        attributeIR.varAttr.accessKind = AccessKind.indir;
                        attributeIR.varAttr.level = level;
                        /*计算形参的偏移*/
                        attributeIR.varAttr.off = off;
                        off += 1;
                    } /*如果是变参，则偏移加1*/
                    else {
                        attributeIR.varAttr.accessKind = AccessKind.dir;
                        attributeIR.varAttr.level = level;
                        /*计算值参的偏移*/
                        if (attributeIR.idType != null) {
                            attributeIR.varAttr.off = off;
                            off = off + (attributeIR.idType.size);
                        }
                    }
                    /*其他情况均为值参，偏移加变量类型的size*/
                    /*登记该变量的属性及名字,并返回其类型内部指针*/
                    Enter(ref present, attributeIR, symTableItem, p.name[i]);
                    if (present) {
                        Console.WriteLine("defined repetation");
                    } else {
                        p.table[i] = symTableItem;
                    }
                }
                if(p != null) {
                    p = p.sibling;
                }
            }
            /*如果是主程序，则记录此时偏移，用于目标代码生成时的displayOff*/
            if (level == 0) {
                mainOff = off;
                StoreNoff = off;
            }
            /*如果不是主程序，则记录此时偏移，用于下面填写过程信息表的noff信息*/
            else {
                savedOff = off;
            }
        }

        public FieldChain NewBody() {
            FieldChain ptr = new FieldChain();
            ptr.next = null;
            ptr.off = 0;
            ptr.unitType = null;
            return ptr;
        }

        public SymTableItem getTableItem() {
            SymTableItem table = new SymTableItem();
            table.attrIR.typeKind = IdKind.typeKind;
            table.attrIR.varAttr.isParam = false;
            table.nextItem = null;
            return table;
        }
        
        public TypeIR NewTy(TypeKind kind) {
            TypeIR table = new TypeIR();
            switch (kind) {
                case TypeKind.intTy:
                case TypeKind.charTy:
                case TypeKind.boolTy:
                    table.typeKind = kind;
                    table.size = 1;
                    break;
                case TypeKind.arrayTy:
                    table.typeKind = TypeKind.arrayTy;
                    table.arrayAttr.indexTy = null;
                    table.arrayAttr.elementType = null;
                    break;
                case TypeKind.recordTy:
                    table.typeKind = TypeKind.recordTy;
                    table.next = null;
                    break;
            }
            return table;
        }

        public void initialize() {
            intPtr = NewTy(TypeKind.intTy);
            charPtr = NewTy(TypeKind.charTy);
            boolPtr = NewTy(TypeKind.boolTy);
        }

        public List<List<String>> PrintSymbTable() {
            int lev = 0;
            List<List<String>> list = new List<List<String>>();
            while (scope[lev] != null) {
                PrintOneLayer(list, lev);
                lev++;
            }
            return list;
        }

        public void PrintOneLayer(List<List<String>> list, int lev) {
            SymTableItem t = scope[lev];
            Console.WriteLine("-------SymbTable  in level " + lev + " ---------");
            while (t != null) {
                List<String> line = new List<String>(new String[7]);
                line[0] = lev.ToString();
                Console.Write(t.idname + "     ");
                line[1] = t.idname;
                AttributeIR attribute = t.attrIR;
                if (attribute != null) {
                    if(attribute.idType == null) {
                        Console.Write("undefined  ");
                        line[3] = "undefined";
                    } else {
                        switch (attribute.idType.typeKind) {
                            case TypeKind.intTy:
                                Console.Write("intTy  ");
                                line[3] = "intTy";
                                break;
                            case TypeKind.charTy:
                                Console.Write("charTy  ");
                                line[3] = "charTy";
                                break;
                            case TypeKind.arrayTy:
                                Console.Write("arrayTy  ");
                                line[3] = "arrayTy";
                                break;
                            case TypeKind.recordTy:
                                Console.Write("recordTy  ");
                                line[3] = "recordTy";
                                break;
                            default:
                                Console.Write("error  type!  ");
                                line[3] = "error type!";
                                break;
                        }
                    }                    
                }
                switch (attribute.typeKind) {
                    case IdKind.typeKind:
                        Console.Write("typekind  ");
                        line[2] = "typekind";
                        break;
                    case IdKind.varKind:
                        Console.Write("varkind  ");
                        line[2] = "varkind";
                        Console.Write("Level " + attribute.varAttr.level + "   ");
                        Console.Write("Offset " + attribute.varAttr.off + "   ");
                        line[5] = attribute.varAttr.off.ToString();
                        switch (attribute.varAttr.accessKind) {
                            case AccessKind.dir:
                                Console.Write("dir  ");
                                line[6] = "dir";
                                break;
                            case AccessKind.indir:
                                Console.Write("indir  ");
                                line[6] = "indir";
                                break;
                        }
                        break;
                    case IdKind.procKind:
                        Console.Write("funckind  ");
                        line[2] = "funckind";
                        Console.Write("level  " + attribute.procAttr.level);
                        Console.Write("Noff  " + attribute.procAttr.nOff);
                        line[4] = attribute.procAttr.nOff.ToString();
                        break;
                    default : Console.Write("error  ");
                        break;
                }
                t = t.nextItem;
                Console.WriteLine("");
                list.Add(line);
            }
        }
    }
}