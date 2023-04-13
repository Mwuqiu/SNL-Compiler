using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilationPrinciple {

    public enum TypeKind { intTy, charTy, arrayTy, recordTy, boolTy}

    public enum IdKind { typeKind, varKind, procKind }

    public enum AccessKind { dir, indir }

    public class IntPtr {
        public int size { get; set; }
        public TypeKind kind { get; set; }
    }

    public class CharPtr {
        public int size { get; set; }
        public TypeKind kind { get; set; }
    }

    public class BoolPtr {
        public int size { get; set; }
        public TypeKind kind { get; set; }
    }

    public class ArrayPtr {
        int size;
        TypeKind kind;
        TypeKind indexType;
        TypeKind elementType;
    }

    public class RecordPtr {
        int size;
        TypeKind kind;
        List<FieldChain> recordBody;
    }

    public class FieldChain {
        public string id { get; set; }
        public int off { get; set; }
        public TypeIR unitType { get; set; }
        public FieldChain next { get; set; }
    }       

    public class TypeIR {

        public TypeIR() {
            arrayAttr = new ArrayAttr();
        }

        public int size { get; set; }
        public TypeKind typeKind { get; set; }

        public class ArrayAttr {
            public TypeIR indexTy;
            public TypeIR elementType;
            public int low;
            public int up;
        }

        public ArrayAttr arrayAttr;

        public FieldChain next { get; set; }
    }

    public class ParamTable {
        public SymTableItem entry { get; set; }
        public ParamTable next { get; set; }

        public ParamTable() {
            entry = new SymTableItem();
            next = null;
        }
    }

    public class AttributeIR {      
        
        public AttributeIR() {
            varAttr = new VarAttr();
            procAttr = new ProcAttr();
            idType = new TypeIR();
        }

        public class VarAttr {
            public AccessKind accessKind;
            public int level;
            public int off;
            public bool isParam;
        }

        public class ProcAttr {
            public ParamTable param;
            public int level;
            public int code;
            public int size;
            public int mOff;      /*过程活动记录的大小*/
            public int nOff;  	   /*sp到display表的偏移量*/
        }

        public VarAttr varAttr;
        public ProcAttr procAttr;

        public TypeIR idType; // int char bool array record
        public IdKind typeKind;  // type var proc
    }

    public class SymTableItem {
        public string idname { get; set; }
        public AttributeIR attrIR { get; set; }
        public SymTableItem ? nextItem { get; set; }

        public SymTableItem() {
            attrIR = new AttributeIR();
            idname = "";
            nextItem = null;
        }

        public void copyItem(SymTableItem item) {
            this.idname = item.idname;
            this.attrIR = item.attrIR;
            this.nextItem = item.nextItem;
        }
    }
}
