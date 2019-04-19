using System.Xml.Linq;

namespace RiggVar.FR
{
    public class TFRXMLWriter : TExcelExporter
    {

        public void WriteXml(TStrings Memo)
        {
            XElement xe = new XElement("FR");

            WriteContent(xe);
            Memo.Text = xe.ToString();
        }

        private void AddEventProp(XElement xe, string K, string V)
        {
            XElement o = new XElement("EP");
            o.Add(new XAttribute("K", K));
            o.Add(new XAttribute("V", V));
            xe.Add(o);
        }
        private void AddEventProps(XElement xe)
        {
            TBO BO = TMain.BO;

            xe.Add(new XAttribute("StartlistCount", Utils.IntToStr(BO.BOParams.StartlistCount)));
            xe.Add(new XAttribute("ITCount", Utils.IntToStr(BO.BOParams.ITCount)));
            xe.Add(new XAttribute("RaceCount", Utils.IntToStr(BO.BOParams.RaceCount)));
            xe.Add(new XAttribute("DivisionName", BO.EventProps.DivisionName));
            xe.Add(new XAttribute("InputMode", TEventProps.InputModeStrings[BO.EventProps.InputMode]));

            TEventProps ep = TMain.BO.EventProps;
            
            AddEventProp(xe, "Name", ep.EventName);

            if (!string.IsNullOrEmpty(ep.EventDates))
            {
                AddEventProp(xe, "Dates", ep.EventDates);
            }

            if (!string.IsNullOrEmpty(ep.HostClub))
            {
                AddEventProp(xe, "HostClub", ep.HostClub);
            }

            if (!string.IsNullOrEmpty(ep.PRO))
            {
                AddEventProp(xe, "PRO", ep.PRO);
            }

            if (!string.IsNullOrEmpty(ep.JuryHead))
            {
                AddEventProp(xe, "JuryHead", ep.JuryHead);
            }

            xe.Add(new XElement("ScoringSystem", TEventProps.ScoringSystemStruct[ep.ScoringSystem]));

            if (ep.ScoringSystem2 != 0)
            {
                AddEventProp(xe, "ScoringSystem2", Utils.IntToStr(ep.ScoringSystem2));
            }

            AddEventProp(xe, "Throwouts", Utils.IntToStr(ep.Throwouts));

            if (ep.ThrowoutScheme != TThrowoutScheme.throwoutBYNUMRACES)
            {
                AddEventProp(xe, "ThrowoutScheme", TEventProps.ThrowoutSchemeStruct[ep.ThrowoutScheme]);
            }

            if (ep.FirstIs75)
            {
                AddEventProp(xe, "FirstIs75", "True");
            }

            if (!ep.ReorderRAF)
            {
                AddEventProp(xe, "ReorderRAF", "False");
            }

            AddEventProp(xe, "DivisionName", ep.DivisionName);
            AddEventProp(xe, "InputMode", TEventProps.InputModeStrings[ep.InputMode]);
            AddEventProp(xe, "RaceLayout", ep.RaceLayout);
            AddEventProp(xe, "NameSchema", ep.NameSchema);
            AddEventProp(xe, "FieldMap", ep.FieldMap);
            AddEventProp(xe, "FieldCaptions", ep.FieldCaptions);
            AddEventProp(xe, "FieldCount", ep.FieldCount);
            AddEventProp(xe, "NameFieldCount", ep.NameFieldCount);
            AddEventProp(xe, "NameFieldOrder", ep.NameFieldOrder);

            if (ep.ShowPosRColumn)
            {
                AddEventProp(xe, "ShowPosRColumn", Utils.BoolStr[ep.ShowPosRColumn]);
            }

            if (ep.ShowCupColumn)
            {
                AddEventProp(xe, "ShowCupColumn", Utils.BoolStr[ep.ShowCupColumn]);
            }

            AddEventProp(xe, "ColorMode", ep.ColorMode);
            AddEventProp(xe, "UseFleets", Utils.BoolStr[ep.UseFleets]);
            AddEventProp(xe, "TargetFleetSize", Utils.IntToStr(ep.TargetFleetSize));
            AddEventProp(xe, "FirstFinalRace", Utils.IntToStr(ep.FirstFinalRace));
            AddEventProp(xe, "IsTimed", Utils.BoolStr[ep.IsTimed]);
            AddEventProp(xe, "UseCompactFormat", Utils.BoolStr[ep.UseCompactFormat]);

            if (ep.ShowCupColumn)
            {
                AddEventProp(xe, "Uniqua.Faktor", string.Format("{0:0.00}", ep.Faktor));
                AddEventProp(xe, "Unica.Enabled", Utils.BoolStr[ep.EnableUniquaProps]);
                AddEventProp(xe, "Unica.Gesegelt", Utils.IntToStr(ep.Gesegelt));
                AddEventProp(xe, "Unica.Gemeldet", Utils.IntToStr(ep.Gemeldet));
                AddEventProp(xe, "Unica.Gezeitet", Utils.IntToStr(ep.Gezeitet));
            }
        }

        private void AddColCaptions(XElement xe)
        {
            GetCaptionList();
            foreach (string s in SL)
            {
                xe.Add(new XElement("CL", s));
            }
        }

        private void AddNameList(XElement xe)
        {
            GetNameList(TMain.BO);
            foreach (string s in SL)
            {
                xe.Add(new XElement("NL", s));
            }
        }

        private void AddEntries(XElement xe)
        {
            XElement o;
            TStammdatenRowCollectionItem cr;
            TStammdatenRowCollection cl = TMain.BO.StammdatenNode.Collection;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                o = new XElement("SNR");
                xe.Add(o);
                o.Add(new XAttribute("oID", Utils.IntToStr(cr.SNR)));
                for (int j = 0; j < cl.FieldCount; j++)
                {
                    o.Add(new XAttribute("N" + Utils.IntToStr(j + 1), cr[j + 1]));
                }
            }
        }

