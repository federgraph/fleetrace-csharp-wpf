using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Text.RegularExpressions;

using RiggVar.FR;

namespace FRXML
{
    public class TransformerHtml
    {
        //bool FOK;
        private string FError;
        private string upchar;
        private string downchar;
        private XElement Xml;
        private IEnumerable<XElement> PropertyRows;
        private IEnumerable<XElement> HeaderFields;
        private IEnumerable<XElement> BodyRows;
        private TStrings SL; //not owned, do not free
        private TStrings FieldList;
        private TStrings Props;
        private TStrings FLA;
        private int StartListCount;
        private int RaceCount;
        private int TargetFleetSize;
        private bool UseFleets;
        private int R1Col;
        private int EndCol;
        private int BibCol;
        private int Bib;
        private int Race;
        private readonly bool WantPreTags = false;

        public TransformerHtml()
        { 
            FieldList = new TStringList();
            Props = new TStringList();
            FLA = new TStringList
            {
                Delimiter = ';'
            };
        }

        public string EventName
        {
            get
            {
                return Props.Values("EventName");
            }
        }

        void WriteNameListHeaderLine(int StartIndex, int StopIndex)
        {
            int n;
            string fl;
            n = 0;
            FLA.Clear();
            FLA.Add("ID");
            FLA.Add("SNR");
            FLA.Add("Bib");
            for (int i = StartIndex; i < StopIndex; i++)
            {
                n++;
                FLA.Add("N" + n);
            }
            fl = FLA.DelimitedText;
            SL.Add(fl);
            FLA.Clear();
        }

        void WriteFirstLine(TStrings FL,
            int StartIndex,
            int StopIndex,
            int RaceIndex,
            int EndIndex)
        {
            string fl;
            string s;
            if (FL.Count == 0)
            {
                FError = "FieldList.Count == 0";
                return;
            }

            FLA.Clear();
            for (int i = 0; i < EndIndex; i++)
            {
                if ( (i >= StartIndex && i <= StopIndex) || (i >= RaceIndex))
                {
                    if (i > FL.Count - 1)
                    {
                        FError = "WriteFirstLine: FieldList.Count < Index";
                        return;
                    }
                    s = FL[i];
                    if (s != "")
                    {
                        s = s.Replace(upchar, string.Empty);
                        s = s.Replace(downchar, string.Empty);
                        FLA.Add(s);
                    }
                }
            }
            fl = FLA.DelimitedText;
            SL.Add(fl);
        }

        string GenQU(string value)
        {
            return "FR.*.W" + Race + ".Bib" + Bib + ".QU=" + value;
        }

        int CalcFleetSize(int rCol, string gClass)
        {
            IEnumerable<XElement> tds;
            int i;
            int c;

            c = 0;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                i = 0;
                foreach (XElement td in tds)
                {
                    if (i == rCol)
                    {
                        if (td.Attribute(gClass) != null)
                        {
                            c++;
                        }
                    }
                    i++;
                }
            }
            return c;
        }

        int GetRaceIndex(int r)
        {
            return r + R1Col - 1;
        }

