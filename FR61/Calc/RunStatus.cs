using System.Windows.Media;

namespace RiggVar.FR
{
    public enum TRunStatusEnum 
    {
        rsNone,
        rsStarted,
        rsResults
    }

    public class TRunStatus
    {
        public void Assign(TRunStatus source)
        {
        }

        public static bool CheckInput(string Value)
        {
            string s = Value.ToLower();
            if ((s == "none") || (s == "n"))
            {
                return true;
            }
            else if ((s == "list") || (s == "l"))
            {
                return true;
            }
            else if ((s == "started") || (s == "s"))
            {
                return true;
            }
            else if ((s == "results") || (s == "r"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static TRunStatusEnum StrToRunStatusEnum(string Value)
        {
            TRunStatusEnum result = TRunStatusEnum.rsNone;
            string s = Value.ToLower();
            if ((s == "none") || (s == "n"))
            {
                result = TRunStatusEnum.rsNone;
            }
            else if ((s == "started") || (s == "s"))
            {
                result = TRunStatusEnum.rsStarted;
            }
            else if ((s == "results") || (s == "r"))
            {
                result = TRunStatusEnum.rsResults;
            }

            return result;
        }
        public bool Parse(string Value)
        {
            string s = Value.ToLower();
            if ((s == "none") || (s == "n"))
            {
                Status = TRunStatusEnum.rsNone;
                return true;
            }
            else if ((s == "started") || (s == "s"))
            {
                Status = TRunStatusEnum.rsStarted;
                return true;
            }
            else if ((s == "results") || (s == "r"))
            {
                Status = TRunStatusEnum.rsResults;
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            switch (Status)
            {
                case TRunStatusEnum.rsNone: return string.Empty; 
                case TRunStatusEnum.rsStarted: return "S"; 
                case TRunStatusEnum.rsResults: return "R"; 
                default: return string.Empty;
            }
        }
        public Color ToColor(Color aDefault)
        {
            switch (Status)
            {
                case TRunStatusEnum.rsNone: return aDefault; 
                case TRunStatusEnum.rsStarted: return Colors.Lime; 
                case TRunStatusEnum.rsResults: return Colors.Cyan; 
                default: return Colors.White;
            }
        }
        public TRunStatusEnum Status { get; set; }
        public int AsInteger
        {
            get 
            {
                switch (Status)
                {
                    case TRunStatusEnum.rsNone: return 0; 
                    case TRunStatusEnum.rsStarted: return 1; 
                    case TRunStatusEnum.rsResults: return 2; 
                    default: return 0;
                }
            }
            set 
            {
                switch (value)
                {
                    case 0: Status = TRunStatusEnum.rsNone; 
                        break;
                    case 1: Status = TRunStatusEnum.rsStarted; 
                        break;
                    case 2: Status = TRunStatusEnum.rsResults; 
                        break;
                    default: 
                        break;
                }
            }
        }
    }
}
