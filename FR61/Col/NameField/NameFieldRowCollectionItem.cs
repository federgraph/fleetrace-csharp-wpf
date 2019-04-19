namespace RiggVar.FR
{
    public class TNameFieldRowCollectionItem : TBaseRowCollectionItem<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
        >
    {
        public TNameFieldRowCollectionItem()
            : base()
        {
        }

        public override void Assign(TNameFieldRowCollectionItem e)
        {
            FieldName = e.FieldName;
            Caption = e.Caption;
            Map = e.Map;
            Swap = e.Swap;
        }

        public string FieldName { get; set; }

        public string Caption { get; set; }

        public int Swap { get; set; }

        public int Map { get; set; }

    }

}
