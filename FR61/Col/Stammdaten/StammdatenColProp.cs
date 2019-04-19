namespace RiggVar.FR
{
    public class TStammdatenColProp : TBaseColProp<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
        >
    {

        protected const int NumID_SNR = -1;
        protected const int NumID_FN = 1;
        protected const int NumID_LN = 2;
        protected const int NumID_SN = 3;
        protected const int NumID_NC = 4;
        protected const int NumID_GR = 5;
        protected const int NumID_PB = 6;

        public TStammdatenColProp()
            : base()
        {
        }

        public int GetFieldCount()
        {
            int i;
            i = TMain.BO.AdapterParams.FieldCount;
            if (i < TStammdatenRowCollection.FixFieldCount)
            {
                i = TStammdatenRowCollection.FixFieldCount;
            }

            return i;
        }

        public string GetFieldCaptionDef(TStammdatenRowCollection cl, int index, string def)
        {
            return (cl != null) ? cl.GetFieldCaption(index) : def;
        }

        public TStammdatenRowCollection GetStammdatenRowCollection()
        {
            TStammdatenNode n = GetStammdatenNode();
            return n?.Collection;
        }

        public TStammdatenNode GetStammdatenNode()
        {
            return TMain.BO.StammdatenNode;
        }

        public override void InitColsAvail()
        {
            TStammdatenColProps ColsAvail = Collection;

            TStammdatenColProp cp;
            TStammdatenRowCollection cl = GetStammdatenRowCollection();

            //SNR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_SNR";
            cp.Caption = "SNR";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = NumID_SNR;
            cp.BindingPath = "SNR";

            //FN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FN";
            cp.Caption = GetFieldCaptionDef(cl, 1, FieldNames.FN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_FN;
            cp.BindingPath = "FN";

            //LN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_LN";
            cp.Caption = GetFieldCaptionDef(cl, 2, FieldNames.LN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_LN;
            cp.BindingPath = "LN";

            //SN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_SN";
            cp.Caption = GetFieldCaptionDef(cl, 3, FieldNames.SN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_SN;
            cp.BindingPath = "SN";

            //NC
            cp = ColsAvail.AddRow();
            cp.NameID = "col_NC";
            cp.Caption = GetFieldCaptionDef(cl, 4, FieldNames.NC);
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NC;
            cp.BindingPath = "NC";

            //GR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_GR";
            cp.Caption = GetFieldCaptionDef(cl, 5, FieldNames.GR);
            cp.Width = 55;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_GR;
            cp.BindingPath = "GR";

            //PB
            cp = ColsAvail.AddRow();
            cp.NameID = "col_PB";
            cp.Caption = GetFieldCaptionDef(cl, 6, FieldNames.PB);
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_PB;
            cp.BindingPath = "PB";

            int fieldCount = GetFieldCount();
            if (fieldCount > TStammdatenRowCollection.FixFieldCount)
            {
                for (int i = TStammdatenRowCollection.FixFieldCount + 1; i <= fieldCount; i++)
                {
                    cp = ColsAvail.AddRow();
                    cp.NameID = "col_N" + i.ToString();
                    cp.Caption = GetFieldCaptionDef(cl, i, "N" + i.ToString());
                    cp.Width = 60;
                    cp.Sortable = true;
                    cp.Alignment = TColAlignment.taLeftJustify;
                    cp.ColType = TColType.colTypeString;
                    cp.NumID = i;
                    cp.BindingPath = string.Format("[{0}]", i);
                }
            }

        }

        protected override void GetTextDefault(TStammdatenRowCollectionItem cr, ref string Value)
        {
            base.GetTextDefault(cr, ref Value);

            if (NumID == NumID_SNR)
            {
                Value = cr.SNR.ToString();
            }
            else if (NumID == NumID_FN)
            {
                Value = cr.FN;
            }
            else if (NumID == NumID_LN)
            {
                Value = cr.LN;
            }
            else if (NumID == NumID_SN)
            {
                Value = cr.SN;
            }
            else if (NumID == NumID_NC)
            {
                Value = cr.NC;
            }
            else if (NumID == NumID_GR)
            {
                Value = cr.GR;
            }
            else if (NumID == NumID_PB)
            {
                Value = cr.PB;
            }
            else if (NumID > TStammdatenRowCollection.FixFieldCount)
            {
                Value = cr[NumID];
            }

        }

    }

}
