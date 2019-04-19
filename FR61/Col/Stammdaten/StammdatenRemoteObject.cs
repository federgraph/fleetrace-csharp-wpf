namespace RiggVar.FR
{

    public class TStammdatenRemoteObject : TStammdatenEntry
    {

        protected override void GetOutput()
        {
            SLADD("SNR", SNR.ToString());
            SLADD(FieldNames.FN, FN);
            SLADD(FieldNames.LN, LN);
            SLADD(FieldNames.SN, SN);
            SLADD(FieldNames.NC, NC);
            SLADD(FieldNames.GR, GR);
            SLADDLAST(FieldNames.PB, PB);
        }

        public override void Assign(object source)
        {
            if (source is TStammdatenRowCollectionItem e)
            {
                SNR = e.SNR;
                FN = e.FN;
                LN = e.LN;
                SN = e.SN;
                NC = e.NC;
                GR = e.GR;
                PB = e.PB;
            }
            else if (source is TStammdatenRemoteObject r)
            {
                SNR = r.SNR;
                FN = r.FN;
                LN = r.LN;
                SN = r.SN;
                NC = r.NC;
                GR = r.GR;
                PB = r.PB;
            }
            else
            {
                base.Assign(source);
            }
        }

        public override string GetCommaText(TStrings SL)
        {
            if (Equals(SL, null))
            {
                return string.Empty;
            }

            SL.Clear();

            SL.Add(SNR.ToString());
            SL.Add(FN);
            SL.Add(LN);
            SL.Add(SN);
            SL.Add(NC);
            SL.Add(GR);
            SL.Add(PB);

            return SL.CommaText;
        }

        public override void SetCommaText(TStrings SL)
        {
            if (Equals(SL, null))
            {
                return;
            }
            
            TSLIterator i = new TSLIterator(SL);
            
            SNR = i.NextI();
            FN = i.NextS();
            LN = i.NextS();
            SN = i.NextS();
            NC = i.NextS();
            GR = i.NextS();
            PB = i.NextS();
        }

        public override string GetCSV_Header()
        {
            return "SNR" + sep +
                FieldNames.FN + sep +
                FieldNames.LN + sep +
                FieldNames.SN + sep +
                FieldNames.NC + sep +
                FieldNames.GR + sep +
                FieldNames.PB;
        }

    }

}
