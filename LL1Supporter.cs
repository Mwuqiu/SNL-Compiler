using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilationPrinciple {

    // All non-ultimate characters in the SNL language
    public enum NonUltimate {
        Program = 1, ProgramHead, ProgramName, DeclarePart, TypeDec, TypeDeclaration, TypeDecList, TypeDecMore, TypeId, TypeName,
        BaseType, StructureType, ArrayType, Low, Top, RecType, FieldDecList, FieldDecMore, IdList, IdMore, VarDec, VarDeclaration,
        VarDecList, VarDecMore, VarIdList, VarIdMore, ProcDec, ProcDeclaration, ProcDecMore, ProcName, ParamList, ParamDecList,
        ParamMore, Param, FormList, FidMore, ProcDecPart, ProcBody, ProgramBody, StmList, StmMore, Stm, AssCall, AssignmentRest,
        ConditionalStm, StmL, LoopStm, InputStm, InVar, OutputStm, ReturnStm, CallStmRest, ActParamList, ActParamMore, RelExp,
        OtherRelE, Exp, OtherTerm, Term, OtherFactor, Factor, Variable, VariMore, FieldVar, FieldVarMore, CmpOp, AddOp, MultOp,
    }


    // All ultimate characters in the SNL language
    public enum Ultimate {
        ENDFILE, ERROR,PROGRAM, PROCEDURE, TYPE, VAR, IF,
        THEN, ELSE, FI, WHILE, DO,ENDWH, BEGIN, END, READ, WRITE,
        ARRAY, OF, RECORD, RETURN,INTEGER_T, CHAR_T,ID, INTC_VAL, CHARC_VAL,
        ASSIGN, EQ, LT, PLUS, MINUS,TIMES, DIVIDE, LPAREN, RPAREN, DOT,
        COLON, SEMI, COMMA, LMIDPAREN, RMIDPAREN,UNDERRANGE
    }

    internal class LL1Supporter {
        
        public LL1Supporter() {
            LL1Table = new int[300, 150];
            createTable();
        }

        void createTable() {
            LL1Table[(int)NonUltimate.Program, (int)LexType.PROGRAM] = 1;            
            LL1Table[(int)NonUltimate.ProgramHead, (int)LexType.PROGRAM] = 2;
            LL1Table[(int)NonUltimate.ProgramName, (int)LexType.ID] = 3;
            LL1Table[(int)NonUltimate.DeclarePart, (int)LexType.TYPE] = 4;
            LL1Table[(int)NonUltimate.DeclarePart, (int)LexType.VAR] = 4;
            LL1Table[(int)NonUltimate.DeclarePart, (int)LexType.PROCEDURE] = 4;
            LL1Table[(int)NonUltimate.DeclarePart, (int)LexType.BEGIN] = 4;
            LL1Table[(int)NonUltimate.TypeDec, (int)LexType.TYPE] = 6;
            LL1Table[(int)NonUltimate.TypeDec, (int)LexType.VAR] = 5;
            LL1Table[(int)NonUltimate.TypeDec, (int)LexType.PROCEDURE] = 5;
            LL1Table[(int)NonUltimate.TypeDec, (int)LexType.BEGIN] = 5;
            LL1Table[(int)NonUltimate.TypeDeclaration, (int)LexType.TYPE] = 7;
            LL1Table[(int)NonUltimate.TypeDecList, (int)LexType.ID] = 8;
            LL1Table[(int)NonUltimate.TypeDecMore, (int)LexType.VAR] = 9;
            LL1Table[(int)NonUltimate.TypeDecMore, (int)LexType.PROCEDURE] = 9;
            LL1Table[(int)NonUltimate.TypeDecMore, (int)LexType.BEGIN] = 9;
            LL1Table[(int)NonUltimate.TypeDecMore, (int)LexType.ID] = 10;
            LL1Table[(int)NonUltimate.TypeId, (int)LexType.ID] = 11;
            LL1Table[(int)NonUltimate.TypeName, (int)LexType.INTEGER_T] = 12;
            LL1Table[(int)NonUltimate.TypeName, (int)LexType.CHAR_T] = 12;
            LL1Table[(int)NonUltimate.TypeName, (int)LexType.ARRAY] = 13;
            LL1Table[(int)NonUltimate.TypeName, (int)LexType.RECORD] = 13;
            LL1Table[(int)NonUltimate.TypeName, (int)LexType.ID] = 14;
            LL1Table[(int)NonUltimate.BaseType, (int)LexType.INTEGER_T] = 15;
            LL1Table[(int)NonUltimate.BaseType, (int)LexType.CHAR_T] = 16;
            LL1Table[(int)NonUltimate.StructureType, (int)LexType.ARRAY] = 17;
            LL1Table[(int)NonUltimate.StructureType, (int)LexType.RECORD] = 18;
            LL1Table[(int)NonUltimate.ArrayType, (int)LexType.ARRAY] = 19;
            LL1Table[(int)NonUltimate.Low, (int)LexType.INTC_VAL] = 20;
            LL1Table[(int)NonUltimate.Top, (int)LexType.INTC_VAL] = 21;
            LL1Table[(int)NonUltimate.RecType, (int)LexType.RECORD] = 22;
            LL1Table[(int)NonUltimate.FieldDecList, (int)LexType.INTEGER_T] = 23;
            LL1Table[(int)NonUltimate.FieldDecList, (int)LexType.CHAR_T] = 23;
            LL1Table[(int)NonUltimate.FieldDecList, (int)LexType.ARRAY] = 24;
            LL1Table[(int)NonUltimate.FieldDecMore, (int)LexType.INTEGER_T] = 26;
            LL1Table[(int)NonUltimate.FieldDecMore, (int)LexType.CHAR_T] = 26;
            LL1Table[(int)NonUltimate.FieldDecMore, (int)LexType.ARRAY] = 26;
            LL1Table[(int)NonUltimate.FieldDecMore, (int)LexType.END] = 25;
            LL1Table[(int)NonUltimate.IdList, (int)LexType.ID] = 27;
            LL1Table[(int)NonUltimate.IdMore, (int)LexType.SEMI] = 28;
            LL1Table[(int)NonUltimate.IdMore, (int)LexType.COMMA] = 29;
            LL1Table[(int)NonUltimate.VarDec, (int)LexType.VAR] = 31;
            LL1Table[(int)NonUltimate.VarDec, (int)LexType.PROCEDURE] = 30;
            LL1Table[(int)NonUltimate.VarDec, (int)LexType.BEGIN] = 30;
            LL1Table[(int)NonUltimate.VarDeclaration, (int)LexType.VAR] = 32;
            LL1Table[(int)NonUltimate.VarDecList, (int)LexType.INTEGER_T] = 33;
            LL1Table[(int)NonUltimate.VarDecList, (int)LexType.CHAR_T] = 33;
            LL1Table[(int)NonUltimate.VarDecList, (int)LexType.ARRAY] = 33;
            LL1Table[(int)NonUltimate.VarDecList, (int)LexType.RECORD] = 33;
            LL1Table[(int)NonUltimate.VarDecList, (int)LexType.ID] = 33;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.INTEGER_T] = 35;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.CHAR_T] = 35;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.ARRAY] = 35;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.RECORD] = 35;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.PROCEDURE] = 34;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.BEGIN] = 34;
            LL1Table[(int)NonUltimate.VarDecMore, (int)LexType.ID] = 35;
            LL1Table[(int)NonUltimate.VarIdList, (int)LexType.ID] = 36;
            LL1Table[(int)NonUltimate.VarIdMore, (int)LexType.SEMI] = 37;
            LL1Table[(int)NonUltimate.VarIdMore, (int)LexType.COMMA] = 38;
            LL1Table[(int)NonUltimate.ProcDec, (int)LexType.PROCEDURE] = 40;
            LL1Table[(int)NonUltimate.ProcDec, (int)LexType.BEGIN] = 39;
            LL1Table[(int)NonUltimate.ProcDeclaration, (int)LexType.PROCEDURE] = 41;
            LL1Table[(int)NonUltimate.ProcDecMore, (int)LexType.PROCEDURE] = 41;
            LL1Table[(int)NonUltimate.ProcDecMore, (int)LexType.BEGIN] = 42;
            LL1Table[(int)NonUltimate.ProcName, (int)LexType.ID] = 44;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.INTEGER_T] = 46;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.CHAR_T] = 46;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.ARRAY] = 46;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.RECORD] = 46;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.VAR] = 46;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.ID] = 46;
            LL1Table[(int)NonUltimate.ParamList, (int)LexType.RMIDPAREN] = 45;
            LL1Table[(int)NonUltimate.ParamDecList, (int)LexType.INTEGER_T] = 47;
            LL1Table[(int)NonUltimate.ParamDecList, (int)LexType.CHAR_T] = 47;
            LL1Table[(int)NonUltimate.ParamDecList, (int)LexType.ARRAY] = 47;
            LL1Table[(int)NonUltimate.ParamDecList, (int)LexType.RECORD] = 47;
            LL1Table[(int)NonUltimate.ParamDecList, (int)LexType.VAR] = 47;
            LL1Table[(int)NonUltimate.ParamDecList, (int)LexType.ID] = 47;
            LL1Table[(int)NonUltimate.ParamMore, (int)LexType.SEMI] = 49;
            LL1Table[(int)NonUltimate.ParamMore, (int)LexType.RPAREN] = 48;
            LL1Table[(int)NonUltimate.Param, (int)LexType.INTEGER_T] = 50;
            LL1Table[(int)NonUltimate.Param, (int)LexType.CHAR_T] = 50;
            LL1Table[(int)NonUltimate.Param, (int)LexType.ARRAY] = 50;
            LL1Table[(int)NonUltimate.Param, (int)LexType.RECORD] = 50;
            LL1Table[(int)NonUltimate.Param, (int)LexType.END] = 51;
            LL1Table[(int)NonUltimate.Param, (int)LexType.ID] = 50;
            LL1Table[(int)NonUltimate.Param, (int)LexType.VAR] = 51;
            LL1Table[(int)NonUltimate.FormList, (int)LexType.ID] = 52;
            LL1Table[(int)NonUltimate.FidMore, (int)LexType.SEMI] = 53;
            LL1Table[(int)NonUltimate.FidMore, (int)LexType.COMMA] = 54;
            LL1Table[(int)NonUltimate.FidMore, (int)LexType.RPAREN] = 53;
            LL1Table[(int)NonUltimate.ProcDecPart, (int)LexType.TYPE] = 55;
            LL1Table[(int)NonUltimate.ProcDecPart, (int)LexType.VAR] = 55;
            LL1Table[(int)NonUltimate.ProcDecPart, (int)LexType.PROCEDURE] = 55;
            LL1Table[(int)NonUltimate.ProcDecPart, (int)LexType.BEGIN]  = 55;
            LL1Table[(int)NonUltimate.ProcBody, (int)LexType.BEGIN] = 56;
            LL1Table[(int)NonUltimate.ProgramBody, (int)LexType.BEGIN] = 57;
            LL1Table[(int)NonUltimate.StmList, (int)LexType.IF] = 58;
            LL1Table[(int)NonUltimate.StmList, (int)LexType.WHILE] = 58;
            LL1Table[(int)NonUltimate.StmList, (int)LexType.READ] = 58;
            LL1Table[(int)NonUltimate.StmList, (int)LexType.WRITE] = 58;
            LL1Table[(int)NonUltimate.StmList, (int)LexType.RETURN] = 58;
            LL1Table[(int)NonUltimate.StmList, (int)LexType.ID] = 58;
            LL1Table[(int)NonUltimate.StmMore, (int)LexType.END] = 59;
            LL1Table[(int)NonUltimate.StmMore, (int)LexType.ELSE] = 59;
            LL1Table[(int)NonUltimate.StmMore, (int)LexType.FI] = 59;
            LL1Table[(int)NonUltimate.StmMore, (int)LexType.ENDWH] = 59;
            LL1Table[(int)NonUltimate.StmMore, (int)LexType.SEMI] = 60;
            LL1Table[(int)NonUltimate.Stm, (int)LexType.IF] = 61;
            LL1Table[(int)NonUltimate.Stm, (int)LexType.WHILE] = 62;
            LL1Table[(int)NonUltimate.Stm, (int)LexType.READ] = 63;
            LL1Table[(int)NonUltimate.Stm, (int)LexType.WRITE] = 64;
            LL1Table[(int)NonUltimate.Stm, (int)LexType.RETURN] = 65;
            LL1Table[(int)NonUltimate.Stm, (int)LexType.ID] = 66;
            LL1Table[(int)NonUltimate.AssCall, (int)LexType.ASSIGN] = 67;
            LL1Table[(int)NonUltimate.AssCall, (int)LexType.DOT] = 67;
            LL1Table[(int)NonUltimate.AssCall, (int)LexType.LMIDPAREN] = 67;
            LL1Table[(int)NonUltimate.AssCall, (int)LexType.LPAREN] = 68;
            LL1Table[(int)NonUltimate.AssignmentRest, (int)LexType.DOT] = 69;
            LL1Table[(int)NonUltimate.AssignmentRest, (int)LexType.LMIDPAREN] = 69;
            LL1Table[(int)NonUltimate.AssignmentRest, (int)LexType.ASSIGN] = 69;
            LL1Table[(int)NonUltimate.ConditionalStm, (int)LexType.IF] = 70;
            LL1Table[(int)NonUltimate.LoopStm, (int)LexType.WHILE] = 71;
            LL1Table[(int)NonUltimate.InputStm, (int)LexType.READ] = 72;
            LL1Table[(int)NonUltimate.InVar, (int)LexType.ID] = 73;
            LL1Table[(int)NonUltimate.OutputStm, (int)LexType.WRITE] = 74;
            LL1Table[(int)NonUltimate.ReturnStm, (int)LexType.RETURN] = 75;
            LL1Table[(int)NonUltimate.CallStmRest, (int)LexType.LPAREN] = 76;
            LL1Table[(int)NonUltimate.ActParamList, (int)LexType.INTC_VAL] = 78;
            LL1Table[(int)NonUltimate.ActParamList, (int)LexType.ID] = 78;
            LL1Table[(int)NonUltimate.ActParamList, (int)LexType.LPAREN] = 78;
            LL1Table[(int)NonUltimate.ActParamList, (int)LexType.RPAREN] = 77;
            LL1Table[(int)NonUltimate.ActParamMore, (int)LexType.COMMA] = 80;
            LL1Table[(int)NonUltimate.ActParamMore, (int)LexType.RPAREN] = 79;
            LL1Table[(int)NonUltimate.RelExp, (int)LexType.INTC_VAL] = 81;
            LL1Table[(int)NonUltimate.RelExp, (int)LexType.ID] = 81;
            LL1Table[(int)NonUltimate.RelExp, (int)LexType.LMIDPAREN] = 81;
            LL1Table[(int)NonUltimate.OtherRelE, (int)LexType.LT] = 82;
            LL1Table[(int)NonUltimate.OtherRelE, (int)LexType.EQ] = 82;
            LL1Table[(int)NonUltimate.Exp, (int)LexType.INTC_VAL] = 83;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.END] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.THEN] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.ELSE] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.FI] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.DO] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.ENDWH] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.SEMI] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.COMMA] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.RPAREN] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.RMIDPAREN] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.LT] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.EQ] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.PLUS] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.MINUS] = 87;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.TIMES] = 88;
            LL1Table[(int)NonUltimate.OtherFactor, (int)LexType.DIVIDE] = 88;
            LL1Table[(int)NonUltimate.Term, (int)LexType.INTC_VAL] = 86;
            LL1Table[(int)NonUltimate.Factor, (int)LexType.INTC_VAL] = 90;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.END] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.END] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.END] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.END] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.THEN] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.THEN] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.THEN] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.THEN] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.ELSE] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.ELSE] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.ELSE] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.ELSE] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.FI] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.FI] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.FI] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.FI] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.DO] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.DO] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.DO] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.DO] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.ENDWH] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.ENDWH] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.ENDWH] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.ENDWH] = 97;
            LL1Table[(int)NonUltimate.Exp, (int)LexType.ID] = 83;
            LL1Table[(int)NonUltimate.Term, (int)LexType.ID] = 86;
            LL1Table[(int)NonUltimate.Factor, (int)LexType.ID] = 91;
            LL1Table[(int)NonUltimate.Variable, (int)LexType.ID] = 92;
            LL1Table[(int)NonUltimate.FieldVar, (int)LexType.ID] = 96;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.DOT] = 95;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.SEMI] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.SEMI] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.SEMI] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.SEMI] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.COMMA] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.COMMA] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.COMMA] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.COMMA] = 97;
            LL1Table[(int)NonUltimate.Exp, (int)LexType.LPAREN] = 83;
            LL1Table[(int)NonUltimate.Term, (int)LexType.LPAREN] = 86;
            LL1Table[(int)NonUltimate.Factor, (int)LexType.LPAREN] = 89;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.RPAREN] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.RPAREN] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.RPAREN] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.RPAREN] = 97;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.LMIDPAREN] = 94;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.LMIDPAREN] = 98;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.RMIDPAREN] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.RMIDPAREN] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.RMIDPAREN] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.RMIDPAREN] = 97;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.LT] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.LT] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.LT] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.LT] = 97;
            LL1Table[(int)NonUltimate.CmpOp, (int)LexType.LT] = 99;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.EQ] = 84;
            LL1Table[(int)NonUltimate.Term, (int)LexType.EQ] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.EQ] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.EQ] = 97;
            LL1Table[(int)NonUltimate.CmpOp, (int)LexType.EQ] = 100;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.PLUS] = 85;
            LL1Table[(int)NonUltimate.Term, (int)LexType.PLUS] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.PLUS] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.PLUS] = 97;
            LL1Table[(int)NonUltimate.AddOp, (int)LexType.PLUS] = 101;
            LL1Table[(int)NonUltimate.OtherTerm, (int)LexType.MINUS] = 85;
            LL1Table[(int)NonUltimate.Term, (int)LexType.MINUS] = 87;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.MINUS] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.MINUS] = 97;
            LL1Table[(int)NonUltimate.AddOp, (int)LexType.MINUS] = 102;
            LL1Table[(int)NonUltimate.Term, (int)LexType.TIMES] = 88;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.TIMES] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.TIMES] = 97;
            LL1Table[(int)NonUltimate.MultOp, (int)LexType.TIMES] = 103;
            LL1Table[(int)NonUltimate.Term, (int)LexType.DIVIDE] = 88;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.DIVIDE] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.DIVIDE] = 97;
            LL1Table[(int)NonUltimate.MultOp, (int)LexType.DIVIDE] = 104;
            LL1Table[(int)NonUltimate.VariMore, (int)LexType.ASSIGN] = 93;
            LL1Table[(int)NonUltimate.FieldVarMore, (int)LexType.ASSIGN] = 97;
            LL1Table[(int)NonUltimate.CmpOp, (int)LexType.LT] = 99;
            LL1Table[(int)NonUltimate.CmpOp, (int)LexType.EQ] = 100;
            LL1Table[(int)NonUltimate.AddOp, (int)LexType.PLUS] = 101;
            LL1Table[(int)NonUltimate.AddOp, (int)LexType.MINUS] = 102;
            LL1Table[(int)NonUltimate.MultOp, (int)LexType.DIVIDE] = 104;
            LL1Table[(int)NonUltimate.MultOp, (int)LexType.TIMES] = 103;
        }

        //the table of ll1 syntax analysis
        public int[,] LL1Table;

    }
}