namespace RiggVar.FR
{
    public class TNameFieldBO : TBaseColBO<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp>
    {

        public TNameFieldBO()
            : base()
        {
        }

        public override void InitColsActive(TNameFieldColGrid g)
        {
            TNameFieldColProp cp;
            g.ColsActive.Clear();
            g.AddColumn("col_BaseID");

            g.AddColumn("col_FieldName");

            cp = g.AddColumn("col_Caption");
            cp.OnFinishEdit = new TNameFieldColGrid.TBaseGetTextEvent(EditCaption);
            cp.ReadOnly = false;

            cp = g.AddColumn("col_Swap");
            cp.OnFinishEdit = new TNameFieldColGrid.TBaseGetTextEvent(EditSwap);
            cp.ReadOnly = false;

            cp = g.AddColumn("col_Map");
            cp.OnFinishEdit = new TNameFieldColGrid.TBaseGetTextEvent(EditMap);
            cp.ReadOnly = false;
        }

        public void EditCaption(TNameFieldRowCollectionItem cr, ref string Value)
        {
            cr.Caption = Value;
        }

        public void EditSwap(TNameFieldRowCollectionItem cr, ref string Value)
        {
            int t = Utils.StrToIntDef(Value, -1);
            if (t >= 0 && t <= cr.Collection.Count)
            {
                cr.Swap = Utils.StrToIntDef(Value, cr.Swap);
            }

            Value = cr.Swap.ToString();
        }

        public void EditMap(TNameFieldRowCollectionItem cr, ref string Value)
        {
            int t = Utils.StrToIntDef(Value, -1);
            if (t == 0)
            {
                cr.Map = 0;
            }

            if (t > 0 && t <= NameFieldMap.JS_NameFieldMax)
            {
                cr.Map = t;
                NameFieldMap.Instance()[t] = cr.BaseID;
            }
            Value = NameFieldMap.Instance().FieldName(cr.Map);
        }

    }

}
