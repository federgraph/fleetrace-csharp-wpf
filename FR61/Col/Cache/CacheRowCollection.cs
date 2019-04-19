namespace RiggVar.FR
{
    public class TCacheRowCollection : TBaseRowCollection<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
        >
    {

        public TCacheRowCollection()
            : base()
        {
        }

        public override TCacheRowCollectionItem AddRow()
        {
            TCacheRowCollectionItem cr = base.AddRow();
            cr.TimeStamp = System.DateTime.Now;
            return cr;
        }

        public TCacheRowCollectionItem FindKey(int Bib)
        {
            for (int i = 0; i < Count; i++)
            {
                TCacheRowCollectionItem o = this[i];
                if (o != null && o.BaseID == Bib)
                {
                    return o;
                }
            }
            return null;
        }

    }

}
