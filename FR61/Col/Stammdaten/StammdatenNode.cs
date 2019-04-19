namespace RiggVar.FR
{
    public class TStammdatenNode : TBaseNode<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
    >
    {

        public TStammdatenNode()
            : base()
        {
        }

        public void Load()
        {
            TStammdatenRowCollectionItem o;
            Collection.Clear();
            o = Collection.AddRow();
            o.SNR = 1001;
            o.FN = "FN";
            o.LN = "LN";
            o.SN = "SN";
            o.NC = "GER";
            o.BaseID = 1;
        }

        public void Init(int RowCount)
        {
            TStammdatenRowCollectionItem o;
            Collection.Clear();

            for (int i = 0; i < RowCount; i++)
            {
                o = Collection.AddRow();
                o.BaseID = i + 1;
                o.SNR = 1000 + i + 1;
            }
        }

    }

}
