namespace RiggVar.FR
{

    public class TRaceRemoteObject : TRaceEntry
    {
        private string FRunID;
        public TRaceRemoteObject(int aITCount)
            : base(aITCount)
        {
        }

        public override void Assign(object source)
        {
            if (source is TRaceRowCollectionItem o)
            {
                SO = o.BaseID;
                Bib = o.Bib;
                //
                SNR = o.SNR;
                FN = o.FN;
                LN = o.LN;
                SN = o.SN;
                NC = o.NC;
                GR = o.GR;
                PB = o.PB;
                //
                QU = o.QU.ToString();
                DG = o.DG;
                ST = o.ST.ToString();
                for (int i = 0; i < ITCount; i++)
                {
                    if (i < o.ITCount)
                    {
                        IT[i].Assign(o[i]);
                    }
                }

                MRank = o.MRank;
            }
            else if (source is TRaceRemoteObject e)
            {
                SO = e.SO;
                Bib = e.Bib;

                SNR = e.SNR;
                FN = e.FN;
                LN = e.LN;
                SN = e.SN;
                NC = e.NC;
                GR = e.GR;
                PB = e.PB;

                QU = e.QU;
                DG = e.DG;
                ST = e.ST;
                for (int i = 0; i < ITCount; i++)
                {
                    if (i < e.ITCount)
                    {
                        IT[i].Assign(e.IT[i]);
                    }
                }

                MRank = e.MRank;
            }
            else
            {
                base.Assign(source);
            }
        }
        protected override void GetOutput()
        {
            //SLADD("Pos", IntToStr(SO));
            SLADD("Bib", Bib.ToString());
            //
            SLADD("SNR", SNR.ToString());
            SLADD(FieldNames.FN, FN);
            SLADD(FieldNames.LN, LN);
            SLADD(FieldNames.SN, SN);
            SLADD(FieldNames.NC, NC);
            SLADD(FieldNames.GR, GR);
            SLADD(FieldNames.PB, PB);

            //Parameter
            int i = 0;
            string s;
            if (i == 0)
            {
                s = "FT";
            }
            else
            {
                s = "IT" + i.ToString();
            }

            SLADD(s + "Time", IT[i].OTime);
            SLADD(s + "Behind", IT[i].Behind);
            if (i > 0)
            {
                SLADD(s + "BFT", IT[i].BFT);
            }

            SLADD(s + "BPL", IT[i].BPL);
            SLADD(s + "ORank", IT[i].ORank.ToString());
            SLADD(s + "Rank", IT[i].Rank.ToString());
            SLADD(s + "PosR", IT[i].PosR.ToString());
            SLADDLAST(s + "PLZ", IT[i].PLZ.ToString());
        }
        public override string GetCommaText(TStrings SL)
        {
            if (SL == null)
            {
                return string.Empty;
            }

            SL.Clear();

            SL.Add(RunID);
            SL.Add(SO.ToString());
            SL.Add(Bib.ToString());
            //
            SL.Add(SNR.ToString());
            SL.Add(FN);
            SL.Add(LN);
            SL.Add(SN);
            SL.Add(NC);
            SL.Add(GR);
            SL.Add(PB);
            //
            SL.Add(QU);
            SL.Add(DG.ToString());
            SL.Add(ST);
            //FT
            for (int i = 1; i < ITCount; i++)
            {
                SL.Add(IT[i].OTime);
                SL.Add(IT[i].Behind);
                SL.Add(IT[i].BFT);
                SL.Add(IT[i].BPL);
                SL.Add(IT[i].ORank.ToString());
                SL.Add(IT[i].Rank.ToString());
                SL.Add(IT[i].PosR.ToString());
                SL.Add(IT[i].PLZ.ToString());
            }
            SL.Add(MRank.ToString());
            //IT
            SL.Add(ITCount.ToString());
            for (int i = 1; i < ITCount; i++)
            {
                SL.Add(IT[i].OTime);
                SL.Add(IT[i].Behind);
                SL.Add(IT[i].BFT);
                SL.Add(IT[i].BPL);
                SL.Add(IT[i].ORank.ToString());
                SL.Add(IT[i].Rank.ToString());
                SL.Add(IT[i].PosR.ToString());
                SL.Add(IT[i].PLZ.ToString());
            }

            return SL.CommaText;
        }
        public override void SetCommaText(TStrings SL)
        {
            if (SL == null)
            {
                return;
            }

            int i, c;
            i = 0;
            c = SL.Count;

            TSLIterator si = new TSLIterator(SL);
            RunID = si.NextS();
            SO = si.NextI();
            Bib = si.NextI();
            //
            SNR = si.NextI();
            FN = si.NextS();
            LN = si.NextS();
            SN = si.NextS();
            NC = si.NextS();
            GR = si.NextS();
            PB = si.NextS();
            //
            QU = si.NextS();
            DG = si.NextI();
            ST = si.NextS();
            //FT
            i = 0;
            IT[i].OTime = si.NextS();
            IT[i].Behind = si.NextS();
            //IT[i].BFT = si.NextS();
            IT[i].BPL = si.NextS();
            IT[i].ORank = si.NextI();
            IT[i].Rank = si.NextI();
            IT[i].PosR = si.NextI();
            IT[i].PLZ = si.NextI();
            MRank = si.NextI();
            //IT
            int tempITCount = si.NextI();
            //limit
            if (tempITCount > ITCount)
            {
                tempITCount = ITCount;
            }

            for (i = 1; i < tempITCount; i++)
            {
                IT[i].OTime = si.NextS();
                IT[i].Behind = si.NextS();
                IT[i].BFT = si.NextS();
                IT[i].BPL = si.NextS();
                IT[i].ORank = si.NextI();
                IT[i].Rank = si.NextI();
                IT[i].PosR = si.NextI();
                IT[i].PLZ = si.NextI();
            }
        }
        public override string GetCSV_Header()
        {
            string sep = ",";
            string result =
                "RunID" + sep +
                "Pos" + sep +
                "Bib" + sep +
                //
                "SNR" + sep +
                FieldNames.FN + sep +
                FieldNames.LN + sep +
                FieldNames.SN + sep +
                FieldNames.NC + sep +
                FieldNames.GR + sep +
                FieldNames.PB + sep +
                //
                "QU" + sep +
                "DG" + sep +
                "ST" + sep +

                "FTTime" + sep +
                "FTBehind" + sep +
                //"BFT"
                "FTBPL" + sep +
                "FTRank" + sep +
                "FTPosR" + sep +
                "FTPLZ" + sep +

                "MRank" + sep +
                "ITCount";

            string s;
            for (int i = 1; i < ITCount; i++)
            {
                s = "IT" + i.ToString();
                result += sep +
                    s + "Time" + sep +
                    s + "Behind" + sep +
                    s + "BFT" + sep +
                    s + "BPL" + sep +
                    s + "Rank" + sep +
                    s + "PosR" + sep +
                    s + "PLZ";
            }
            return result;
        }

        public override string RunID
        {
            get => FRunID;
            set => FRunID = value;
        }
    }

}
