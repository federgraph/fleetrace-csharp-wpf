using System.Text;

namespace RiggVar.FR
{

    public interface IMsg
    {
        void ReceiveMsg(string s);

        void TokenizeLines(string s);

        void DispatchLines();

        int GetRequestLines(TStrings SL);
        int GetParamLines(TStrings SL);
        int GetOptionLines(TStrings SL);
        int GetPropertyLines(TStrings SL);
        int GetDataLines(TStrings SL);

        void DispatchLine();

        void TokenizeLine(string s);
        void TokenizePath(string s);

        bool IsValid();
        bool HasError();
        string GetPath();
        string GetValue();
        string GetError();

        void DispatchCommand();

        string GetReport();
    }

    public class TMsgLine
    {
        public const int None = 0;
        public const int Request = 1;
        public const int Param = 2;
        public const int Option = 3;
        public const int Property = 4;
        public const int Data = 5;

        public int MsgType;
        public string MsgPath;
        public string MsgValue;
        public int MsgIndex;
        public bool OK;
    }

    public class TMsgService
    {
        public static char StartChar = (char)2;
        public static char EndChar = (char)3;
        private StringBuilder sb = null;

        private string msg;

        private TStringList SL = new TStringList();

        private TStringList SLRequests = new TStringList();
        private TStringList SLParams = new TStringList();
        private TStringList SLOptions = new TStringList();
        private TStringList SLProperties = new TStringList();
        private TStringList SLData = new TStringList();

        public static string TokenRequest = "RiggVar.Request";
        public static string TokenParam = "DP.";
        public static string TokenProperty = "EP.";
        public static string TokenOption = ".CC."; //CompetitionControl
        public static string TokenFleetRace = "FR";
        public static string TokenDivision = "*";
            
        public TMsgService()
        {
        }

        public virtual void ReceiveMsg(string s)
        {
            char ch;
            for (int i = 0; i < s.Length; i++)
            {
                ch = s[i];
                if (ch == StartChar)
                {
                    sb = new StringBuilder();
                }
                else if (ch == EndChar)
                {
                    msg = sb.ToString();
                    this.TokenizeLines(msg);
                }
                else
                {
                    sb.Append(ch);
                }
            }
        }

        public virtual void TokenizeLines(string s)
        {
            SL.Text = s;
            DispatchLines();
        }

        public virtual void DispatchLines()
        {
            foreach (string s in SL)
            {
                if (IsCommentLine(s))
                {
                    continue;
                }
                else if (IsRequestLine(s))
                {
                    SLRequests.Add(s);
                }
                else if (IsParamLine(s))
                {
                    SLParams.Add(s);
                }
                else if (IsOptionLine(s))
                {
                    SLOptions.Add(s);
                }
                else if (IsPropertyLine(s))
                {
                    SLProperties.Add(s);
                }
                else if (IsDataLine(s))
                {
                    SLData.Add(s);
                }
            }
        }

        public virtual bool IsCommentLine(string s)
        {
            if ((s.Equals(string.Empty)) 
                || s.StartsWith("//")
                || s.StartsWith("#"))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsRequestLine(string s)
        {
            if (s.StartsWith(TMsgService.TokenRequest)
                || s.StartsWith("RiggVar.Request."))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsParamLine(string s)
        {
            if (s.StartsWith(TMsgService.TokenParam)
                || (s.IndexOf(".StartlistCount") > -1)
                || (s.IndexOf(".ITCount") > -1)
                || (s.IndexOf(".RaceCount") > -1))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsOptionLine(string s)
        {
            if (s.StartsWith(TMsgService.TokenOption)
                || (s.IndexOf(".CC.") > 0))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsPropertyLine(string s)
        {
            if (s.StartsWith(TMsgService.TokenProperty))
            {
                return true;
            }

            return false;
        }

        public virtual bool IsDataLine(string s)
        {            
            return s.IndexOf("=") > 0;        
        }


        public int GetParamLines(TStrings SL)
        {
            SL.Assign(SLParams);
            return SLParams.Count;
        }
        public int GetOptionLines(TStrings SL)
        {
            SL.Assign(SLOptions);
            return SLOptions.Count;
        }
        public int GetRequestLines(TStrings SL)
        {
            SL.Assign(SLRequests);
            return SLRequests.Count;
        }
        public int GetDataLines(TStrings SL)
        {
            SL.Assign(SLData);
            return SLData.Count;
        }


        public virtual void TokenizeMsg()
        {
        }
        public virtual void TokenizePath()
        {
        }
        public virtual void DispatchProt()
        {
        }
        public virtual string GetPath()
        {
            return null;
        }
        public virtual string GetValue()
        {
            return null;
        }
        public virtual bool IsValid()
        {
            return false;
        }
        public bool HasError()
        {
            return !IsValid();
        }
        public virtual string GetError()
        {
            return "";
        }
        public virtual string GetReport()
        {
            return "";
        }
    }
}
