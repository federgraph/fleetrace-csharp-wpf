using System;
using System.IO;
using System.Xml;
using System.Text;
using RiggVar.Scoring;

namespace RiggVar.FR
{
    /// <summary>
    /// proxy for using internal scoring code via TFRProxy and TProxyLoader
    /// </summary>
    public class TCalcEventProxy11 : TCalcEventProxy
    {
        protected TEventNode eventNode;
        protected TEventProps EventProps; //shortcut to eventNode.BO.Props
        protected TFRProxy proxyNode;

        public TCalcEventProxy11()
        {
        }
        public override void Calc(TEventNode aqn)
        {
            eventNode = aqn;
            EventProps = TMain.BO.EventProps;

            if (eventNode.Collection.Count == 0)
            {
                return;
            }

            proxyNode = new TFRProxy();
            LoadProxy();
            ScoreRegatta(proxyNode);
            if (WithTest)
            {
                WithTest = false;            
                CheckResult(proxyNode);
            }
            UnLoadProxy();
            proxyNode = null;
        }
        protected void LoadProxy()
        {
            if (eventNode == null)
            {
                return;
            }

            EventName = TMain.BO.EventProps.EventName;

            TJSEventProps ep = proxyNode.EventProps;
            ep.ScoringSystem = Utils.EnumInt(EventProps.ScoringSystem);
            ep.ThrowoutScheme = Utils.EnumInt(EventProps.ThrowoutScheme);
            ep.Throwouts = EventProps.Throwouts;
            ep.FirstIs75 = EventProps.FirstIs75;
            ep.ReorderRAF = EventProps.ReorderRAF;
            
            ep.DivisionName = EventProps.DivisionName;

            proxyNode.RCount = eventNode.RCount; //SetLength(p.IsRacing, qn.RCount); // RCount = RaceCount+1
            for (int r = 1; r < eventNode.RCount; r++)
            {
                if (TMain.BO.RNode[r].IsRacing)
                {
                    proxyNode.IsRacing[r] = true;
                }
                else
                {
                    proxyNode.IsRacing[r] = false;
                }

                proxyNode.UseFleets = eventNode.UseFleets;
                proxyNode.TargetFleetSize = eventNode.TargetFleetSize;
                proxyNode.FirstFinalRace = eventNode.FirstFinalRace;
            }
            TEventRowCollection cl = eventNode.Collection;
            proxyNode.EntryInfoCollection.Clear();
            for (int i = 0; i < cl.Count; i++)
            {
                TEventRowCollectionItem cr = cl[i];
                TEntryInfo ei = proxyNode.EntryInfoCollection.Add();
                ei.SNR = cr.SNR;
                ei.RaceList.Clear();
                for (int r = 0; r < cr.RCount; r++)
                {
                    TRaceInfo ri = new TRaceInfo();
                    ei.RaceList.Add(ri);
                    if (r == 0)
                    {
                        continue;
                    }

                    TEventRaceEntry er = cr.Race[r];
                    ri.OTime = er.OTime;
                    ri.QU = er.QU;
                    ri.Penalty_Points = er.Penalty.Points;
                    ri.Penalty_Percent = er.Penalty.Percent;
#if Sailtime
                    ri.Penalty_Note = er.Penalty.Note;
                    ri.Penalty_TimePenalty = er.Penalty.TimePenalty;
#endif
                    ri.Fleet = er.Fleet;
                    ri.IsRacing = er.IsRacing;
                }
            }
        }
        private void ScoreRegatta(TFRProxy p)
        {
            TProxyLoader se = new TProxyLoader();
            se.Calc(p);
        }
        protected void UnLoadProxy()
        {                                                            
            if (eventNode == null)
            {
                return;
            }

            try
            {
                TEventRowCollection cl = eventNode.Collection;
                TEventRowCollectionItem cr;
                TEntryInfo ei;

                TRaceInfo ri;
                TEventRaceEntry er;

                for (int i = 0; i < cl.Count; i++)
                {
                    cr = cl[i];
                    ei = proxyNode.EntryInfoCollection[i];

                    cr.isTied = ei.IsTied;
                    cr.isGezeitet = ei.IsGezeitet;
                    cr.Race[0].CPoints = ei[0].CPoints;
                    cr.Race[0].Rank = ei[0].Rank;
                    cr.Race[0].PosR = ei[0].PosR;
                    cr.Race[0].PLZ = ei[0].PLZ;

                    for (int r = 0; r < cr.RCount; r++)
                    {
                        ri = ei[r];
                        er = cr.Race[r];
                        er.CPoints = ri.CPoints;
                        er.Drop = ri.Drop;
                        er.Rank = ri.Rank;
                        er.PosR = ri.PosR;
                        er.PLZ = ri.PLZ;
                    }                
                }
                TMain.BO.Gezeitet = proxyNode.Gezeitet;
            }
            catch
            {
            }
        }
        protected void CheckResult(TFRProxy p)
        {
            System.Diagnostics.Debug.Assert(TMain.UseDB = false,
                "cannot use standard XmlTextWriter when IO is redirected to DB");

            if (TMain.Redirector.UseDB)
            {
                return;
            }

            //string dn = "FRResult";
            string dn = TMain.FolderInfo.WorkspacePath;

            if (Directory.Exists(dn))
            {
                string en;
                en = EventName; //PrettyName
#if Desktop
                if (TMain.Controller.DocManager != null)
                    en = TMain.Controller.DocManager.EventName; //FileName, w/o prefix FR_
#endif
                //string fn = dn + Path.DirectorySeparatorChar + en + ".xml";
                string fn = TMain.FolderInfo.WorkspacePath + "FRResult\\" + en + ".xml";
                XmlTextWriter tw = new XmlTextWriter(fn, Encoding.UTF8)
                {
                    Formatting = Formatting.Indented
                };
                p.WriteXml(tw);        
                tw.Flush();
                tw.Close();
            }
        }
        public void GetProxyXmlInput(TEventNode aqn, System.Xml.XmlWriter xw)
        {
            eventNode = aqn;
            EventProps = TMain.BO.EventProps;

            if (eventNode.Collection.Count == 0)
            {
                return;
            }

            if (!CheckAnyRace())
            {
                ClearAllWithoutCalc();
            }
            else
            {
                try
            {
                proxyNode = new TFRProxy();
                LoadProxy();
                proxyNode.WriteXml(xw);
            }
            catch (Exception ex)
            {
                TCalcEvent._ScoringResult = -1;
                TCalcEvent._ScoringExceptionLocation = "TCalcEventProxy11.GetProxyXmlInput";
                TCalcEvent._ScoringExceptionMessage = ex.Message;
            }
            }
        }
        public void GetProxyXmlOutput(TEventNode aqn, System.Xml.XmlWriter xw)
        {
            eventNode = aqn;
            EventProps = TMain.BO.EventProps;

            if (eventNode.Collection.Count == 0)
            {
                return;
            }

            try
            {
                proxyNode = new TFRProxy();
                LoadProxy();
                ScoreRegatta(proxyNode); //<-- added line (not in GetProxyXmlInput)
                proxyNode.WriteXml(xw);
            }
            catch (Exception ex)
            {
                TCalcEvent._ScoringResult = -1;
                TCalcEvent._ScoringExceptionLocation = "TCalcEventProxy11.GetProxyXmlOutput";
                TCalcEvent._ScoringExceptionMessage = ex.Message;
            }

        }

        private bool CheckAnyRace()
        {
            //precondition
            if (eventNode == null)
            {
                return false;
            }

            if (eventNode.Collection.Count == 0)
            {
                return false;
            }

            //count of sailed races
            int IsRacingCount = 0;
            for (int r = 1; r <= eventNode.RaceCount; r++)
            {
                if (TMain.BO.RNode[r].IsRacing)
                {
                    IsRacingCount++;
                }
            }

            if (IsRacingCount < 1)
            {
                return false;
            }

            return true;
        }

        public void ClearAllWithoutCalc()
        {
            try
            {
                TEventRowCollection cl = eventNode.Collection;
                int posr = 0;
                foreach (TEventRowCollectionItem cr in cl)
                {
                    cr.isTied = true;
                    cr.Race[0].CPoints = 0;
                    cr.Race[0].Rank = 0;
                    cr.Race[0].PosR = posr++;
                    cr.Race[0].PLZ = cr.IndexOfRow;

                    // loop thru races
                    for (int i = 0; i < eventNode.RaceCount; i++)
                    {
                        cr.Race[i + 1].CPoints = 0;
                        cr.Race[i + 1].Drop = false;
                    }
                }
            }
            catch
            {
            }
        }

    }
}
