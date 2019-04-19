/*
RCount = BO.BOParams.RaceCount + 1;

Race[0] --> series results
Race[1] ... Race[RCount-1] --> race-results

BO.RNode[0] --> series results
BO.RNode[1] ... BO.RNode[RaceCount] --> race results
*/

namespace RiggVar.FR
{

    public class TEventColProps : TBaseColProps<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
        >
    {
        public TEventColProps()
            : base()
        {
        }
    }

    public class TEventColGrid : TColGrid<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
            >
    {
        public TEventColGrid()
            : base()
        {
        }
    }

}
