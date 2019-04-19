namespace RiggVar.FR
{

    public class TCacheColProps : TBaseColProps<
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
    
    public class TCacheColGrid : TColGrid<
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

}
