using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, PlusOp, MinusOp, MultiplyOp, DivideOp, String,
    Idenifier, Constant, Comment, LeftBraces, RightBraces, Repeat, Assign, Endline, Return, Float
}
namespace JASON_Compiler
{

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INT", Token_Class.Integer);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);
            ReservedWords.Add("REPEAT", Token_Class.Repeat);
            ReservedWords.Add("ENDLINE", Token_Class.Endline);
            ReservedWords.Add("RETURN", Token_Class.Return);
            ReservedWords.Add("FLOAT", Token_Class.Float);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("/**/", Token_Class.Comment);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces); 
        }

        public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //identifier
                {
                    while (CurrentChar >= 'a' && CurrentChar <= 'z'|| CurrentChar >= 'A' && CurrentChar <= 'Z'||CurrentChar=='_' || CurrentChar >= '0' && CurrentChar <= '9')
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        if (CurrentChar >= 'a' && CurrentChar <= 'z' || CurrentChar >= 'A' && CurrentChar <= 'Z' || CurrentChar == '_' || CurrentChar >= '0' && CurrentChar <= '9')
                        {
                            CurrentLexeme += CurrentChar;
                        }
                    }
                    i = j - 1;
                    
                    FindTokenClass(CurrentLexeme.ToUpper().ToUpper());
                }

                else if(CurrentChar >= '0' && CurrentChar <= '9')//number
                {
                    while (CurrentChar == '.' || CurrentChar >= '0' && CurrentChar <= '9')
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '.' || CurrentChar >= '0' && CurrentChar <= '9')
                        {
                            CurrentLexeme += CurrentChar;
                        }
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else if(CurrentChar == '{')
                {
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else if (CurrentChar == '}')
                {
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else if (CurrentChar == '(')
                {
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else if (CurrentChar == ')')
                {
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else if (CurrentChar=='"')
                {
                    j++;
                    while (SourceCode[j]!='"')
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar;
                        j++;
                    }
                    CurrentLexeme += '"';
                    j++;
                    i = j - 1;
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else if (SourceCode[i]=='/'&&SourceCode[i+1]=='*')//comment
                {
                    bool found = true;
                    while (!(SourceCode[j] == '*' && SourceCode[j+1] == '/'))
                    {
                        j++;
                        if (j >= SourceCode.Length)
                        {
                            Errors.Error_List.Add(CurrentLexeme.ToUpper());
                            found=false;
                            break;
                        }
                        CurrentChar = SourceCode[j];
                        if (!(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                        {
                            CurrentLexeme += CurrentChar;
                        }
                    }
                    if (found==true)
                    {
                        j += 2;
                        i = j - 1;
                        CurrentLexeme += "*/";
                        FindTokenClass(CurrentLexeme.ToUpper());
                    }
                }

                else if (CurrentLexeme == ":" && SourceCode[i + 1] == '=')
                {
                    CurrentLexeme += "=";
                    i++;
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                else
                {
                    FindTokenClass(CurrentLexeme.ToUpper());
                }

                if (j == SourceCode.Length)
                {
                    break;
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            bool eror = false;
            //Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (isReservedWord(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
            }
            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;
            }
            //Is it an operator?
            else if (isOperator(Lex))
            {
                Tok.token_type = Operators[Lex];
            }
            //Is it an comment?
            else if (isComment(Lex))
            {
                Tok.token_type = Operators["/**/"];
            }
            //Is it an string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
            }
            else
            {
                Errors.Error_List.Add(Lex);
                eror = true;
            }
            if (eror == false)
            {
                Tokens.Add(Tok);   
            }
        }
        bool isComment(string lex)
        {
            if (lex[0]=='/'&&lex[1]=='*'&&lex[lex.Length-2]=='*'&&lex[lex.Length-1]=='/')
            {
                return true;
            }
            return false;
        }
        bool isReservedWord(string lex)
        {
            for (int i = 0; i < ReservedWords.Count; i++)
            {
                if (ReservedWords.Keys.Contains(lex))
                {
                    return true;
                }
            }
            return false;
        }
        bool isOperator(string lex)
        {
            for (int i = 0; i < Operators.Count; i++)
            {
                if (Operators.Keys.Contains(lex))
                {
                    return true;
                }
            }
            return false;
        }
        bool isIdentifier(string lex)
        {
            bool isValid=false;
            if (lex[0] >= 'a' && lex[0] <= 'z' || lex[0] >= 'A' && lex[0] <= 'Z')
            {
                isValid = true;
            }
            
            
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = false;
            int count = 0;
          if(lex[0]>='0'&&lex[0]<='9')
            {
                isValid = true;
            }
          for(int i=1 ; i < lex.Length;i++)
            {
                if (lex[i] == '.')
                {
                    count++;
                    if (count >1)
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            return isValid;
        }
        bool isString(string lex)
        {
            if (lex[0]=='"'&&lex[lex.Length-1]=='"')
            {
                return true;
            }
            return false;
        }
    }
}