        private void AddStartList(XElement xe)
        {
            TEventRowCollection cl = TMain.BO.EventNode.Collection;
            int i = 0;
            foreach (TEventRowCollectionItem cr in cl)
            {
                i++;
                XElement o = new XElement("Pos");
                xe.Add(o);
                o.Add(new XAttribute("oID", Utils.IntToStr(i)));
                o.Add(new XAttribute("Bib", Utils.IntToStr(cr.Bib)));
                o.Add(new XAttribute("SNR", Utils.IntToStr(cr.SNR)));
            }
        }

        private void AddFleetList(XElement xe)
        {
            GetFleetList(TMain.BO);
            foreach (string s in SL)
            {
                xe.Add(new XElement("FL", s));
            }
        }

        private void AddFinishList(XElement xe)
        {
            GetFinishList(TMain.BO);
            foreach (string s in SL)
            {
                xe.Add(new XElement("FL", s));
            }
        }

        private void AddTimeList(XElement xe)
        {
            for (int r = 1; r <= TMain.BO.BOParams.RaceCount; r++)
            {
                XElement o = new XElement("TimeList");
                xe.Add(o);
                o.Add(new XAttribute("RaceID", 'R' + Utils.IntToStr(r)));
                GetTimeList(r, TMain.BO);
                for (int i = 0; i < SL.Count; i++)
                {
                    xe.Add(new XElement("FL", SL[i]));
                }
            }
        }

        private void SaveLine(object o, string s)
        {
            SL.Add(s);
        }

        private void AddMsgList(XElement xe)
        {
            TRaceRowCollection cl;
            TRaceRowCollectionItem cr;
            TRaceNode rn;
            TDivision g;
            TInputAction InputAction;
            TRun r;
            TEventRaceEntry ere;

            SL.Clear();
            InputAction = new TInputAction();
            InputAction.OnSend = new TInputAction.ActionEvent(SaveLine);
            TInputActionManager.DynamicActionRef = InputAction;
            try
            {
                TBO BO = TMain.BO;
                // rest of messages...
                for (int n = 1; n <= BO.BOParams.RaceCount; n++)
                {
                    rn = BO.RNode[n];
                    g = BO.MsgTree.Division;
                    cl = rn.Collection;
                    if (n == 1)
                    {
                        r = g.Race1;
                    }
                    else if (n > 1 && n <= BO.BOParams.RaceCount)
                    {
                        r = g.Race(n);
                    }
                    else
                    {
                        r = null;
                    }

                    if (r == null)
                    {
                        continue;
                    }

                    if (!rn.IsRacing)
                    {
                        r.IsRacing(Utils.BoolStr[false]);
                    }

                    for (int i = 0; i < cl.Count; i++)
                    {
                        cr = cl[i];
                        if (i == 0 && cr.ST.TimePresent)
                        {
                            r.Bib(cr.Bib).ST(cr.ST.AsString);
                        }

                        if (BO.EventNode.UseFleets)
                        {
                            ere = BO.EventNode.Collection[i].Race[n];
                            if (!ere.IsRacing)
                            {
                                r.Bib(cr.Bib).RV("x");
                            }
                        }

                        if (cr.QU.AsInteger != 0)
                        {
                            r.Bib(cr.Bib).QU(cr.QU.ToString());
                        }

                        if (cr.DG > 0)
                        {
                            r.Bib(cr.Bib).DG(Utils.IntToStr(cr.DG));
                        }
                    }
                }
            }
            finally
            {
                TInputActionManager.DynamicActionRef = null;
                //InputAction.Free;
            }

            for (int i = 0; i < SL.Count; i++)
            {
                xe.Add(new XElement("ML", SL[i]));
            }
        }

        private void AddErrorList(XElement xe)
        {
            SL.Clear();
            TMain.BO.EventNode.ErrorList.GetMsg(SL);
            for (int i = 0; i < SL.Count; i++)
            {
                xe.Add(new XElement("EL"), SL[i]);
            }
        }

        private void WriteContent(XElement FR)
        {
            XElement xe;

            xe = new XElement("Properties");
            FR.Add(xe);
            AddEventProps(xe);

            if (TColCaptions.ColCaptionBag.IsPersistent && TColCaptions.ColCaptionBag.Count > 0)
            {
                xe = new XElement("ColCaptions");
                FR.Add(xe);
                AddColCaptions(xe);
            }

            //xe = new XElement("NameList");
            //FR.Add(xe);
            //AddNameList(xe);

            xe = new XElement("Entries");
            FR.Add(xe);
            AddEntries(xe);

            xe = new XElement("StartList");
            FR.Add(xe);
            AddStartList(xe);

            if (TMain.BO.EventNode.UseFleets)
            {
                xe = new XElement("FleetList");
                FR.Add(xe);
                AddFleetList(xe);
            }

            xe = new XElement("FinishList");
            FR.Add(xe);
            AddFinishList(xe);

            if (TMain.BO.BOParams.ITCount > 0 || TMain.BO.EventProps.IsTimed)
            {
                AddTimeList(FR);
            }

            xe = new XElement("MsgList");
            FR.Add(xe);
            AddMsgList(xe);

            TMain.BO.EventNode.ErrorList.CheckAll(TMain.BO.EventNode);
            if (TMain.BO.EventNode.ErrorList.HasErrors())
            {
                xe = new XElement("ErrorList");
                FR.Add(xe);
                AddErrorList(xe);
            }
        }

    }

}
