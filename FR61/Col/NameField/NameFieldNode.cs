namespace RiggVar.FR
{
    public class TNameFieldNode : TBaseNode<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
        >
    {

        public TNameFieldNode()
            : base()
        {
        }

        public void Init(TStammdatenNode sdn)
        {
            Collection.Clear();
            TStammdatenRowCollection cl = sdn.Collection;
            TNameFieldRowCollectionItem o;
            for (int i = 1; i <= cl.FieldCount; i++)
            {
                o = Collection.AddRow();
                //o.FieldName = "N" + i.ToString(); //automatic see Add
                o.Caption = cl.GetFieldCaption(i);
                //o.Map = i; //automatic see Add
            }
        }

    }

}
