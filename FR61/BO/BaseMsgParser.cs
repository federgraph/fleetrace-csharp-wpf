namespace RiggVar.FR
{
    public enum TMsgParserError
    {
        Error_None,
        Error_Division,
        Error_RunID,
        Error_Bib,
        Error_Command,
        Error_Value,
        Error_Pos,
        Error_Athlete,
        Error_MsgID,
        Error_Race,
    }

    public enum TMsgType
    {
        None,
        Input,
        Param,
        Option,
        Prop,
        Cmd,
        Request,
        Comment,
        Test
    }

    public class TBaseMsgParser
    {
        protected TStringList SL = new TStringList();
        protected TStringList SLCompare = new TStringList();
        protected bool FIsValid;
        protected TMsgParserError FLastError;
        protected string FInput;
        protected string FKey;
        protected string FValue;
        protected string sToken;
        protected string sRest;

        public TMsgType MsgType;

        public string MsgKey => FKey;

        public string MsgValue => FValue;

        public TBaseMsgParser()
        {

        }

        protected virtual void Clear()
        {
            FLastError = TMsgParserError.Error_None;
            FIsValid = false;
            FInput = "";
            FKey = "";
            FValue = "";
        }

        public static string MsgTypeToString(TMsgType value)
        {
            return MsgTypeToChar(value).ToString();
        }

        public static char MsgTypeToChar(TMsgType value)
        {
            switch (value)
            {
                case TMsgType.Input: return 'I';
                case TMsgType.Param: return 'P';
                case TMsgType.Option: return 'O';
                case TMsgType.Prop: return 'E';
                case TMsgType.Cmd: return 'C';
                case TMsgType.Request: return 'R';
                case TMsgType.Comment: return 'K';
                case TMsgType.Test: return 'T';
                default: return 'N';
            }
        }

        public static TMsgType ParseMsgTypeAsEnumDef(string value, TMsgType def)
        {
            TMsgType result = ParseMsgTypeAsEnum(value);
            if (result == TMsgType.None)
            {
                return def;
            }
            else
            {
                return result;
            }
        }

        public static TMsgType ParseMsgTypeAsEnum(string value)
        {
            if (value == null)
            {
                return TMsgType.None;
            }
            else if (value == "")
            {
                return TMsgType.None;
            }
            else
            {
                return ParseMsgTypeAsEnum(value[0]);
            }
        }

        public static TMsgType ParseMsgTypeAsEnum(char value)
        {
            switch (value)
            {
                case 'I': return TMsgType.Input;
                case 'P': return TMsgType.Param;
                case 'O': return TMsgType.Option;
                case 'E': return TMsgType.Prop;
                case 'C': return TMsgType.Cmd;
                case 'R': return TMsgType.Request;
                case 'K': return TMsgType.Comment;
                case 'T': return TMsgType.Test;
                default: return TMsgType.None;
            }
        }

        protected void NextToken()
        {
            sRest = Utils.Cut(".", sRest, ref sToken);
        }

        protected int NextTokenX(string TokenName)
        {
            NextToken();
            int l = TokenName.Length;
            if (Utils.Copy(sToken, 1, l) == TokenName)
            {
                sToken = Utils.Copy(sToken, l+1, sToken.Length - l);
                return Utils.StrToIntDef(sToken, -1);
            }
            return -1;
        }

        protected bool TestTokenName(string TokenName)
        {
            string LongTokenName = LongToken(TokenName);
            return (Utils.Copy(sRest, 1, TokenName.Length) == TokenName) ||
                (Utils.Copy(sRest, 1, LongTokenName.Length) == LongTokenName);
        }

        protected string CompareToken(string t)
        {
            for (int i = 0; i < SLCompare.Count; i++)
            {
                string s = SLCompare[i];
                if ((t == s) || (t == LongToken(s)))
                {
                    return s;
                }
            }
            if (IsProp(t))
            {
                return t;
            }

            return string.Empty;
        }

        protected bool ParseLeaf()
        {
            FIsValid = false;

            //Command
            if (!ParseCommand())
            {
                FLastError = TMsgParserError.Error_Command;
                return false;
            }
            //Value
            if (!ParseValue())
            {
                FLastError = TMsgParserError.Error_Value;
                return false;
            }

            FIsValid = true;
            return true;
        }

        protected virtual bool ParseCommand()
        {
            return false;
        }

        protected virtual bool ParseValue()
        {
            return false;
        }

        public bool Parse(string s)
        {
            SL.Clear();        

            string temp;
            int i = Utils.Pos("=", s);
            if (i > 0)
            {
                string s1 = Utils.Copy(s, 1, i-1);
                s1 = s1.Trim();
                string s2 = Utils.Copy(s, i+1, s.Length);
                s2 = s2.Trim();
                temp = s1 + "=" + s2;
            }
            else
            {
                temp = s.Replace(" ", "");
            }

            if (Utils.Pos("=", temp) == 0)
            {
                temp = temp + "=";
            }

            SL.Add(temp);
            FKey = SL.Names(0);
            FValue = SL.Values(FKey);

            return ParseKeyValue(FKey, FValue);
        }

        protected virtual bool ParseKeyValue(string aKey, string aValue)
        {
            Clear();
            FKey = aKey;
            FValue = aValue;
            FInput = aKey + "=" + aValue;
            sRest = aKey;

            return true;
        }

        public static string MsgParserErrorStrings(TMsgParserError e)
        {
            switch (e)
            {
                case TMsgParserError.Error_Athlete: return "Error_Athlete";
                case TMsgParserError.Error_Bib: return "Error_Bib";
                case TMsgParserError.Error_Command: return "Error_Command";
                case TMsgParserError.Error_Division: return "Error_Division";
                case TMsgParserError.Error_MsgID: return "Error_MsgID";
                case TMsgParserError.Error_None: return "Error_None";
                case TMsgParserError.Error_Pos: return "Error_Pos";
                case TMsgParserError.Error_Race: return "Error_Race";
                case TMsgParserError.Error_RunID: return "Error_RunID";
                case TMsgParserError.Error_Value: return "Error_Value";
                default: return string.Empty;
            }
        }

        protected bool ParsePositiveIntegerValue()
        {
            return Utils.StrToIntDef(FValue, -1) > -1;
        }

        protected bool ParseBooleanValue()
        {
            string s = FValue.ToLower();
            return ((s == "false") || (s == "true"));
        }

        protected bool IsProp(string Token)
        {
            return (Utils.Copy(Token, 1, 5) == "Prop_");
        }

        protected bool IsNameCommand(string Token)
        {
            if (Token == null)
            {
                return false;
            }

            if (Token.Length < 2)
            {
                return false;
            }

            if (! Token.StartsWith("N"))
            {
                return false;
            }

            if (Utils.StrToIntDef(Token.Substring(1), -1) == -1)
            {
                return false;
            }

            return true;
        }

        public virtual string LongToken(string t)
        {
            return t;
        }
    }
}
