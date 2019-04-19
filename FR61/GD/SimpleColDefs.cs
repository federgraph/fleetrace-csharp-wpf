namespace RiggVar.FR
{

    public class TSimpleRaceGrid : TSimpleCellGrid<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        public TSimpleRaceGrid()
            : base()
        {
        }
    }

    public class TSimpleEventGrid : TSimpleCellGrid<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
            >
    {
        public TSimpleEventGrid()
            : base()
        {
        }
    }

    public class TSimpleCacheGrid : TSimpleCellGrid<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
        >
    {
    }

    public class TSimpleNameValueGrid : TSimpleCellGrid<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {
        public TSimpleNameValueGrid()
            : base()
        {
        }
    }

}
