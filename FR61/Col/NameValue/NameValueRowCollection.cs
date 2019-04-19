namespace RiggVar.FR
{
    public class TNameValueRowCollection : TBaseRowCollection<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {

        public TNameValueRowCollection()
            : base()
        {
        }

        public TNameValueRowCollectionItem FindKey(int Bib)
        {
            for (int i = 0; i < Count; i++)
            {
                TNameValueRowCollectionItem o = this[i];
                if (o != null && o.BaseID == Bib)
                {
                    return o;
                }
            }
            return null;
        }

    }

}
