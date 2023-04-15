using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilationPrinciple {

    public class ClassifiedSymbos {

        // All non-ultimate characters in the SNL language
        public string[] NonUltimate =  {
            "Program","ProgramHead","ProgramName","DeclarePart","TypeDec","TypeDeclaration","TypeDecList","TypeDecMore","TypeId","TypeName",
            "BaseType","StructureType","ArrayType","Low","Top","RecType","FieldDecList","FieldDecMore","IdList","IdMore","VarDec","VarDeclaration",
            "VarDecList","VarDecMore","VarIdList","VarIdMore","ProcDec","ProcDeclaration","ProcDecMore","ProcName","ParamList","ParamDecList",
            "ParamMore","Param","FormList","FidMore","ProcDecPart","ProcBody","ProgramBody","StmList","StmMore","Stm","AssCall","AssignmentRest",
            "ConditionalStm","StmL","LoopStm","InputStm","InVar","OutputStm","ReturnStm","CallStmRest","ActParamList","ActParamMore","RelExp",
            "OtherRelE","Exp","OtherTerm","Term","OtherFactor","Factor","Variable","VariMore","FieldVar","FieldVarMore","CmpOp","AddOp","MultOp"
        };

        // All ultimate characters in the SNL language
        public string[] Ultimate = {
            "ENDFILE","ERROR","PROGRAM","PROCEDURE","TYPE","VAR","IF",
            "THEN","ELSE","FI","WHILE","DO","ENDWH","BEGIN","END","READ","WRITE",
            "ARRAY","OF","RECORD","RETURN","INTEGER_T","CHAR_T","ID","INTC_VAL","CHARC_VAL",
            "ASSIGN","EQ","LT","PLUS","MINUS","TIMES","DIVIDE","LPAREN","RPAREN","DOT",
            "COLON","SEMI","COMMA","LMIDPAREN","RMIDPAREN","UNDERRANGE" 
        };
        

        public bool isInNonUtimate(string str) {
            return NonUltimate.Contains(str);
        }

        public bool isInUtimate(string str) {
            return Ultimate.Contains(str);
        }

    }

    internal class LL1Supporter {
        public LL1Supporter() {
            LL1Table = new int[300, 150];
            createTable();
        }
        void createTable() {
            LL1Table[(int)LexType.Program, (int)LexType.PROGRAM] = 1;
            LL1Table[(int)LexType.ProgramHead, (int)LexType.PROGRAM] = 2;
            LL1Table[(int)LexType.ProgramName, (int)LexType.ID] = 3; 
            LL1Table[(int)LexType.DeclarePart, (int)LexType.TYPE] = 4;
            LL1Table[(int)LexType.DeclarePart, (int)LexType.VAR] = 4;
            LL1Table[(int)LexType.DeclarePart, (int)LexType.PROCEDURE] = 4;
            LL1Table[(int)LexType.DeclarePart, (int)LexType.BEGIN] = 4;
            LL1Table[(int)LexType.TypeDec, (int)LexType.TYPE] = 6;
            LL1Table[(int)LexType.TypeDec, (int)LexType.VAR] = 5;
            LL1Table[(int)LexType.TypeDec, (int)LexType.PROCEDURE] = 5;
            LL1Table[(int)LexType.TypeDec, (int)LexType.BEGIN] = 5;
            LL1Table[(int)LexType.TypeDeclaration, (int)LexType.TYPE] = 7;
            LL1Table[(int)LexType.TypeDecList, (int)LexType.ID] = 8;
            LL1Table[(int)LexType.TypeDecMore, (int)LexType.VAR] = 9;
            LL1Table[(int)LexType.TypeDecMore, (int)LexType.PROCEDURE] = 9;
            LL1Table[(int)LexType.TypeDecMore, (int)LexType.BEGIN] = 9;
            LL1Table[(int)LexType.TypeDecMore, (int)LexType.ID] = 10;
            LL1Table[(int)LexType.TypeId, (int)LexType.ID] = 11;
            LL1Table[(int)LexType.TypeName, (int)LexType.INTEGER_T] = 12;
            LL1Table[(int)LexType.TypeName, (int)LexType.CHAR_T] = 12;
            LL1Table[(int)LexType.TypeName, (int)LexType.ARRAY] = 13;
            LL1Table[(int)LexType.TypeName, (int)LexType.RECORD] = 13;
            LL1Table[(int)LexType.TypeName, (int)LexType.ID] = 14;
            LL1Table[(int)LexType.BaseType, (int)LexType.INTEGER_T] = 15;
            LL1Table[(int)LexType.BaseType, (int)LexType.CHAR_T] = 16;
            LL1Table[(int)LexType.StructureType, (int)LexType.ARRAY] = 17;
            LL1Table[(int)LexType.StructureType, (int)LexType.RECORD] = 18;
            LL1Table[(int)LexType.ArrayType, (int)LexType.ARRAY] = 19;
            LL1Table[(int)LexType.Low, (int)LexType.INTC_VAL] = 20;
            LL1Table[(int)LexType.Top, (int)LexType.INTC_VAL] = 21;
            LL1Table[(int)LexType.RecType, (int)LexType.RECORD] = 22;
            LL1Table[(int)LexType.FieldDecList, (int)LexType.INTEGER_T] = 23;
            LL1Table[(int)LexType.FieldDecList, (int)LexType.CHAR_T] = 23;
            LL1Table[(int)LexType.FieldDecList, (int)LexType.ARRAY] = 24;
            LL1Table[(int)LexType.FieldDecMore, (int)LexType.INTEGER_T] = 26;
            LL1Table[(int)LexType.FieldDecMore, (int)LexType.CHAR_T] = 26;
            LL1Table[(int)LexType.FieldDecMore, (int)LexType.ARRAY] = 26;
            LL1Table[(int)LexType.FieldDecMore, (int)LexType.END] = 25;
            LL1Table[(int)LexType.IdList, (int)LexType.ID] = 27;
            LL1Table[(int)LexType.IdMore, (int)LexType.SEMI] = 28;
            LL1Table[(int)LexType.IdMore, (int)LexType.COMMA] = 29;
            LL1Table[(int)LexType.VarDec, (int)LexType.VAR] = 31;
            LL1Table[(int)LexType.VarDec, (int)LexType.PROCEDURE] = 30;
            LL1Table[(int)LexType.VarDec, (int)LexType.BEGIN] = 30;
            LL1Table[(int)LexType.VarDeclaration, (int)LexType.VAR] = 32;
            LL1Table[(int)LexType.VarDecList, (int)LexType.INTEGER_T] = 33;
            LL1Table[(int)LexType.VarDecList, (int)LexType.CHAR_T] = 33;
            LL1Table[(int)LexType.VarDecList, (int)LexType.ARRAY] = 33;
            LL1Table[(int)LexType.VarDecList, (int)LexType.RECORD] = 33;
            LL1Table[(int)LexType.VarDecList, (int)LexType.ID] = 33;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.INTEGER_T] = 35;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.CHAR_T] = 35;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.ARRAY] = 35;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.RECORD] = 35;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.PROCEDURE] = 34;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.BEGIN] = 34;
            LL1Table[(int)LexType.VarDecMore, (int)LexType.ID] = 35;
            LL1Table[(int)LexType.VarIdList, (int)LexType.ID] = 36;
            LL1Table[(int)LexType.VarIdMore, (int)LexType.SEMI] = 37;
            LL1Table[(int)LexType.VarIdMore, (int)LexType.COMMA] = 38;
            LL1Table[(int)LexType.ProcDec, (int)LexType.PROCEDURE] = 40;
            LL1Table[(int)LexType.ProcDec, (int)LexType.BEGIN] = 39;
            LL1Table[(int)LexType.ProcDeclaration, (int)LexType.PROCEDURE] = 41;
            LL1Table[(int)LexType.ProcDecMore, (int)LexType.PROCEDURE] = 43;
            LL1Table[(int)LexType.ProcDecMore, (int)LexType.BEGIN] = 42;
            LL1Table[(int)LexType.ProcName, (int)LexType.ID] = 44;
            LL1Table[(int)LexType.ParamList, (int)LexType.INTEGER_T] = 46;
            LL1Table[(int)LexType.ParamList, (int)LexType.CHAR_T] = 46;
            LL1Table[(int)LexType.ParamList, (int)LexType.ARRAY] = 46;
            LL1Table[(int)LexType.ParamList, (int)LexType.RECORD] = 46;
            LL1Table[(int)LexType.ParamList, (int)LexType.VAR] = 46;
            LL1Table[(int)LexType.ParamList, (int)LexType.ID] = 46;
            LL1Table[(int)LexType.ParamList, (int)LexType.RPAREN] = 45;
            LL1Table[(int)LexType.ParamDecList, (int)LexType.INTEGER_T] = 47;
            LL1Table[(int)LexType.ParamDecList, (int)LexType.CHAR_T] = 47;
            LL1Table[(int)LexType.ParamDecList, (int)LexType.ARRAY] = 47;
            LL1Table[(int)LexType.ParamDecList, (int)LexType.RECORD] = 47;
            LL1Table[(int)LexType.ParamDecList, (int)LexType.VAR] = 47;
            LL1Table[(int)LexType.ParamDecList, (int)LexType.ID] = 47;
            LL1Table[(int)LexType.ParamMore, (int)LexType.SEMI] = 49;
            LL1Table[(int)LexType.ParamMore, (int)LexType.RPAREN] = 48;
            LL1Table[(int)LexType.Param, (int)LexType.INTEGER_T] = 50;
            LL1Table[(int)LexType.Param, (int)LexType.CHAR_T] = 50;
            LL1Table[(int)LexType.Param, (int)LexType.ARRAY] = 50;
            LL1Table[(int)LexType.Param, (int)LexType.RECORD] = 50;
            LL1Table[(int)LexType.Param, (int)LexType.END] = 51;
            LL1Table[(int)LexType.Param, (int)LexType.ID] = 50;
            LL1Table[(int)LexType.Param, (int)LexType.VAR] = 51;
            LL1Table[(int)LexType.FormList, (int)LexType.ID] = 52;
            LL1Table[(int)LexType.FidMore, (int)LexType.SEMI] = 53;
            LL1Table[(int)LexType.FidMore, (int)LexType.COMMA] = 54;
            LL1Table[(int)LexType.FidMore, (int)LexType.RPAREN] = 53;
            LL1Table[(int)LexType.ProcDecPart, (int)LexType.TYPE] = 55;
            LL1Table[(int)LexType.ProcDecPart, (int)LexType.VAR] = 55;
            LL1Table[(int)LexType.ProcDecPart, (int)LexType.PROCEDURE] = 55;
            LL1Table[(int)LexType.ProcDecPart, (int)LexType.BEGIN] = 55;
            LL1Table[(int)LexType.ProcBody, (int)LexType.BEGIN] = 56;
            LL1Table[(int)LexType.ProgramBody, (int)LexType.BEGIN] = 57;
            LL1Table[(int)LexType.StmList, (int)LexType.IF] = 58;
            LL1Table[(int)LexType.StmList, (int)LexType.WHILE] = 58;
            LL1Table[(int)LexType.StmList, (int)LexType.READ] = 58;
            LL1Table[(int)LexType.StmList, (int)LexType.WRITE] = 58;
            LL1Table[(int)LexType.StmList, (int)LexType.RETURN] = 58;
            LL1Table[(int)LexType.StmList, (int)LexType.ID] = 58;
            LL1Table[(int)LexType.StmMore, (int)LexType.END] = 59;
            LL1Table[(int)LexType.StmMore, (int)LexType.ELSE] = 59;
            LL1Table[(int)LexType.StmMore, (int)LexType.FI] = 59;
            LL1Table[(int)LexType.StmMore, (int)LexType.ENDWH] = 59;
            LL1Table[(int)LexType.StmMore, (int)LexType.SEMI] = 60;
            LL1Table[(int)LexType.Stm, (int)LexType.IF] = 61;
            LL1Table[(int)LexType.Stm, (int)LexType.WHILE] = 62;
            LL1Table[(int)LexType.Stm, (int)LexType.READ] = 63;
            LL1Table[(int)LexType.Stm, (int)LexType.WRITE] = 64;
            LL1Table[(int)LexType.Stm, (int)LexType.RETURN] = 65;
            LL1Table[(int)LexType.Stm, (int)LexType.ID] = 66;
            LL1Table[(int)LexType.AssCall, (int)LexType.ASSIGN] = 67;
            LL1Table[(int)LexType.AssCall, (int)LexType.DOT] = 67;
            LL1Table[(int)LexType.AssCall, (int)LexType.LMIDPAREN] = 67;
            LL1Table[(int)LexType.AssCall, (int)LexType.LPAREN] = 68;
            LL1Table[(int)LexType.AssignmentRest, (int)LexType.DOT] = 69;
            LL1Table[(int)LexType.AssignmentRest, (int)LexType.LMIDPAREN] = 69;
            LL1Table[(int)LexType.AssignmentRest, (int)LexType.ASSIGN] = 69;
            LL1Table[(int)LexType.ConditionalStm, (int)LexType.IF] = 70;
            LL1Table[(int)LexType.LoopStm, (int)LexType.WHILE] = 71;
            LL1Table[(int)LexType.InputStm, (int)LexType.READ] = 72;
            LL1Table[(int)LexType.InVar, (int)LexType.ID] = 73;
            LL1Table[(int)LexType.OutputStm, (int)LexType.WRITE] = 74;
            LL1Table[(int)LexType.ReturnStm, (int)LexType.RETURN] = 75;
            LL1Table[(int)LexType.CallStmRest, (int)LexType.LPAREN] = 76;
            LL1Table[(int)LexType.ActParamList, (int)LexType.INTC_VAL] = 78;
            LL1Table[(int)LexType.ActParamList, (int)LexType.ID] = 78;
            LL1Table[(int)LexType.ActParamList, (int)LexType.LPAREN] = 78;
            LL1Table[(int)LexType.ActParamList, (int)LexType.RPAREN] = 77;
            LL1Table[(int)LexType.ActParamMore, (int)LexType.COMMA] = 80;
            LL1Table[(int)LexType.ActParamMore, (int)LexType.RPAREN] = 79;
            LL1Table[(int)LexType.RelExp, (int)LexType.INTC_VAL] = 81;
            LL1Table[(int)LexType.RelExp, (int)LexType.ID] = 81;
            LL1Table[(int)LexType.RelExp, (int)LexType.LMIDPAREN] = 81;
            LL1Table[(int)LexType.OtherRelE, (int)LexType.LT] = 82;
            LL1Table[(int)LexType.OtherRelE, (int)LexType.EQ] = 82;
            LL1Table[(int)LexType.Exp, (int)LexType.INTC_VAL] = 83;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.END] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.THEN] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.ELSE] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.FI] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.DO] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.ENDWH] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.SEMI] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.COMMA] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.RPAREN] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.RMIDPAREN] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.LT] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.EQ] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.PLUS] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.MINUS] = 87;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.TIMES] = 88;
            LL1Table[(int)LexType.OtherFactor, (int)LexType.DIVIDE] = 88;
            LL1Table[(int)LexType.Term, (int)LexType.INTC_VAL] = 86;
            LL1Table[(int)LexType.Factor, (int)LexType.INTC_VAL] = 90;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.END] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.END] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.END] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.END] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.THEN] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.THEN] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.THEN] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.THEN] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.ELSE] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.ELSE] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.ELSE] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.ELSE] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.FI] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.FI] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.FI] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.FI] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.DO] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.DO] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.DO] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.DO] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.ENDWH] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.ENDWH] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.ENDWH] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.ENDWH] = 97;
            LL1Table[(int)LexType.Exp, (int)LexType.ID] = 83;
            LL1Table[(int)LexType.Term, (int)LexType.ID] = 86;
            LL1Table[(int)LexType.Factor, (int)LexType.ID] = 91;
            LL1Table[(int)LexType.Variable, (int)LexType.ID] = 92;
            LL1Table[(int)LexType.FieldVar, (int)LexType.ID] = 96;
            LL1Table[(int)LexType.VariMore, (int)LexType.DOT] = 95;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.SEMI] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.SEMI] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.SEMI] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.SEMI] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.COMMA] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.COMMA] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.COMMA] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.COMMA] = 97;
            LL1Table[(int)LexType.Exp, (int)LexType.LPAREN] = 83;
            LL1Table[(int)LexType.Term, (int)LexType.LPAREN] = 86;
            LL1Table[(int)LexType.Factor, (int)LexType.LPAREN] = 89;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.RPAREN] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.RPAREN] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.RPAREN] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.RPAREN] = 97;
            LL1Table[(int)LexType.VariMore, (int)LexType.LMIDPAREN] = 94;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.LMIDPAREN] = 98;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.RMIDPAREN] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.RMIDPAREN] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.RMIDPAREN] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.RMIDPAREN] = 97;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.LT] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.LT] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.LT] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.LT] = 97;
            LL1Table[(int)LexType.CmpOp, (int)LexType.LT] = 99;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.EQ] = 84;
            LL1Table[(int)LexType.Term, (int)LexType.EQ] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.EQ] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.EQ] = 97;
            LL1Table[(int)LexType.CmpOp, (int)LexType.EQ] = 100;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.PLUS] = 85;
            LL1Table[(int)LexType.Term, (int)LexType.PLUS] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.PLUS] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.PLUS] = 97;
            LL1Table[(int)LexType.AddOp, (int)LexType.PLUS] = 101;
            LL1Table[(int)LexType.OtherTerm, (int)LexType.MINUS] = 85;
            LL1Table[(int)LexType.Term, (int)LexType.MINUS] = 87;
            LL1Table[(int)LexType.VariMore, (int)LexType.MINUS] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.MINUS] = 97;
            LL1Table[(int)LexType.AddOp, (int)LexType.MINUS] = 102;
            LL1Table[(int)LexType.Term, (int)LexType.TIMES] = 88;
            LL1Table[(int)LexType.VariMore, (int)LexType.TIMES] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.TIMES] = 97;
            LL1Table[(int)LexType.MultOp, (int)LexType.TIMES] = 103;
            LL1Table[(int)LexType.Term, (int)LexType.DIVIDE] = 88;
            LL1Table[(int)LexType.VariMore, (int)LexType.DIVIDE] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.DIVIDE] = 97;
            LL1Table[(int)LexType.MultOp, (int)LexType.DIVIDE] = 104;
            LL1Table[(int)LexType.VariMore, (int)LexType.ASSIGN] = 93;
            LL1Table[(int)LexType.FieldVarMore, (int)LexType.ASSIGN] = 97;
            LL1Table[(int)LexType.CmpOp, (int)LexType.LT] = 99;
            LL1Table[(int)LexType.CmpOp, (int)LexType.EQ] = 100;
            LL1Table[(int)LexType.AddOp, (int)LexType.PLUS] = 101;
            LL1Table[(int)LexType.AddOp, (int)LexType.MINUS] = 102;
            LL1Table[(int)LexType.MultOp, (int)LexType.DIVIDE] = 104;
            LL1Table[(int)LexType.MultOp, (int)LexType.TIMES] = 103;
        }

        //the table of ll1 syntax analysis
        public int[,] LL1Table;

    }
}