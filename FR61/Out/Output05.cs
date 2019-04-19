using System;

namespace RiggVar.FR
{
    /// <summary>
    /// ColMenu Reports (CSV, HTM, XML)
    /// </summary>
    public class TOutput05 : TOutput00
    {
        private TRaceRemoteObject qe;
        private TEventRemoteObject fe;
        private TStammdatenRemoteObject se;

        public TOutput05() : base()
        {
            qe = new TRaceRemoteObject(BO.BOParams.ITCount);
            fe = new TEventRemoteObject(BO.BOParams.RaceCount);
            se = new TStammdatenRemoteObject();        
        }

        public void Welcome()
        {
            SL.Add("Welcome, FR Server VS");
            SL.Add("ServerTime: " + DateTime.Now.ToUniversalTime());
            SL.Add("EventName: " + BO.EventProps.EventName);
            SL.Add("RaceCount: " + BO.BOParams.RaceCount.ToString());
            SL.Add("ITCount: " + BO.BOParams.ITCount.ToString());
            SL.Add("StartlistCount: " + BO.BOParams.StartlistCount.ToString());
        }

        public void Athletes()
        {
            TStammdatenRowCollection cl = BO.StammdatenNode.Collection;
            se.GetHeader(SL, OutputType, "Athletes", XMLSection);
            for (int i = 0; i < cl.Count; i++)
            {
                TStammdatenRowCollectionItem cr = cl[i];
                se.Assign(cr);
                switch (OutputType)
                {
                    case TOutputType.otCSV:
                        SL.Add(se.GetCSV());
                        break;
                    case TOutputType.otHTM:
                        SL.Add(se.GetHTM());
                        break;
                    case TOutputType.otXML:
                        SL.Add(se.GetXML("A"));
                        break;
                }
            }
            se.GetFooter(SL, OutputType, "Athletes", XMLSection);
        }

        public void Backup(BackupFormat bf)
        {
            switch (OutputType)
            {
                case TOutputType.otCSV:
                    break;
                case TOutputType.otHTM:
                    SL.Add("<table border=\"1\" cellspacing=\"0\" cellpadding=\"1\">");
                    SL.Add("<caption>Backup</caption>");
                    SL.Add("<tr><th>Messagelist</th></tr>");
                    SL.Add("<tr><td>");
                    SL.Add("<pre>");
                    break;
                case TOutputType.otXML:
                    if (XMLSection)
                    {
                        SL.Add("<Backup>");
                    }
                    break;
            }

            switch (bf)
            {
                case BackupFormat.Default: BO.BackupToSL(SL); break;
                case BackupFormat.Compact: BO.BackupToSL(SL, true); break;
                case BackupFormat.MsgList: BO.BackupToSL(SL, false); break;
                default: BO.BackupToSL(SL, true); break;
            }

            switch (OutputType)
            {
                case TOutputType.otCSV:
                    break;
                case TOutputType.otHTM:
                    SL.Add("</pre>");
                    SL.Add("</td></tr></table>");
                    break;
                case TOutputType.otXML:
                    int j = 0;
                    for (int i = SL.Count-1; i >= 0; i--)
                    {
                        string s = SL[i];
                        if (s == string.Empty)
                        {
                            SL.Delete(i);
                            continue;
                        }
                        if (s[0] != '<')
                        {
                            if ((s == string.Empty) || (s[0] == '#') || (Utils.Copy(s, 1, 2) == "//"))
                            {
                                SL.Delete(i);
                                continue;
                            }
                            else
                            {
                                string sName = SL.Names(i).Trim();
                                string sValue = SL.ValueFromIndex(i).Trim();
                                j++;
                                s = "<B I=\"" + j.ToString() + "\" N=\"" + sName + "\" V=\"" + sValue + "\" />";
                                s = s.Replace(BO.cTokenA + "." + BO.cTokenB + ".", "");
                                SL[i] = s;
                            }
                        }
                    }
                    if (XMLSection)
                    {
                        SL.Add("</Backup>");
                    }

                    break;
            }
        }

        public void RaceResult(TRaceNode qn)
        {
            TRaceRowCollection cl = qn.Collection;
            qe.RunID = "";
            qe.GetHeader(SL, OutputType, "Race", XMLSection);
            for (int i = 0; i < cl.Count; i++)
            {
                TRaceRowCollectionItem cr = cl[i];
                qe.Assign(cr);
                switch (OutputType)
                {
                    case TOutputType.otCSV:
                        SL.Add(qe.GetCSV());
                        break;
                    case TOutputType.otHTM:
                        SL.Add(qe.GetHTM());
                        break;
                    case TOutputType.otXML:
                        SL.Add(qe.GetXML("Q"));
                        break;
                }
            }
            se.GetFooter(SL, OutputType, "Race", XMLSection);
        }

        public void EventResult(TEventNode fn, bool IncludeHeader, bool IncludeFooter)
        {
            TEventRowCollection cl = fn.Collection;
            if (IncludeHeader)
            {
                fe.GetHeader(SL, OutputType, "Event", XMLSection);
            }

            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                fe.Assign(cr);
                switch (OutputType)
                {
                    case TOutputType.otCSV:
                        SL.Add(fe.GetCSV());
                        break;
                    case TOutputType.otHTM:
                        SL.Add(fe.GetHTM());
                        break;
                    case TOutputType.otXML:
                        SL.Add(fe.GetXML("F"));
                        break;
                }
            }
            if (IncludeFooter)
            {
                fe.GetFooter(SL, OutputType, "Event", XMLSection);
            }
        }

        public void Properties()
        {
            TBOParams p = BO.BOParams;
            TEventProps e = BO.EventProps;
            SL.Clear();
            WantPageHeader = false;
            SL.Add("RaceCount=" + p.RaceCount.ToString());
            SL.Add("ITCount=" + p.ITCount.ToString());
            SL.Add("StartlistCount=" + p.StartlistCount.ToString());
            SL.Add("DivisionName=" + p.DivisionName);
            SL.Add("EventName=" + e.EventName);
            SL.Add("EventDates=" + e.EventDates);
            SL.Add("HostClub=" + e.HostClub);
            SL.Add("PRO=" + e.PRO);
            SL.Add("JuryHead=" + e.JuryHead);
        }

    }

}
