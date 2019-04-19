namespace RiggVar.FR
{
    public class TNameFieldRowCollection : TBaseRowCollection<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
        >
    {
        public TNameFieldRowCollection()
            : base()
        {
        }

        public override TNameFieldRowCollectionItem AddRow()
        {
            TNameFieldRowCollectionItem cr = base.AddRow();
            cr.FieldName = 'N' + Count.ToString();
            cr.Caption = cr.FieldName;
            cr.Map = cr.BaseID;
            return cr;
        }

        public TNameFieldRowCollectionItem FindKey(int Bib)
        {
            for (int i = 0; i < Count; i++)
            {
                TNameFieldRowCollectionItem o = this[i];
                if (o != null && o.BaseID == Bib)
                {
                    return o;
                }
            }
            return null;
        }

        public string GetFieldCaptions()
        {
            TStringList SL = new TStringList();
            TNameFieldRowCollectionItem cr;
            for (int i = 0; i < Count; i++)
            {
                cr = this[i];
                SL.Add(cr.Caption);
            }
            return "SNR," + SL.CommaText;
        }

    }

}
