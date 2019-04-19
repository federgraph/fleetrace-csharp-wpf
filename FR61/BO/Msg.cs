#define StatusFeedback


namespace RiggVar.FR
{
    public class TBOMsgFactory : TMsgFactory
    {
        public override TBaseMsgList CreateMsg()
        {
            return new TBOMsgList();
        }
    }

    public class TBOMsgList : TBaseMsgList
    {
        public TBOMsgList() : base()
        {
            KatID = LookupKatID.FR;
        }
        protected override TBaseMsg NewMsg()
        {
            return new TBOMsg();
        }
    }

    public class TBOMsg : TBaseMsg
    {
        public TBO BO;
        public TMsgParser MsgParser;
        public int ItemPos;
        public int AthleteID;

        public TBOMsg() : base()
        {
            BO = TMain.BO;
            MsgParser = new TMsgParser();
            KatID = LookupKatID.FR;
        }

        private TRaceRowCollectionItem FindCR()
        {
            TRaceNode qn = BO.FindNode(RunID);
            if (qn != null)
            {
                if (ItemPos > 0)
                {
                    return qn.Collection[ItemPos - 1];
                }
                else
                {
                    return qn.FindBib(Bib);
                }
            }
            else
            {
                return null;
            }
        }

        private string GetColName()
        {
            if (Utils.Copy(RunID, 1, 1) != "W")
            {
                return string.Empty;
            }

            string s = Utils.Copy(RunID, 2, RunID.Length);
            int i = Utils.StrToIntDef(s, -1);
            if ((i < 1) || (i > BO.BOParams.RaceCount))
            {
                return string.Empty;
            }

            return "col_R" + i.ToString();
        }

        public override void ClearResult()
        {
            base.ClearResult();
            ItemPos = -1;
            AthleteID = -1;
        }

        public override bool DispatchProt()
        {
            ClearResult();

            //Comments-----------------------------------
            if ((Prot == "") || Prot.StartsWith("//") || Prot.StartsWith("#"))
            {
                MsgType = TMsgType.Comment;
                return true;
            }

            //Management Commands------------------------
            if (Prot.StartsWith("Manage."))
            {
#if StatusFeedback
                BO.StatusFeedback.ParseLine(Prot);
#endif
                return true;
            }

            //Properties---------------------------------
            if (Prot.StartsWith("EP.") || Prot.StartsWith("Event.Prop_"))
            {
                BO.EventProps.ParseLine(Prot);                
                MsgType = TMsgType.Prop;
                return true;
            }

            //ignore params------------------------------
            if (Prot.StartsWith("DP.") || Prot.StartsWith("Event."))
            {
                return true;
            }
        
            //Data---------------------------------------
            if (Prot.StartsWith(BO.cTokenModul))
            {
                return ParseProt();
            }

            return false;
        }

        private bool ParseProt()
        {
            MsgType = TMsgType.Input;
            bool result = MsgParser.Parse(Prot);
            if (result)
            {
                MsgType = MsgParser.MsgType;
                MsgKey = MsgParser.MsgKey;
                MsgValue = MsgParser.sValue;
                
                Division = MsgParser.sDivision;
                RunID = MsgParser.sRunID;
                Bib = Utils.StrToIntDef(MsgParser.sBib, -1);
                Cmd = MsgParser.sCommand;
                ItemPos = Utils.StrToIntDef(MsgParser.sPos, -1);
                AthleteID = Utils.StrToIntDef(MsgParser.sAthlete, -1);
                DBID = Utils.StrToIntDef(MsgParser.sMsgID, -1);
                
                HandleProt();
            }
            return result;
        }

        public void HandleProt()
        {
            MsgResult = 1;
            bool MsgHandled = false;

            //Testmessage
            if (Cmd == "XX")
            {
                //if (Verbose) Trace("HandleProt: Testmessage");
                MsgType = TMsgType.Test;
            }

            else if (Cmd == "Count")
            {
                MsgHandled = BO.UpdateStartlistCount(RunID, Utils.StrToIntDef(MsgValue, -1));
                MsgType = TMsgType.Param;
            }
            else if (AthleteID > 0)
            {
                MsgHandled = BO.UpdateAthlete(AthleteID, Cmd, MsgValue);
            }

            else if (Cmd == "IsRacing")
            {
                if (BO.FindNode(RunID) != null)
                {
                    BO.FindNode(RunID).IsRacing = (MsgValue == Utils.BoolStr[true]);
                }

                MsgType = TMsgType.Option;
            }

            else
            {
                string temp = MsgValue.ToLower();
                if ((temp == "empty") || (temp == "null") || (temp == "99:99:99.99"))
                {
                    MsgValue = "-1";
                }

                TRaceRowCollectionItem cr = FindCR();
                if (cr != null)
                {
                    MsgHandled = HandleMsg(cr);
                }
            }

            if (MsgHandled)
            {
                BO.CounterMsgHandled++;
                MsgResult = 0;
            }
        }
        public bool HandleMsg(TRaceRowCollectionItem cr)
        {
            TRaceBO o = cr.Ru.ColBO;
            string s = MsgValue;

            if ((Cmd == "ST") || (Cmd == "SC"))
            {
                o.EditST(cr, ref s);
            }
            else if ((Utils.Copy(Cmd, 1, 2) == "IT") || (Utils.Copy(Cmd, 1, 2) == "FC"))
            {
                int channel = Utils.StrToIntDef(Utils.Copy(Cmd, 3, Cmd.Length),  -1);
                if (channel > -1)
                {
                    o.EditIT(cr, ref s, "col_IT" + channel.ToString());
                }
            }

            else if ((Cmd == "FT") || (Cmd == "FC"))
            {
                o.EditFT(cr, ref s);
            }
            else if (Cmd == "QU")
            {
                o.EditQU(cr, ref s);
            }
            else if (Cmd == "DG")
            {
                o.EditDG(cr, ref s);
            }
            else if (Cmd == "Rank")
            {
                o.EditOTime(cr, ref s);
            }
            else if (Cmd == "RV")
            {
                TEventRowCollectionItem crev = BO.EventNode.Collection[cr.IndexOfRow];
                if (crev != null)
                {
                    BO.EventNode.ColBO.EditRaceValue(crev, ref s, GetColName());
                }
            }
            else if (Cmd == "FM")
            {
                TEventRowCollectionItem crev = BO.EventNode.Collection[cr.IndexOfRow];
                int r = GetRaceIndex();
                if (r != -1)
                {
                    crev.Race[r].Fleet = Utils.StrToIntDef(s, crev.Race[r].Fleet);
                }
            }

            else if (Cmd == "Bib")
            {
                o.EditBib(cr, ref s); //--> wird horizontal kopiert, bo.Bib[Index] := cr.Bib
            }
            else if (Cmd == "SNR")
            {
                o.EditSNR(cr, ref s); //--> wird horizontal kopiert, bo.SNR[Index] := cr.SNR
            }

            return true;
        }

        private int GetRaceIndex()
        {
            if (!RunID.StartsWith("W"))
            {
                return -1;
            }

            string s = RunID.Substring(1);
            int i = Utils.StrToIntDef(s, -1);
            if (i < 1 || i > BO.BOParams.RaceCount)
            {
                i = -1;
            }

            return i;
        }

    }

}
