namespace RiggVar.FR
{
    public class TNameValueRowCollectionItem : TBaseRowCollectionItem<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
        >
    {
        public TNameValueRowCollectionItem()
            : base()
        {
        }

        public override void Assign(TNameValueRowCollectionItem source)
        {
            if (source != null)
            {
                TNameValueRowCollectionItem o = source;
                FieldName = o.FieldName;
                FieldValue = o.FieldValue;
                FieldType = o.FieldType;
                Description = o.Description;
                Category = o.Category;
                Caption = o.Caption;
            }
        }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public NameValueFieldType FieldType { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public string FieldTypeString => TNameValueColProp.FieldTypeStrings(FieldType);

    }

}
