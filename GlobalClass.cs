using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CompilationPrinciple
{
    public enum LexType
    {
        //簿记单词符号
        ENDFILE, ERROR,
        //保留字
        PROGRAM, PROCEDURE, TYPE, VAR, IF,
        THEN, ELSE, FI, WHILE, DO,
        ENDWH, BEGIN, END, READ, WRITE,
        ARRAY, OF, RECORD, RETURN,
        //类型
        INTEGER_T, CHAR_T,
        //多字符单词符号
        ID, INTC_VAL, CHARC_VAL,
        //特殊符号
        ASSIGN, EQ, LT, PLUS, MINUS,
        TIMES, DIVIDE, LPAREN, RPAREN, DOT,
        COLON, SEMI, COMMA, LMIDPAREN, RMIDPAREN,
        UNDERRANGE,
        //非终结符
        Program, ProgramHead, ProgramName, DeclarePart,
        TypeDec, TypeDeclaration, TypeDecList, TypeDecMore,
        TypeId, TypeName, BaseType, StructureType,
        ArrayType, Low, Top, RecType,
        FieldDecList, FieldDecMore, IdList, IdMore,
        VarDec, VarDeclaration, VarDecList, VarDecMore,
        VarIdList, VarIdMore, ProcDec, ProcDeclaration,
        ProcDecMore, ProcName, ParamList, ParamDecList,
        ParamMore, Param, FormList, FidMore,
        ProcDecPart, ProcBody, ProgramBody, StmList,
        StmMore, Stm, AssCall, AssignmentRest,
        ConditionalStm, StmL, LoopStm, InputStm,
        InVar, OutputStm, ReturnStm, CallStmRest,
        ActParamList, ActParamMore, RelExp, OtherRelE,
        Exp, OtherTerm, Term, OtherFactor,
        Factor, Variable, VariMore, FieldVar,
        FieldVarMore, CmpOp, AddOp, MultOp
    };
    
    public class Token
    {
        public int line { get; set; } //行数
        public LexType lex { get; set; } //词法信息
        public String sem { get; set; } //语义信息
    }

    public class Dictionarys
    {
        public Dictionarys()
        {
            //Provide the conversion from identifier to identifier type 
            reservedWords = new Dictionary<String, LexType>()
            {
                {"program",LexType.PROGRAM},{"type", LexType.TYPE},
                {"var", LexType.VAR},         {"procedure", LexType.PROCEDURE},
                {"begin", LexType.BEGIN},     {"end", LexType.END},
                {"array", LexType.ARRAY},     {"of", LexType.OF},
                {"record", LexType.RECORD},   {"if", LexType.IF},
                {"then", LexType.THEN},       {"else",LexType.ELSE},
                {"fi", LexType.FI},           {"char", LexType.CHAR_T},
                {"while", LexType.WHILE},     {"do", LexType.DO},
                {"endwh", LexType.ENDWH},     {"read", LexType.READ},
                {"write", LexType.WRITE},     {"return", LexType.RETURN},
                {"integer", LexType.INTEGER_T},
            };

            //Provide the conversion from separatorWords to separatorWords type 
            separatorWords = new Dictionary<char, LexType>() {
                {'+', LexType.PLUS},   {'-', LexType.MINUS},     {'*', LexType.TIMES},
                {'/', LexType.DIVIDE}, {'(', LexType.LPAREN},    {')', LexType.RPAREN},
                {';', LexType.SEMI},   {'[', LexType.LMIDPAREN}, {']', LexType.RMIDPAREN},
                {'=', LexType.EQ},     {'<', LexType.LT},        {',', LexType.COMMA}                            
            };
        }

        public Dictionary<String,LexType> reservedWords;

        public Dictionary<char, LexType> separatorWords;
    }

}
