namespace RiggVar.FR
{
    public class TNameFieldColProp : TBaseColProp<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
        >
    {

        public TNameFieldColProp()
            : base()
        {
        }

        public override void InitColsAvail()
        {
            TNameFieldColProps ColsAvail = Collection;
            TNameFieldColProp cp;

            cp = ColsAvail.AddRow();
            cp.NameID = "col_FieldName";
            cp.Caption = "Name";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;

            cp = ColsAvail.AddRow();
            cp.NameID = "col_Caption";
            cp.Caption = "Caption";
            cp.Width = 100;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;

            cp = ColsAvail.AddRow();
            cp.NameID = "col_Swap";
            cp.Caption = "Swap";
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;

            cp = ColsAvail.AddRow();
            cp.NameID = "col_Map";
            cp.Caption = "JS FieldMap";
            cp.Width = 100;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;

        }

        protected override void GetTextDefault(TNameFieldRowCollectionItem cr, ref string Value)
        {
            Value = "";

            base.GetTextDefault(cr, ref Value);

            if (NameID == "Col_FieldName")
            {
                Value = cr.FieldName;
            }
            else if (NameID == "col_Caption")
            {
                Value = cr.Caption;
            }
            else if (NameID == "col_Swap")
            {
                Value = cr.Swap.ToString();
            }
            else if (NameID == "col_Map")
            {
                Value = NameFieldMap.Instance().FieldName(cr.Map);
            }
        }

    }

}
