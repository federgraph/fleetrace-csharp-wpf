namespace RiggVar.FR
{

    public class TNameFieldColProps : TBaseColProps<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
        >
    {
        public TNameFieldColProps()
            : base()
        {
        }
    }

    public class TNameFieldColGrid : TColGrid<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
        >
    {
        public TNameFieldColGrid()
            : base()
        {
        }
    }

}
