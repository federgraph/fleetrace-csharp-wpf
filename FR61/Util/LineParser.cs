
namespace RiggVar.FR
{

    public class TTokenParser
    {
        public string sToken;
        public string sRest;
        public void NextToken()
        {
            sRest = Utils.Cut(".", sRest, ref sToken);
        }
        public int NextTokenX(string TokenName)
        {
            NextToken();
            int result = -1;
            int l = TokenName.Length;
            if (Utils.Copy(sToken, 1, l) == TokenName)
            {
                sToken = Utils.Copy(sToken, l + 1, sToken.Length - l);
                result = Utils.StrToIntDef(sToken, -1);
            }
            return result;
        }
    }

    public class TLineParser
    {
        private TStringList SL = new TStringList();

        protected virtual bool ParseKeyValue(string Key, string Value)
        {
            //virtual, this version is only used in unit tests
            return Key == "Key" && Value == "Value";
        }

        public bool ParseLine(string s)
        {
            string temp;

            SL.Clear();
            int i = Utils.Pos("=", s);
            if (i > 0)
            {
                temp = Utils.Copy(s, 1, i - 1).Trim();
                temp += "=";
                temp += Utils.Copy(s, i + 1, s.Length).Trim();
            }
            else
            {
                temp = s.Trim();
                temp = temp.Replace(' ', '_');
            }

            if (Utils.Pos("=", temp) == 0)
            {
                temp = temp + "=";
            }

            SL.Add(temp);
            string sK = SL.Names(0);
            string sV = SL.Values(sK);
            return ParseKeyValue(sK, sV);
        }

    }

}
