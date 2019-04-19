namespace RiggVar.FR
{
    public class TNameValueColProp : TBaseColProp<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {

        public const int NumID_FieldName = 1;
        public const int NumID_FieldValue = 2;
        public const int NumID_FieldType = 3;
        public const int NumID_Caption = 4;
        public const int NumID_Description = 5;
        public const int NumID_Category = 6;
        public const int NumID_FieldTypeString = 7;

        public TNameValueColProp()
            : base()
        {
        }

        public override void InitColsAvail()
        {
            TNameValueColProps ColsAvail = Collection;

            TNameValueColProp cp;

            //FieldName
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FieldName";
            cp.Caption = "Name";
            cp.Width = 100;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.NumID = NumID_FieldName;

            //FieldValue
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FieldValue";
            cp.Caption = "Value";
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.NumID = NumID_FieldValue;

            //FieldType
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FieldType";
            cp.Caption = "Type";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.NumID = NumID_FieldType;

            //Caption
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Caption";
            cp.Caption = "Caption";
            cp.Width = 120;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.NumID = NumID_Caption;

            //Description
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Description";
            cp.Caption = "Description";
            cp.Width = 260;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.NumID = NumID_Description;

            //Category
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Category";
            cp.Caption = "Category";
            cp.Width = 60;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.Descending = true;
            cp.NumID = NumID_Category;

            //FieldTypeString
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FieldTypeString";
            cp.Caption = "Type";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.NumID = NumID_FieldTypeString;

        }
        protected override void GetTextDefault(TNameValueRowCollectionItem cr, ref string Value)
        {
            Value = "";

            base.GetTextDefault(cr, ref Value);

            if (NumID == NumID_FieldName)
            {
                Value = cr.FieldName;
            }
            else if (NumID == NumID_FieldValue)
            {
                Value = cr.FieldValue;
            }

            //this column is only shown correctly if used with an editable DataSet
            else if (NumID == NumID_FieldType)
            {
                Value = FieldTypeStrings(cr.FieldType);
            }

            //this column can be shown correctly when using a readonly SortedView
            else if (NumID == NumID_FieldTypeString)
            {
                Value = cr.FieldTypeString;
            }
            else if (NumID == NumID_Caption)
            {
                Value = cr.Caption;
            }
            else if (NumID == NumID_Description)
            {
                Value = cr.Description;
            }
            else if (NumID == NumID_Category)
            {
                Value = cr.Category;
            }
        }

        public static string FieldTypeStrings(NameValueFieldType o)
        {
            switch (o)
            {
                case NameValueFieldType.FTBoolean: return "bool";
                case NameValueFieldType.FTInteger: return "int";
                case NameValueFieldType.FTString: return "string";
                default:
                    break;
            }
            return "";
        }

    }

}
