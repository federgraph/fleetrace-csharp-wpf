using System.Text;

namespace RiggVar.FR
{

    public class TEventRemoteObject : TEventEntry
    {
        public TEventRemoteObject(int aRaceCount)
            : base(aRaceCount)
        {
        }

        protected override void GetOutput()
        {
            SLADD("Pos", SO.ToString());
            SLADD("Bib", Bib.ToString());
            
            SLADD("SNR", SNR.ToString());
            SLADD("DN", DN);
            SLADD(FieldNames.NC, NC);
            for (int i = 1; i < RCount; i++)
            {
                SLADD("Wf" + i.ToString(), Race[i].RaceValue);
            }
            SLADD("GPoints", GRace.Points);
            SLADD("GRank", GRace.Rank.ToString());
            SLADDLAST("PosR", GRace.PosR.ToString());
        }

        public override void Assign(object source)
        {
            if (source is TEventRowCollectionItem)
            {
                TEventRowCollectionItem o = (TEventRowCollectionItem)source;
                
                SO = o.BaseID;
                Bib = o.Bib;
                
                SNR = o.SNR;
                DN = o.DN;
                NC = o.NC;
                
                for (int i = 0; i < RCount; i++)
                {
                    if (i < o.RCount)
                    {
                        Race[i].Assign(o.Race[i]);
                    }
                }
                
                Cup = o.Cup;
            }
            else if (source is TEventRemoteObject)
            {
                TEventRemoteObject e = (TEventRemoteObject)source;
                
                SO = e.SO;
                Bib = e.Bib;
                
                SNR = e.SNR;
                DN = e.DN;
                NC = e.NC;
                
                for (int i = 0; i < RCount; i++)
                {
                    if (i < e.RCount)
                    {
                        Race[i].Assign(e.Race[i]);
                    }
                }
                
                Cup = e.Cup;
            }
            else
            {
                base.Assign(source);
            }
        }

        public override string GetCommaText(TStrings SL)
        {
            if (SL == null)
            {
                return string.Empty;
            }

            SL.Clear();
            
            SL.Add(SO.ToString());
            SL.Add(Bib.ToString());
            
            SL.Add(SNR.ToString());
            SL.Add(DN);
            SL.Add(NC);

            for (int r = 1; r < RCount; r++)
            {
                SL.Add(Race[r].RaceValue);
            }

            SL.Add(GRace.Points);
            SL.Add(GRace.Rank.ToString());
            SL.Add(GRace.PosR.ToString());

            return SL.CommaText;
        }

        public override void SetCommaText(TStrings SL)
        {
            if (SL == null)
            {
                return;
            }

            TSLIterator i = new TSLIterator(SL);
            
            SO = i.NextI();
            Bib = i.NextI();
            
            SNR = i.NextI();
            DN = i.NextS();
            NC = i.NextS();
            for (int r = 1; r < RCount; r++)
            {
                Race[r].RaceValue = i.NextS();
            }

            GRace.CPoints = Utils.StrToFloatDef(i.NextS(), 0);
            GRace.Rank = i.NextI();
            GRace.PosR = i.NextI();
        }

        public override string GetCSV_Header()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Pos" + sep +
                "Bib" + sep +
                "SNR" + sep +
                "DN" + sep +
                FieldNames.NC + sep);
            for (int r = 1; r < RCount; r++)
            {
                sb.Append("R" + r.ToString() + sep);
            }

            sb.Append("GPoints" + sep + "GRank" + sep + "GPosR");
            return sb.ToString();
        }

        public string EventID { get; set; }

    }

}
