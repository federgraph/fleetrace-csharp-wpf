using System;

namespace RiggVar.FR
{
    public class TJavaScoreXML
    {
        private TStrings SL = new TDBStringList();
        private TPenaltyISAF FPenalty = new TPenaltyISAF();

        public TBO BO;
        public bool Verbose = true;
        public string RegattaName = "RegattaName";
        public string PrincipalRaceOfficer = "PRO";
        public string JuryChair = "JuryChair";
        public string HostClub = "HostClub";
        public string Dates = "Regatta Dates";
        public bool Final = false;
        public bool UseBowNumbers = false;

        public TJavaScoreXML(TBO abo)
        {
            BO = abo;
        }

//        public void WriteXML()
//        {
//            SL.Clear();
//            GetXML();
//            
//            //save localy                        
//            string fn = BO.BackupDir + "_JavaScore.xml";
//            System.IO.FileInfo fi = new System.IO.FileInfo(fn);
//            if (fi.Directory.Exists)
//                SL.SaveToFile(fn);
//
//            //and also in JavaScore Directory
//            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("C:\\Programme\\JavaScore");
//            if (di.Exists)
//                SL.SaveToFile("C:\\Programme\\JavaScore\\FR03.regatta");
//        }

        public override string ToString()
        {
            SL.Clear();
            GetXML();
            return SL.Text;
        }

        public void GetXML()
        {
            SL.Add("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            WriteRegatta();
        }

        public void GetXML(TStrings Memo)
        {
            SL.Clear();
            GetXML();
            Memo.Assign(SL);
            SL.Clear();
        }

        private void WriteRegatta()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.Append("<Regatta JSVersion=\"1.0\" Version=\"300\"");
            s.Append(" Name=\"" + RegattaName + '"');
            s.Append(" Pro=\"" + PrincipalRaceOfficer + '"');
            s.Append(" JuryChair=\"" + JuryChair + '"');
            s.Append(" HostClub=\"" + HostClub + '"');
            s.Append(" Dates=\"" + Dates + '"');
            s.Append(" Final=\"" + Utils.BoolStr[Final] + '"');
            s.Append(" UseBowNumbers=\"" + Utils.BoolStr[UseBowNumbers] + '"');
            s.Append('>');
            SL.Add(s.ToString());

            WriteDivisionList();
            WriteEntryList();
            WriteFleetList();
            WriteSubDivisionList();
            WriteRaceList();

            SL.Add("</Regatta>");
        }

        private void WriteDivisionList()
        {
            SL.Add("<DivisionList>");
            SL.Add("<Division DivName=\"" + BO.cTokenB + "\">");
            SL.Add("<MinRating System=\"OneDesign\" ClassName=\"" + BO.cTokenB + "\" />");
            SL.Add("<MaxRating System=\"OneDesign\" ClassName=\"" + BO.cTokenB + "\" />");
            SL.Add("</Division>");
            SL.Add("</DivisionList>");
        }

        private void WriteEntryList()
        {
            TEventRowCollectionItem cr;
            TEventRowCollection cl = BO.EventNode.Collection;
            SL.Add("<EntryList>");
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                SL.Add("<Entry EntryId=\"" + Utils.IntToStr(cr.BaseID) + "\" Bow=\"" + Utils.IntToStr(cr.Bib) + "\" Division=\"" + BO.cTokenB + "\">");
                SL.Add("  <Boat BoatName=\"\" SailId=\"" + Utils.IntToStr(cr.SNR) + "\">");
                if (Verbose)
                {
                    SL.Add("  <Owner FirstName=\"\" LastName=\"\" SailorId=\"\" />");
                }

                SL.Add("  <RatingList>");
                SL.Add("     <Rating System=\"OneDesign\" ClassName=\"" + BO.cTokenB + "\" />");
                SL.Add("  </RatingList>");
                SL.Add("  </Boat>");
                SL.Add("  <Skipper FirstName=\"" + cr.FN + "\" LastName=\"" + cr.LN + "\" SailorId=\"\" />");
                SL.Add("  <Crew FirstName=\"" + cr.SN + "\" LastName=\"" + cr.PB + "\" SailorId=\"\" />");
                SL.Add("</Entry>");
            }
            SL.Add("</EntryList>");
        }

