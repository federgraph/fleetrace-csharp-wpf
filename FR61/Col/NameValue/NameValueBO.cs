namespace RiggVar.FR
{
    public class TNameValueBO : TBaseColBO<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {

        public TNameValueBO()
            : base()
        {
        }

        public override void InitColsActive(TNameValueColGrid g)
        {
            InitColsActiveLayout(g, 0);
        }

        public override void InitColsActiveLayout(TNameValueColGrid g, int aLayout)
        {
            TNameValueColProp cp;

            g.ColsActive.Clear();
            g.AddColumn("col_BaseID");

            //g.AddColumn("col_FieldName");
            //g.AddColumn("col_FieldType");
            g.AddColumn("col_FieldTypeString");
            g.AddColumn("col_Caption");

            cp = g.AddColumn("col_FieldValue");
            cp.OnFinishEdit = new TNameValueColGrid.TBaseGetTextEvent(EditValue);
            cp.ReadOnly = false;

            g.AddColumn("col_Description");
            g.AddColumn("col_Category");
        }

        public void EditValue(TNameValueRowCollectionItem cr, ref string Value)
        {
            switch (cr.FieldType)
            {
                case NameValueFieldType.FTInteger: Value = CheckInteger(cr.FieldValue, Value); break;
                case NameValueFieldType.FTBoolean: Value = CheckBoolean(Value); break;
                case NameValueFieldType.FTString: Value = CheckString(cr.FieldValue, Value); break;
                default:
                    break;
            }
            cr.FieldValue = Value;
        }

        private string CheckInteger(string OldValue, string Value)
        {
            int i;
            i = Utils.StrToIntDef(OldValue, 0);
            i = Utils.StrToIntDef(Value, i);
            return i.ToString();
        }

        private string CheckBoolean(string Value)
        {
            bool b = Utils.IsTrue(Value);
            return Utils.BoolStr[b];
        }

        private string CheckString(string OldValue, string Value)
        {
            return Value.Length < 20 ? Value : OldValue;
        }

    }

}
