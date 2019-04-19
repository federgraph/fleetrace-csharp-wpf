namespace RiggVar.FR
{
    public class TStammdatenEntry: TBaseEntry
    {
        public int SNR;
        public string FN;
        public string LN;
        public string SN;
        public string NC;
        public string GR;
        public string PB;
    }

    public class TStammdatenColProps : TBaseColProps<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
        >
    {
        public TStammdatenColProps()
            : base()
        {
        }
    }

    public class TStammdatenColGrid : TColGrid<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
            >
    {
        public TStammdatenColGrid()
            : base()
        {
        }
    }

}

