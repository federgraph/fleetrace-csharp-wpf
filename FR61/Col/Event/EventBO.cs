#define Undo

using System.Collections.Generic;

namespace RiggVar.FR
{
    public class TEventBO : TBaseColBO<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
        >
    {
        List<TEventRowCollectionItem> FL;
        private bool FRelaxedInputMode;
        public int NameFieldCount;
        public string NameFieldOrder;
        public bool WantDiffCols;

#if Undo
        public TUndoAgent UndoAgent;
#endif
        public TEventBO()
            : base()
        {
            FL = new List<TEventRowCollectionItem>();
            NameFieldCount = 2;
            NameFieldOrder = "041256";

#if Undo
            UndoAgent = TMain.BO.UndoAgent;
#endif
        }

        private int RaceCount => TMain.BO.BOParams.RaceCount;

        public bool RelaxedInputMode
        {
            get => FRelaxedInputMode;
            set
            {
#if Undo
                bool oldValue = FRelaxedInputMode;
#endif
                if (value)
                {
                    FRelaxedInputMode = true;
                }
                else
                {
                    TEventNode ev = TMain.BO.EventNode;
                    if (ev.ErrorList.IsPreconditionForStrictInputMode(ev))
                    {
                        FRelaxedInputMode = false; //strict mode on
                    }
                }
#if Undo
                if ((UndoAgent.UndoLock == false) && (oldValue != FRelaxedInputMode))
                {
                    string undoMsg;
                    string redoMsg;
                    if (oldValue)
                    {
                        undoMsg = "EP.InputMode = Relaxed";
                        redoMsg = "EP.InputMode = Strict";
                    }
                    else
                    {
                        undoMsg = "EP.InputMode = Strict";
                        redoMsg = "EP.InputMode = Relaxed";
                    }
                    TMain.BO.UndoManager.AddMsg(undoMsg, redoMsg);
                    UndoAgent.UndoLock = false;
                }
#endif
            }
        }

        public override void InitColsActive(TEventColGrid g)
        {
            InitColsActiveLayout(g, 0);
        }

        public override void InitColsActiveLayout(TEventColGrid g, int aLayout)
        {
            TEventColProp cp;
            g.ColsActive.Clear();
            g.AddColumn("col_BaseID");

            cp = g.AddColumn("col_SNR");
            cp.OnFinishEdit = new TEventColGrid.TBaseGetTextEvent(EditSNR);
            cp.ReadOnly = false;

            cp = g.AddColumn("col_Bib");
            cp.OnFinishEdit = new TEventColGrid.TBaseGetTextEvent(EditBib);
            cp.ReadOnly = false;

            switch (aLayout)
            {
                case 1:
                    g.AddColumn("col_DN");
                    g.AddColumn("col_NC");
                    break;

                case 2:
                    g.AddColumn("col_FN");
                    g.AddColumn("col_LN");
                    g.AddColumn("col_NC");
                    break;

                case 3:
                    g.AddColumn("col_FN");
                    g.AddColumn("col_LN");
                    g.AddColumn("col_SN");
                    g.AddColumn("col_NC");
                    break;

                default:
                    string s;
            for (int i = 1; i <= NameFieldCount; i++)
            {
                s = GetNameFieldID(i);
                if (s != "")
                {
                    cp = g.AddColumn(s);
                    if (s == "col_NC")
                    {
                        cp.OnFinishEdit = new TEventColGrid.TBaseGetTextEvent(EditNC);
                        cp.ReadOnly = false;
                    }
                }
            }
                    break;
            }

            int rc = RaceCount;
            for (int i = 1; i <= rc; i++)
            {
                cp = g.AddColumn("col_R" + i.ToString());
                cp.OnFinishEdit2 = new TEventColGrid.TBaseGetTextEvent2(EditRaceValue);
                cp.ReadOnly = false;
            }

            g.AddColumn("col_GPoints");
            if (TMain.BO.EventNode.WebLayout == 0)
            {
                g.AddColumn("col_GRank");
            }

            if (TMain.BO.EventProps.ShowPosRColumn)
            {
                g.AddColumn("col_GPosR");
            }

            if (TMain.BO.EventProps.ShowPLZColumn)
            {
                g.AddColumn("col_PLZ");
            }

            if (TMain.BO.EventProps.ShowCupColumn) //if (aLayout > 0)
            {
                g.AddColumn("col_Cup");
            }
        }

        private string GetNameFieldID(int Index)
        {
            char c;

            if (Index < 1 || Index > 6)
            {
                return "";
            }

            if (Index <= NameFieldOrder.Length)
            {
                c = NameFieldOrder[Index - 1];
            }
            else
            {
                string s = Index.ToString();
                c = s[0];
            }

            switch (c)
            {
                case '0':
                    return "col_DN";
                case '1':
                    return "col_FN";
                case '2':
                    return "col_LN";
                case '3':
                    return "col_SN";
                case '4':
                    return "col_NC";
                case '5':
                    return "col_GR";
                case '6':
                    return "col_PB";
            }

            return "";
        }

        public void EditSNR(TEventRowCollectionItem cr, ref string Value)
        {
            cr.SNR = Utils.StrToIntDef(Value, cr.SNR);
            Value = Utils.IntToStr(cr.SNR);
            TMain.BO.SetSNR(cr.IndexOfRow, cr.SNR);
        }

        public void EditBib(TEventRowCollectionItem cr, ref string Value)
        {
#if Undo
            int oldBib = cr.Bib;
#endif
            cr.Bib = Utils.StrToIntDef(Value, cr.Bib);
            Value = Utils.IntToStr(cr.Bib);
            TMain.BO.SetBib(cr.IndexOfRow, cr.Bib);
#if Undo
            if (oldBib != cr.Bib)
            {
                UndoAgent.UndoFlag = true;
                UndoAgent.MsgTree.Division.Race1.Startlist.Pos(cr.BaseID).Bib(oldBib.ToString());
                UndoAgent.MsgTree.Division.Race1.Startlist.Pos(cr.BaseID).Bib(cr.Bib.ToString());
                UndoAgent.UndoLock = false;
            }
#endif
        }

        public void EditNC(TEventRowCollectionItem cr, ref string Value)
        {
            TStammdatenRowCollection cl = TMain.BO.StammdatenNode.Collection;
            TStammdatenRowCollectionItem crs = cl.FindKey(cr.SNR);
            TMain.BO.StammdatenBO.EditNC(crs, ref Value);
        }

        private bool IsFleetInputChar(char c)
        {
            switch (c)
            {
                case 'y': return true;
                case 'b': return true;
                case 'r': return true;
                case 'g': return true;
                case 'm': return true;
                default: return false;
            }
        }

        public void EditRaceValue(TEventRowCollectionItem cr, ref string Value, string ColName)
        {
            if (cr != null)
            {
                int i;
                try
                {
                    i = int.Parse(ColName.Substring(5));
                }
                catch
                {
                    i = -1;
                }

                if ((i < 1) || (i > RaceCount))
                {
                    return;
                }

                if (Value == "$")
                {
                    bool oldIsRacing = TMain.BO.RNode[i].IsRacing;
                    TMain.BO.RNode[i].IsRacing = !oldIsRacing;
#if Undo
                    if (!UndoAgent.UndoLock)
                    {
                        UndoAgent.UndoFlag = true;
                        UndoAgent.MsgTree.Division.Race(i).IsRacing(Utils.BoolStr[oldIsRacing]); //undoMsg
                        UndoAgent.MsgTree.Division.Race(i).IsRacing(Utils.BoolStr[!oldIsRacing]); //redoMsg
                        UndoAgent.UndoLock = false;
                    }
#endif
                    cr.Modified = true;
                }

                //else if (Value == "§")
                //{
                //    RelaxedInputMode = ! RelaxedInputMode;
                //    BO.RNode[i].IsTied := ! BO.RNode[i].IsTied;
                //}

                else if (Utils.StrToIntDef(Value, -1) > -1)
                {
                    EditOTime(cr, ref Value, i);
                }

                //Fleet Assignment, easy edit
                else if (Value.Length == 1 && IsFleetInputChar(Value[0]))
                {
                    TEventRaceEntry re = cr.Race[i];
                    char c = Value[0];
                    switch (c)
                    {
                        case 'y': re.Fleet = 1; break; //yellow
                        case 'b': re.Fleet = 2; break; //blue
                        case 'r': re.Fleet = 3; break; //red
                        case 'g': re.Fleet = 4; break; //green
                        case 'm': re.Fleet = 0; break; //medal
                    }
                }

                //Fleet Assignment, general method
                else if (Value.Length > 1 && Value[0] == 'F')
                {
                    TEventRaceEntry re = cr.Race[i];
                    re.RaceValue = Value; //do not broadcast Fleet Assignments
                    //cr.Modified = true;
                    //Value = re.RaceValue;
                }

                else if (Value == "x")
                {
                    cr.Race[i].IsRacing = false;
                    //cr.Modified = true; //use CalcBtn
                }

                else if (Value == "-x")
                {
                    cr.Race[i].IsRacing = true;
                    //cr.Modified = true; //use CalcBtn
                }

                else
                {
                    int oldQU = cr.Race[i].QU;
                    string oldQUString = cr.Race[i].Penalty.ToString();
#if Undo
                    string undoValue = cr.Race[i].Penalty.Invert(Value);
                    string redoValue = Value;
#endif
                    cr.Race[i].RaceValue = Value;

                    Value = cr.Race[i].RaceValue;
                    cr.Modified = true;

                    int newQU = cr.Race[i].QU;
                    string newQUString = cr.Race[i].Penalty.ToString();

                    if ((oldQU != newQU) || (oldQUString != newQUString))
                    {
                        TMain.BO.SetPenalty(i, cr.IndexOfRow, cr.Race[i].Penalty);
#if Undo
                        if (!UndoAgent.UndoLock)
                        {
                            UndoAgent.UndoFlag = true;
                            UndoAgent.MsgTree.Division.Race(i).Bib(cr.Bib).RV(undoValue);
                            UndoAgent.MsgTree.Division.Race(i).Bib(cr.Bib).RV(redoValue);
                            UndoAgent.UndoLock = false;
                        }
#endif
                    }
                }
            }
        }
        public void EditOTime(TEventRowCollectionItem cr, ref string Value, int RaceIndex)
        {
            TEventRowCollection cl;

            if (cr != null)
            {
                cl = cr.Collection;
            }
            else
            {
                return;
            }

            TEventRaceEntry re = cr.Race[RaceIndex];

            int oldRank;
            int newRank;

            //mode a: direkt input, minimal restriction
            if (RelaxedInputMode)
            {
                oldRank = re.OTime;
                newRank = Utils.StrToIntDef(Value, oldRank);
                if ((newRank >= 0) && (newRank <= cl.Count) && (newRank != oldRank))
                {
                    re.OTime = newRank;
                    TMain.BO.RNode[RaceIndex].Collection[cr.IndexOfRow].MRank = newRank;
                    cr.Modified = true;
                }
#if Undo
                if (!UndoAgent.UndoLock)
                {
                    UndoAgent.UndoFlag = true;
                    UndoAgent.MsgTree.Division.Race(RaceIndex).Bib(cr.Bib).RV(oldRank.ToString());
                    UndoAgent.MsgTree.Division.Race(RaceIndex).Bib(cr.Bib).RV(newRank.ToString());
                    UndoAgent.UndoLock = false;
                }
#endif
                Value = re.OTime.ToString();
            }

            //mode b: maintain contiguous rank from 1 to maxrank
            else
            {
                oldRank = re.OTime;
                CheckOTime(cl, cr, RaceIndex, ref Value);
                newRank = re.OTime;
                if (oldRank != newRank)
                {
                    TMain.BO.CopyOTimes(RaceIndex);
                    cr.Modified = true;
#if Undo
                    if ((!UndoAgent.UndoLock) && (oldRank != newRank))
                    {
                        UndoAgent.UndoFlag = true;
                        UndoAgent.MsgTree.Division.Race(RaceIndex).Bib(cr.Bib).RV(oldRank.ToString());
                        UndoAgent.MsgTree.Division.Race(RaceIndex).Bib(cr.Bib).RV(newRank.ToString());
                        UndoAgent.UndoLock = false;
                    }
#endif
                }
            }

        }

        private void CheckOTime(
            TEventRowCollection cl,
            TEventRowCollectionItem cr,
            int r,
            ref string Value)
        {
            if (TMain.BO.EventNode.UseFleets)
            {
                int f = cr.Race[r].Fleet;
                cl.FillFleetList(FL, r, f);
                CheckOTimeForFleet(FL, cr, r, ref Value);
                FL.Clear();
            }
            else
            {
                CheckOTimeForAll(cl, cr, r, ref Value);
            }
        }

        private void CheckOTimeForFleet(
            List<TEventRowCollectionItem> cl,
            TEventRowCollectionItem cr,
            int r,
            ref string Value)
        {
            TEventRaceEntry er;

            TEventRowCollectionItem cr1;
            TEventRaceEntry er1;

            int oldRank;
            int newRank;
            int maxRank;

            er = cr.Race[r];
            oldRank = er.OTime;
            newRank = Utils.StrToIntDef(Value, er.OTime);
            maxRank = 0;
            for (int i = 0; i < cl.Count; i++)
            {
                cr1 = (TEventRowCollectionItem)cl[i];
                er1 = cr1.Race[r];
                if (cr == cr1)
                {
                    continue;
                }
                else if (er1.OTime > 0)
                {
                    maxRank++;
                }
            }

            //limit new value
            if (newRank < 0)
            {
                newRank = 0;
            }

            if (newRank > maxRank + 1)
            {
                newRank = maxRank + 1;
            }

            if (newRank > cl.Count)
            {
                newRank = cl.Count;
            }

            if (oldRank == newRank)
            {
                Value = er.OTime.ToString();
            }
            else
            {
                for (int i = 0; i < cl.Count; i++)
                {
                    cr1 = (TEventRowCollectionItem)cl[i];
                    er1 = cr1.Race[r];
                    if (cr1 == cr)
                    {
                        continue;
                    }

                    int temp = er1.OTime;
                    //remove
                    if ((oldRank > 0) && (oldRank < temp))
                    {
                        er1.OTime = temp - 1;
                    }
                    //insert
                    if ((newRank > 0) && (newRank <= er1.OTime))
                    {
                        er1.OTime = er1.OTime + 1;
                    }
                }
                cr.Race[r].OTime = newRank;
                Value = er.OTime.ToString();
            }
        }

        private void CheckOTimeForAll(
            TEventRowCollection cl,
            TEventRowCollectionItem cr,
            int r,
            ref string Value)
        {
            TEventRaceEntry er;

            TEventRowCollectionItem cr1;
            TEventRaceEntry er1;

            int oldRank;
            int newRank;
            int maxRank;

            er = cr.Race[r];
            oldRank = er.OTime;
            newRank = Utils.StrToIntDef(Value, er.OTime);
            maxRank = 0;
            for (int i = 0; i < cl.Count; i++)
            {
                cr1 = cl[i];
                er1 = cr1.Race[r];
                if (cr == cr1)
                {
                    continue;
                }
                else if (er1.OTime > 0)
                {
                    maxRank++;
                }
            }

            //limit new value
            if (newRank < 0)
            {
                newRank = 0;
            }

            if (newRank > maxRank + 1)
            {
                newRank = maxRank + 1;
            }

            if (newRank > cl.Count)
            {
                newRank = cl.Count;
            }

            if (oldRank == newRank)
            {
                Value = er.OTime.ToString();
            }
            else
            {
                for (int i = 0; i < cl.Count; i++)
                {
                    cr1 = cl[i];
                    er1 = cr1.Race[r];
                    if (cr1 == cr)
                    {
                        continue;
                    }

                    int temp = er1.OTime;
                    //remove
                    if ((oldRank > 0) && (oldRank < temp))
                    {
                        er1.OTime = temp - 1;
                    }
                    //insert
                    if ((newRank > 0) && (newRank <= er1.OTime))
                    {
                        er1.OTime = er1.OTime + 1;
                    }
                }
                cr.Race[r].OTime = newRank;
                Value = er.OTime.ToString();
            }
        }

    }

}
