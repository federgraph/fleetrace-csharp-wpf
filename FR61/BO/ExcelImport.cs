namespace RiggVar.FR
{
    public class TExcelImporter
    {
        public const int TableID_NameList = 1;
        public const int TableID_FinishList = 2;
        public const int TableID_StartList = 3;
        public const int TableID_ResultList = 4;
        public const int TableID_TimeList = 5;
        public const int TableID_FleetList = 6;
        public const int TableID_CaptionList = 7;

        public const string NameListStartToken = "NameList.Begin";
        public const string NameListEndToken = "NameList.End";

        public const string StartListStartToken = "StartList.Begin";
        public const string StartListEndToken = "StartList.End";

        public const string FinishListStartToken = "FinishList.Begin";
        public const string FinishListEndToken = "FinishList.End";

        public const string TimeListStartToken = "TimeList.Begin"; //with Param: TimeList.Begin.R1
        public const string TimeListEndToken = "TimeList.End";

        public const string ResultListStartToken = "ResultList.Begin";
        public const string ResultListEndToken = "ResultList.End";

        public const string FleetListStartToken = "FleetList.Begin";
        public const string FleetListEndToken = "FleetList.End";

        public const string CaptionListStartToken = "CaptionList.Begin";
        public const string CaptionListEndToken = "CaptionList.End";

        private TStringList SL;
        private TStringList SLToken;
        private TStringList SLField;
        private TStringList SLFilter;

        private int SNR;
        private int Bib;
        private int PosID;
        private int N;
        private int W;
        private int IT;
        private char FDelimiter;
        private int TableID;
        private string sToken;
        private string sRest;

        public TExcelImporter()
        {
            FDelimiter = ';';
            SL = new TStringList();
            SLToken = new TStringList();
            SLToken.Delimiter = FDelimiter;
            //SLToken.StrictDelimiter = true;
            SLField = new TStringList();
            SLFilter = new TStringList();
        }

        public char Delimiter
        {
            get => FDelimiter;
            set => FDelimiter = value;
        }

        public void SetTableID(string Value)
        {
            if (Value.IndexOf("StartList") > -1)
            {
                TableID = TableID_StartList;
            }
            else if (Value.IndexOf("FleetList") > -1)
            {
                TableID = TableID_FleetList;
            }
            else if (Value.IndexOf("FinishList") > -1)
            {
                TableID = TableID_FinishList;
            }
            else if (Value.IndexOf("NameList") > -1)
            {
                TableID = TableID_NameList;
            }
            else if (Value.IndexOf("ResultList") > -1)
            {
                TableID = TableID_ResultList;
            }
            else if (Value.IndexOf("TimeList") > -1)
            {
                TableID = TableID_TimeList;
                W = ExtractRaceParam(Value);
            }
            else if (Value.IndexOf("CaptionList") > -1)
            {
                TableID = TableID_CaptionList;
            }
            else
            {
                TableID = 0;
            }
        }

        private int ExtractRaceParam(string Value)
        {
            sRest = Value;
            NextToken(); //TimeList
            NextToken(); //Begin
            return NextTokenX("R"); //RX
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
                sToken = Utils.Copy(sToken, l + 1, sToken.Length - l);
                return Utils.StrToIntDef(sToken, -1);
            }
            return -1;
        }

        protected string TrimAroundEqual(string s)
        {
            string result = s;
            int i = s.IndexOf('=');
            if (i > 0)
            {
                result = s.Substring(0, i).Trim() + '=' + s.Substring(i + 1).Trim();
            }

            return result;
        }

        public void GetTestData(TStrings Memo)
        {
            Memo.Clear();
            Memo.Add("Bib;SNR;FN;LN;NC;R1;R2");
            Memo.Add("1;1001;A;a;FRA;1;2");
            Memo.Add("2;1002;B;b;GER;2;3");
            Memo.Add("3;1003;C;b;ITA;3;1");
        }

        public void ShowTabs(TStrings Memo)
        {
            SL.Text = Memo.Text;
            Memo.Clear();
            char delim = '\t';
            string s;
            for (int i = 0; i < SL.Count; i++)
            {
                s = SL[i];
                s = s.Replace(delim, ';');
                Memo.Add(s);
            }
        }

        public string Go(string command, string data)
        {
            TStringList Memo = new TStringList();
            Memo.Text = data;
            if (command == "Convert")
            {
                Convert(Memo, TableID_ResultList);
            }

            if (command == "ShowTabs")
            {
                ShowTabs(Memo);
            }

            if (command == "GetTestData")
            {
                GetTestData(Memo);
            }

            return Memo.Text;
        }

        public string Convert(string MemoText, int tableID)
        {
            TStringList Memo = new TStringList();
            Memo.Text = MemoText;
            Convert(Memo, tableID);
            return Memo.Text;
        }

        public void Convert(TStrings Memo, int tableID)
        {
            SL.Clear();
            TableID = tableID;
            Transpose(Memo);
            Memo.Text = SL.Text;
            TableID = 0;
            SL.Clear();
        }

        private void SetValue_StartList(string f, string v)
        {
            string s;

            if (f == "SNR")
            {
                SNR = Utils.StrToIntDef(v, 0);
                if (SNR != 0)
                {
                    s = string.Format("FR.*.W1.STL.Pos{0}.SNR={1}", PosID, SNR);
                    SL.Add(s);
                }
            }
            else if (f == "Bib")
            {
                Bib = Utils.StrToIntDef(v, 0);
                if (SNR != 0 && Bib != 0)
                {
                    s = string.Format("FR.*.W1.STL.Pos{0}.Bib={1}", PosID, Bib);
                    SL.Add(s);
                }
            }
        }

        private void SetValue_NameList(string f, string v)
        {

            string s;
            if (f == "FN" || f == "LN" || f == "SN" || f == "NC" || f == "GR" || f == "PB")
            {
                string v1 = v.Trim();
                s = string.Format("FR.*.SNR{0}.{1}={2}", SNR, f, v1);
                SL.Add(s);
            }

            else if (f.Length > 1 && f[0] == 'N')
            {
                N = Utils.StrToIntDef(f.Substring(1), 0);
                if (N > 0 && SNR > 0)
                {
                    s = string.Format("FR.*.SNR{0}.N{1}={2}", SNR, N, v);
                    SL.Add(s);
                }
            }
        }

        private void SetValue_FinishList(string f, string v)
        {
            string s;
            if (f.Length > 1 && f[0] == 'R')
            {
                W = Utils.StrToIntDef(f.Substring(1), 0);
                if (W > 0 && Bib > 0)
                {
                    if (Utils.StrToIntDef(v, 0) > 0)
                    {
                        s = string.Format("FR.*.W{0}.Bib{1}.Rank={2}", W, Bib, v);
                        SL.Add(s);
                    }
                }
            }
        }

        private void SetValue_FleetList(string f, string v)
        {
            string s;
            if (f.Length > 1 && f[0] == 'R')
            {
                W = Utils.StrToIntDef(f.Substring(1), 0);
                if (W > 0 && Bib > 0)
                {
                    if (Utils.StrToIntDef(v, -1) > -1)
                    {
                        //s = string.Format("FR.*.W{0}.Bib{1}.RV=F{2}", W, Bib, v); //alternative
                        s = string.Format("FR.*.W{0}.Bib{1}.FM={2}", W, Bib, v);
                        SL.Add(s);
                    }
                }
            }
        }

        private void SetValue_TimeList(string f, string v)
        {
            string s;
            if (f.Length > 2 && f[0] == 'I')
            {
                IT = Utils.StrToIntDef(f.Substring(2), -1);
                if (IT > 0 && Bib > 0)
                {
                    s = string.Format("FR.*.W{0}.Bib{1}.IT{2}={3}", W, Bib, IT, v);
                    SL.Add(s);
                }
                if (IT == 0 && Bib > 0)
                {
                    s = string.Format("FR.*.W{0}.Bib{1}.FT={2}", W, Bib, v);
                    SL.Add(s);
                }
            }
            else if (f == "FT")
            {
                s = string.Format("FR.*.W{0}.Bib{1}.FT={2}", W, Bib, v);
                SL.Add(s);
            }

        }

        public void SetValue_ResultList(string f, string v)
        {
            string s;

            if (f == "SNR")
            {
                SNR = Utils.StrToIntDef(v, 0);
                if (SNR != 0)
                {
                    s = string.Format("FR.*.W1.STL.Pos{0}.SNR={1}", PosID, SNR);
                    SL.Add(s);
                }
            }
            else if (f == "Bib")
            {
                Bib = Utils.StrToIntDef(v, 0);
                if (SNR != 0 && Bib != 0)
                {
                    s = string.Format("FR.*.W1.STL.Pos{0}.Bib={1}", PosID, Bib);
                    SL.Add(s);
                }
            }

            else if (f == "FN" || f == "LN" || f == "SN" || f == "NC" || f == "GR" || f == "PB")
            {
                SetValue_NameList(f, v);
            }

            else if (f.Length > 1 && f[0] == 'N')
            {
                SetValue_NameList(f, v);
            }

            else if (f.Length > 1 && f[0] == 'R')
            {
                W = Utils.StrToIntDef(f.Substring(1), 0);
                if (W > 0 && Bib > 0)
                {
                    if (Utils.StrToIntDef(v, -1) > -1)
                    {
                        s = string.Format("FR.*.W{0}.Bib{1}.RV={2}", W, Bib, v);
                    }
                    else
                    {
                        s = string.Format("FR.*.W{0}.Bib{1}.QU={2}", W, Bib, v);
                    }

                    SL.Add(s);
                }
            }
        }

        public void SetValue_CaptionList(string f, string v)
        {
            TColCaptions.ColCaptionBag.SetCaption(f, v);
        }

        private void SetValue(string f, string v)
        {
            if (TableID == TableID_StartList)
            {
                SetValue_StartList(f, v);
            }
            else if (TableID == TableID_NameList)
            {
                SetValue_NameList(f, v);
            }
            else if (TableID == TableID_FinishList)
            {
                SetValue_FinishList(f, v);
            }
            else if (TableID == TableID_TimeList)
            {
                SetValue_TimeList(f, v);
            }
            else if (TableID == TableID_FleetList)
            {
                SetValue_FleetList(f, v);
            }
            else if (TableID == TableID_ResultList)
            {
                SetValue_ResultList(f, v);
            }
            else if (TableID == TableID_CaptionList)
            {
                SetValue_CaptionList(f, v);
            }
        }

        private void Transpose(TStrings Memo)
        {
            string s;
            string f;
            string v;
            int snrIndex;
            string snrString;
            int bibIndex;
            string bibString;

            PosID = -1;
            SLField.Clear();
            snrIndex = -1;
            bibIndex = -1;
            for (int i = 0; i < Memo.Count; i++)
            {
                s = Memo[i];
                if (s.Trim() == "")
                {
                    continue;
                }

                PosID++;
                Bib = 0 + PosID;
                SLToken.Delimiter = Delimiter;
                SLToken.DelimitedText = s;
                SNR = 999 + PosID;

                if (i > 0)
                {
                    if (SLToken.Count != SLField.Count)
                    {
                        //        SL.Add('');
                        //        SL.Add('//line skipped - SLToken.Count <> SLField.Count');
                        //        SL.Add('');
                        continue;
                    }
                }

                //get real SNR
                if (i == 0)
                {
                    snrIndex = SLToken.IndexOf("SNR");
                }
                if (i > 0)
                {
                    if (snrIndex > -1)
                    {
                        snrString = SLToken[snrIndex].Trim();
                        SNR = Utils.StrToIntDef(snrString, SNR);
                    }
                }

                //get real Bib
                if (i == 0)
                {
                    bibIndex = SLToken.IndexOf("Bib");
                }
                if (i > 0)
                {
                    if (bibIndex > -1)
                    {
                        bibString = SLToken[bibIndex].Trim();
                        Bib = Utils.StrToIntDef(bibString, Bib);
                    }
                }

                for (int j = 0; j < SLToken.Count; j++)
                {
                    v = SLToken[j];
                    if (i == 0)
                    {
                        SLField.Add(v);
                    }
                    else
                    {
                        if (v.Trim() == "")
                        {
                            continue;
                        }

                        f = SLField[j];
                        SetValue(f, v);
                    }
                }
            }

        }

        private void TransposePropList(TStrings Memo)
        {
            string s, temp, sK, sV;

            for (int l = 0; l < Memo.Count; l ++)
            {
                s = Memo[l];
                if (s.Trim() == "")
                {
                    continue;
                }

                SLToken.Clear();
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

                SLToken.Add(temp);
                sK = SLToken.Names(0);
                sV = SLToken.Values(sK);

                SetValue(sK, sV);
            }
        }

        public void RunImportFilter(string Data, TStrings m)
        {
            SL.Clear();

            string s;
            bool FilterMode;

            TableID = 0;
            FilterMode = false;
            m.Text = Data;
            for (int i = 0; i < m.Count; i++)
            {
                s = m[i];

                //Comment Lines
                if (s == "" || s.StartsWith("//") || s.StartsWith("#"))
                {
                    continue;
                }

                //TableEndToken for key=value property list
                if (s == CaptionListEndToken)
                {
                    TransposePropList(SLFilter);
                    SLFilter.Clear();
                    FilterMode = false;
                    TableID = 0;
                }

                //TableEndToken for delimited Tables
                if (s == NameListEndToken
                || s == StartListEndToken
                || s == FinishListEndToken
                || s == TimeListEndToken
                || s == FleetListEndToken)
                {
                    Transpose(SLFilter);
                    SLFilter.Clear();
                    FilterMode = false;
                    TableID = 0;
                }

                //TableStartToken, may include Parameters
                else if (s == NameListStartToken
                || s == StartListStartToken
                || s == FinishListStartToken
                || s == FleetListStartToken
                || s.IndexOf(TimeListStartToken) > -1
                || s == CaptionListStartToken)
                {
                    SLFilter.Clear();
                    FilterMode = true;
                    SetTableID(s);
                }

                //DataLines, normal Message or delimited Line
                else
                {
                    if (FilterMode)
                    {
                        SLFilter.Add(s);
                    }
                    else
                    {
                        s = TrimAroundEqual(s);
                        SL.Add(s);
                    }
                }
            }
            if (SLFilter.Count != 0)
            {
                System.Diagnostics.Debug.WriteLine("FilterError");
            }
            m.Text = SL.Text;
        }

        public static string Expand(string EventData)
        {
            TStringList t = new TStringList();
            TExcelImporter o = new TExcelImporter();
            o.RunImportFilter(EventData, t);
            return t.Text;
        }
    }

}