        private void WriteFleetList()
        {
            SL.Add("<FleetList />");
        }

        private void WriteSubDivisionList()
        {
            SL.Add("<SubDivisionList />");
        }

        private void WriteRaceList()
        {
            string RaceID;
            bool isRacing;
            TRaceNode rn;

            SL.Add("<RaceList>");
            for (int i = 1; i <= BO.BOParams.RaceCount; i++)
            {
                rn = BO.RNode[i];
                isRacing = rn.IsRacing;
                RaceID = Utils.IntToStr(i);
                if (Verbose)
                {
                    SL.Add("<Race RaceId=\"" + RaceID + "\" Name=\"Race" + RaceID + "\" Comment=\"Conditions\" StartDate=\"\" LongDistance=\"False\" BFactor=\"0\">");
                }
                else
                {
                    SL.Add("<Race RaceId=\"" + RaceID + "\" Name=\"Race" + RaceID + "\">");
                }

                SL.Add("<DivInfo>");
                if (Verbose)
                {
                    SL.Add("  <DivStart Div=\"" + BO.cTokenB + "\" StartTime=\"No Time\" Length=\"1.0\" isRacing=\"" + Utils.BoolStr[isRacing]+ "\" />");
                }
                else
                {
                    SL.Add("  <DivStart Div=\"" + BO.cTokenB + "\" isRacing=\"" + Utils.BoolStr[isRacing]+ "\" />");
                }

                SL.Add("</DivInfo>");
                WriteFinishList(i);
                SL.Add("</Race>");
            }
            SL.Add("</RaceList>");
        }

        private void WriteFinishList(int aRace)
        {
            TEventRowCollectionItem cr;
            TEventRaceEntry ce;

            string Ent;
            string Race;
            string Pos;
            string sTime;
            string Penalty;

            TEventRowCollection cl = BO.EventNode.Collection;
            SL.Add("<FinishList>");
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                ce = cr.Race[aRace];
                FPenalty.AsInteger = ce.QU;

                Ent = Utils.IntToStr(cr.BaseID);
                Race = Utils.IntToStr(aRace);
                Pos = Utils.IntToStr(ce.OTime);
                if ((ce.OTime == 0) || (FPenalty.PenaltyNoFinish != TISAFPenaltyNoFinish.NoFinishBlank))
                {
                    Pos = TPenaltyISAF.PenaltyNoFinishString(FPenalty.PenaltyNoFinish);
                }

                Penalty = FPenalty.ToString();

                if (Penalty == "")
                {
                    if (ce.OTime > 0)
                    {
                        sTime = OTimeToFinishTime(ce.OTime); //'12:10:00';
                        //SL.Add('<Fin Ent="' + Ent + '" Race="' + Race+ '" Pos="' + Pos + '" />')
                        SL.Add("<Fin Ent=\"" + Ent + "\" Race=\"" + Race+ "\" Pos=\"" + Pos + "\" Time=\"" + sTime + "\" />");
                    }
                }
                else
                {
                    SL.Add("<Fin Ent=\"" + Ent + "\" Race=\"" + Race+ "\" Pos=\"" + Pos + "\">");
                    SL.Add("  <Penalty Penalty=\"" + Penalty + "\" />");
                    SL.Add("</Fin>");
                }
            }
            SL.Add("</FinishList>");
        }

        private string OTimeToFinishTime(int OTime)
        {
            TimeSpan dt = new TimeSpan(9, 0, OTime);

            return dt.ToString();
            //return FormatDateTime("hh:mm:ss", dt);
        }

    }
}
