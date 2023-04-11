using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilationPrinciple {

    public enum TypeKind {  intTy,charTy,arrayTy,recordTy,boolTy  }

    public struct intPtr {
        public intPtr() {
            size = 1;
            kind = TypeKind.intTy;
        }

        int size;
        TypeKind kind;
    }

    public struct charPtr {

        public charPtr() { 
            size= 1;
            kind= TypeKind.charTy;
        }

        int size;
        TypeKind kind;
    }

    public struct arrayPtr { 
        public arrayPtr(int size,TypeKind indexType,TypeKind elementType) {
            this.size = size;
            this.indexType = indexType;
            this.elementType = elementType;
            this.kind = TypeKind.arrayTy;
        }
        int size;
        TypeKind kind;
        TypeKind indexType;
        TypeKind elementType;
    }

    public struct recordPtr {
        int size;
        TypeKind kind;

    }





    internal class SemanticSupporter {
    }
}