        int CountGroupElements(int Group)
        {
            IEnumerable<XElement> tds;
            string cls;
            int c;
            string gn;

            c = 0;
            gn = "g" + Group;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                foreach (XElement td in tds)
                {
                    if (td.Attribute("class") != null)
                    {
                        cls = td.Attribute("class").Value;
                        if (cls.Contains(gn))
                        {
                            c++;
                        }
                    }
                }
            }
            return c;
        }

        string RetrieveGroupMatch(MatchCollection mc)
        {
            Match m;
            if (mc.Count > 0)
            {
                m = mc[0];
                if (m.Groups.Count > 1)
                {
                    return m.Groups[1].Value;
                }
            }
            return "";
        }

        bool IsRaceCol(string th)
        {
            Regex regex;
            MatchCollection mc;
            string t;

            //var pattern = /R(\d+)/;
            //return pattern.test($(th).text());

            regex = new Regex(@"R(\d+)");
            mc = regex.Matches(th);
            t = RetrieveGroupMatch(mc);
            if (t != "")
            {
                return true;
            }

            return false;
        }

        void ExtractParams()
        {
            string s;
            int i;
            int g0, g1;

            if (HeaderFields == null)
            {
                FError = "HeaderFields = nil in ExtractParams";
                //FOK = false;
                return;
            }

            i = 0;
            foreach (XElement th in HeaderFields)
            {
                s = th.Value;
                if ((R1Col > 0 && EndCol == 0) && !IsRaceCol(s))
                {
                    RaceCount = i - R1Col;
                    EndCol = i;
                }
                else if ((i > 0 && R1Col == 0) && IsRaceCol(s))
                {
                    R1Col = i;
                }
                if (EndCol == 0)
                {
                    FieldList.Add(s);
                }
                i++;
            }

            StartListCount = BodyRows.Count();

            UseFleets = CountGroupElements(0) > 0;
            UseFleets = UseFleets || (CountGroupElements(1) > 0);

            g0 = CalcFleetSize(GetRaceIndex(1), "g0");
            g1 = CalcFleetSize(GetRaceIndex(1), "g1");
            TargetFleetSize = Math.Max(g0, g1);
        }

        int ExtractGroup(XElement td)
        {
            string cl = td.Attribute("class").Value;
            if (cl == "g0")
            {
                return 0;
            }
            else if (cl == "g1")
            {
                return 1;
            }
            else if (cl == "g2")
            {
                return 2;
            }
            else if (cl == "g3")
            {
                return 3;
            }
            else if (cl == "g4")
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        string ExtractFinishPosition(string s)
        {
            Regex regex;
            MatchCollection mc;
            string t;

            // '12' --> 12
            regex = new Regex(@"^(\d+)$");
            if (regex.IsMatch(s))
            {
                return s;
            }

            // '[35]' --> 35
            regex = new Regex(@"^\[(\d+)\]$");
            mc = regex.Matches(s);
            t = RetrieveGroupMatch(mc);
            if (t != "")
            {
                return t;
            }

            // '[0/DNF]' --> 0
            regex = new Regex(@"^\[(\d+)\/\S+\]");
            mc = regex.Matches(s);
            t = RetrieveGroupMatch(mc);
            if (t != "")
            {
                return t;
            }

            // '5/DSQ' --> 5
            regex = new Regex(@"^(\d+)\/\S+");
            mc = regex.Matches(s);
            t = RetrieveGroupMatch(mc);
            if (t != "")
            {
                return t;
            }

            return "0";
        }

        string ExtractQU(string s)
        {
            Regex regex;
            MatchCollection mc;
            int c;
            string t;

            // '12' --> 12
            regex = new Regex(@"^(\d+)$");
            if (regex.IsMatch(s))
            {
                return "";
            }

            // '[35]' --> 35
            regex = new Regex(@"^\[(\d+)\]$");
            if (regex.IsMatch(s))
            {
                return "";
            }

            // '[0/DNF]' --> 0
            regex = new Regex(@"^\[\d+\/(\S+)\]");
            mc = regex.Matches(s);
            t = RetrieveGroupMatch(mc);
            if (t != "")
            {
                return GenQU(t);
            }

            // '5/DSQ' --> 5
            regex = new Regex(@"^\d+\/(\S+)");
            mc = regex.Matches(s);
            t = RetrieveGroupMatch(mc);
            if (t != "")
            {
                return GenQU(t);
            }

            // 'dnf' --> dnf
            regex = new Regex(@"dns|dnf|dsq", RegexOptions.IgnoreCase);
            mc = regex.Matches(s);
            c = mc.Count;
            if (c >= 1)
            {
                t = mc[0].Value;
                return GenQU(t);
            }

            return "";
        }

        string ExtractNoRace(string s)
        {
            // '(0)'
            Regex regex = new Regex(@"^\(0\)$");
            if (regex.IsMatch(s))
            {
                return "FR.*.W" + Race + ".Bib" + Bib + ".RV=x";
            }

            return "";
        }

        void MakeNameList()
        {
            IEnumerable<XElement> tds;
            string r, s;
            int i;

            WriteNameListHeaderLine(3, R1Col);
            s = "";
            i = 0;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                foreach (XElement td in tds)
                {
                    r = td.Value;
                    if (r == "&nbsp;")
                    {
                        r = "";
                    }

                    if (i >= 0 && i < R1Col)
                    {
                        if (s != "")
                        {
                            s = s + ';';
                        }

                        s = s + r;
                    }
                    i++;
                }
                SL.Add(s);
                s = "";
                i = 0;
            }
        }

        void MakeStartList()
        {
            IEnumerable<XElement> tds;
            string r, s;
            int i;
            int e;

            e = 3;
            WriteFirstLine(FieldList, 0, e, e, e);
            s = "";
            i = 0;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                foreach (XElement td in tds)
                {
                    r = td.Value;
                    if (i < e)
                    {
                        if (s != "")
                        {
                            s = s + ';';
                        }

                        s = s + r;
                    }
                    i++;
                }
                SL.Add(s);
                s = "";
                i = 0;
            }
        }

        void MakeFleetList()
        {
            IEnumerable<XElement> tds;
            string r, s;
            int i;

            WriteFirstLine(FieldList, 1, 2, R1Col, EndCol);
            s = "";
            i = 0;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                foreach (XElement td in tds)
                {
                    r = td.Value;
                    if (i > 0 && i < 3)
                    {
                        if (s != "")
                        {
                            s = s + ';';
                        }

                        s = s + r;
                    }
                    else if (i >= R1Col && i < EndCol)
                    {
                        if (s != "")
                        {
                            s = s + ';';
                        }

                        s = s + ExtractGroup(td);
                    }
                    i++;
                }
                SL.Add(s);
                s = "";
                i = 0;
            }
        }

        void MakeFinishList()
        {
            IEnumerable<XElement> tds;
            string r, s;
            int i;

            WriteFirstLine(FieldList, 0, 2, R1Col, EndCol);
            s = "";
            i = 0;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                foreach (XElement td in tds)
                {
                    r = td.Value;
                    if ((i >= 0 && i <= 2) || (i >= R1Col && i < EndCol))
                    {
                        if (s != "")
                        {
                            s = s + ';';
                        }

                        s = s + ExtractFinishPosition(r);
                    }
                    i++;
                }
                SL.Add(s);
                s = "";
                i = 0;
            }
        }

        void MakeMsgList()
        {
            IEnumerable<XElement> tds;
            string r, s;
            int i;

            s = "";
            i = 0;
            foreach (XElement tr in BodyRows)
            {
                tds = tr.Elements("td");
                foreach (XElement td in tds)
                {
                    r = td.Value;
                    if (i < EndCol)
                    {
                        if (i == BibCol)
                        {
                            Bib = Utils.StrToIntDef(r, 0);
                        }

                        if (i >= R1Col)
                        {
                            Race = i - R1Col + 1;
                            s = ExtractQU(r);
                            if (s != "")
                            {
                                SL.Add(s);
                                s = "";
                            }
                            s = ExtractNoRace(r);
                            if (s != "")
                            {
                                SL.Add(s);
                                s = "";
                            }
                        }
                    }
                    i++;
                }
                i = 0;
            }
        }

        void ReadTables()
        {
            string s;
            XElement body = Xml.Element("body");
            if (body != null)
            {
                // find the tables as direct descendants of body tag
                FindTables(body);

                if (BodyRows == null)
                {
                    //find tables inside the results div
                    IEnumerable<XElement> divs = body.Elements("div");
                    foreach (XElement dive in divs)
                    {
                        s = dive.Attribute("id").Value;
                        if (s == "results")
                        {
                            FindTables(dive);
                        }
                    }
                }
            }
        }

        void ReadProperties(XElement table)
        {
            IEnumerable<XElement> trs, tds;
            string s;
            int i;
            string k, v;

            trs = table.Elements("tr");
            foreach (XElement tr in trs)
            {
                tds = tr.Elements("td");
                i = 1;
                k = "";
                v = "";
                foreach (XElement td in tds)
                {
                    s = td.Value;
                    if (i == 1)
                    {
                        k = s;
                    }
                    else if (i == 2)
                    {
                        v = s;
                    }

                    i++;
                    if (i == 3)
                    {
                        Props.Values(k, v);
                        break;
                    }
                }
            }
            PropertyRows = trs;
            //remember for now and free later in Reset method
        }

        void ReadResults(XElement table)
        {
            if (HeaderFields != null)
            {
                throw new InvalidOperationException("HeaderFields must not be assigned when calling ReadResults(table)");
            }

            XElement nl = table.Element("thead");
            if (nl != null)
            {
                IEnumerable<XElement> trs = nl.Elements("tr");
                if (trs.Count() == 1)
                {
                    HeaderFields = trs.Elements("th");
                }
            }
            BodyRows = table.Elements("tr");
        }

        void Reset()
        {
            FError = "";
            //FOK = false;

            HeaderFields = null;
            BodyRows = null;
            PropertyRows = null;

            upchar = ((char)9650).ToString();
            downchar = ((char)9660).ToString();

            StartListCount = 0;
            R1Col = 0;
            RaceCount = 0;
            EndCol = 0;
            BibCol = 2;
            Bib = 0;

            FieldList.Clear();
            Props.Clear();
            FLA.Clear();

            Props.Values("EP.UseFleets", "false");
            Props.Values("EP.TargetFleetSize", "20");
            Props.Values("EP.FirstFinalRace", "0");
            Props.Values("EP.NameFieldCount", "0");
            Props.Values("EP.NameFieldOrder", "");
            Props.Values("EP.FieldCaptions", "");
            Props.Values("EP.FieldCount", "0");
            Props.Values("EP.ScoringSystem", "Low Point System");
            Props.Values("EP.Throwouts", "1");
            Props.Values("EP.ReorderRAF", "false");
            Props.Values("EP.ColorMode", "N");
        }

        void Transform()
        {
            Reset();

            ReadTables();

            //HeaderFields = $('table.results thead tr th');
            if (HeaderFields == null)
            {
                FError = "HeaderFields = null";
                //FOK = false;
                return;
            }

            //BodyRows = $('table.results tbody tr');
            if (BodyRows == null)
            {
                FError = "BodyRows = null";
                //FOK = false;
                return;
            }

            //PropertyRows = $('table.properties tr');
            if (PropertyRows == null)
            {
                FError = "PropertyRows = null";
                //FOK = false;
                return;
            }

            ExtractParams();

            if (WantPreTags)
            {
                SL.Add("<pre>");
            }

            SL.Add("#Params");

            SL.Add("");

            SL.Add("DP.StartlistCount =" + StartListCount);
            SL.Add("DP.ITCount = 0");
            SL.Add("DP.RaceCount = " + RaceCount);

            SL.Add("");

            SL.Add("#Event Properties");

            SL.Add("");

            SL.Add("EP.Name = " + Props.Values("EP.Name"));
            SL.Add("EP.ScoringSystem = " + Props.Values("EP.ScoringSystem"));
            SL.Add("EP.Throwouts = " + Props.Values("EP.Throwouts"));
            SL.Add("EP.ReorderRAF = " + Props.Values("EP.ReorderRAF"));
            SL.Add("EP.DivisionName = *");
            SL.Add("EP.InputMode = Strict");
            SL.Add("EP.RaceLayout = Finish");
            SL.Add("EP.NameSchema = NX");
            //t.Add("EP.FieldMap = " + Props.Values["EP.FieldMap"]);
            SL.Add("EP.FieldCaptions = " + Props.Values("EP.FieldCaptions"));
            SL.Add("EP.FieldCount = " + Props.Values("EP.FieldCount"));
            SL.Add("EP.NameFieldCount = " + Props.Values("EP.NameFieldCount"));
            SL.Add("EP.NameFieldOrder = " + Props.Values("EP.NameFieldOrder"));

            SL.Add("EP.ColorMode = " + Props.Values("EP.ColorMode"));
            SL.Add("EP.UseFleets = " + Props.Values("EP.UseFleets"));
            SL.Add("EP.TargetFleetSize = " + Props.Values("EP.TargetFleetSize"));
            SL.Add("EP.FirstFinalRace = " + Props.Values("EP.FirstFinalRace"));
            SL.Add("EP.IsTimed = false");
            SL.Add("EP.UseCompactFormat = true");

            SL.Add("");

            SL.Add("NameList.Begin");
            MakeNameList();
            SL.Add("NameList.End");

            SL.Add("");

            SL.Add("StartList.Begin");
            MakeStartList();
            SL.Add("StartList.End");

            if (UseFleets)
            {
                SL.Add("");

                SL.Add("FleetList.Begin");
                MakeFleetList();
                SL.Add("FleetList.End");
            }

            SL.Add("");

            SL.Add("FinishList.Begin");
            MakeFinishList();
            SL.Add("FinishList.End");

            SL.Add("");

            //t.Add("MsgList.Begin");
            MakeMsgList();
            //t.Add("MsgList.End");

            if (WantPreTags)
            {
                SL.Add("</pre>");
            }
        }

        void ReadTable(XElement table)
        {
            string s = table.Attribute("class").Value;
            if (s == "fr properties")
            {
                ReadProperties(table);
            }
            else if (s == "sortable fr results")
            {
                ReadResults(table);
            }
        }

        void FindTables(XElement node)
        {
            IEnumerable<XElement> tables = node.Elements("table");
            if (tables != null)
            {
                foreach (XElement table in tables)
                {
                    ReadTable(table);
                }
            }
        }


        public void TransformEventData(XElement xe, TStrings ASL)
        {
            try
            {
                SL = ASL;
                Xml = xe;

                Transform();

                Xml = null;
                SL = null;

                //FOK = true;
            }
            catch (Exception e)
            {
                //FOK = false;
                FError = e.Message;
            }
        }

    }
}
