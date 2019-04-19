namespace RiggVar.FR
{
    public enum NameValueFieldType 
    {
        FTInteger,
        FTString,
        FTBoolean
    }

    public class TNameValueColProps : TBaseColProps<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {
        public TNameValueColProps()
            : base()
        {
        }
    }

    public class TNameValueColGrid : TColGrid<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {
        public TNameValueColGrid()
            : base()
        {
        }
    }

}
