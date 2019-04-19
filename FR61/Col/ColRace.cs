namespace RiggVar.FR
{

    public class TRaceColProps : TBaseColProps<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        public TRaceColProps()
            : base()
        {
        }
    }

    public class TRaceColGrid : TColGrid<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        public TRaceColGrid()
            : base()
        {
        }
    }

}
