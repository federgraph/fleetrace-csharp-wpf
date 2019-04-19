namespace RiggVar.FR
{
    public class TEventColProp : TBaseColProp<
        TEventColGrid,
        TEventBO,
        TEventNode, 
        TEventRowCollection,
        TEventRowCollectionItem, 
        TEventColProps,
        TEventColProp
        >
    {

        public TEventColProp()
            : base()
        {
        }

        public void GetSortKeyRace(TEventRowCollectionItem cr, ref string Value, string ColName)
        {
            int i = Utils.StrToIntDef(Utils.Copy(ColName, 6, ColName.Length), -1);
            if (i > 0) // && (i <= RaceCount)
            {
                TEventRaceEntry ere = cr.Race[i];
                if (cr.Race[i].OTime > 0)
                {
                    Value = Utils.IntToStr(ere.OTime + ere.Fleet * 2000);
                }
                else
                {
                    Value = Utils.IntToStr(999 + cr.BaseID + ere.Fleet * 2000);
                }
            }
        }

        public void GetSortKeyPoints(TEventRowCollectionItem cr, ref string Value, string ColName)
        {
            if (cr.Ru.ShowPoints == TEventNode.Layout_Finish)
            {
                GetSortKeyRace(cr, ref Value, ColName);
            }
            else //default: if (cr.ru.ShowPoints == TEventNode.Layout_Points)
            {
                int i = Utils.StrToIntDef(Utils.Copy(ColName, 6, ColName.Length), -1);
                if (i > 0) // && (i <= RaceCount)
                {
                    if (cr.Race[i].CTime > 0)
                    {
                        Value = Utils.IntToStr(cr.Race[i].CTime);
                    }
                    else
                    {
                        Value = Utils.IntToStr(99 + cr.BaseID);
                    }
                }
            }
        }

        public void GetSortKeyGPosR(TEventRowCollectionItem cr, ref string Value, string ColName)
        {
            Value = Utils.IntToStr(cr.GPosR);
        }

        public string GetFieldCaptionDef(TStammdatenRowCollection cl, int index, string def)
        {
            return (cl != null) ? cl.GetFieldCaption(index) : def;
        }

        public override void InitColsAvail()
        {
            TEventColProps ColsAvail = Collection;

            ColsAvail.UseCustomColCaptions = true;

            TEventColProp cp;
            TStammdatenRowCollection scl = TMain.BO.StammdatenNode.Collection;

            //SNR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_SNR";
            cp.Caption = "SNR";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = NumID_SNR;
            cp.BindingPath = "SNR";
            cp.ColumnType = TColumnType.TriangleTextColumn;

            //Bib
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Bib";
            cp.Caption = "Bib";
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = NumID_Bib;
            cp.BindingPath = "Bib";
            cp.ColumnType = TColumnType.TriangleTextColumn;

            //FN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FN";
            cp.Caption = GetFieldCaptionDef(scl, 1, FieldNames.FN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NF1;
            cp.BindingPath = "FN";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //LN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_LN";
            cp.Caption = GetFieldCaptionDef(scl, 2, FieldNames.LN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NF2;
            cp.BindingPath = "LN";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //SN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_SN";
            cp.Caption = GetFieldCaptionDef(scl, 3, FieldNames.SN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NF3;
            cp.BindingPath = "SN";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //NC
            cp = ColsAvail.AddRow();
            cp.NameID = "col_NC";
            cp.Caption = GetFieldCaptionDef(scl, 4, FieldNames.NC);
            cp.Width = 70;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NF4;
            cp.BindingPath = "NC";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //GR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_GR";
            cp.Caption = GetFieldCaptionDef(scl, 5, FieldNames.GR);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NF5;
            cp.BindingPath = "GR";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //PB
            cp = ColsAvail.AddRow();
            cp.NameID = "col_PB";
            cp.Caption = GetFieldCaptionDef(scl, 6, FieldNames.PB);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NF6;
            cp.BindingPath = "PB";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //DN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_DN";
            cp.Caption = "DN";
            cp.Width = 180;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_DN;
            cp.BindingPath = "DN";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //Race[0] wird nicht angezeigt, nur Race[1]..Race[RCount-1]
            //bzw. Race[1]..Race[RaceCount]
            int rc = TMain.BO.BOParams.RaceCount;
            for (int i = 1; i <= rc; i++)
            {
                //Ri
                cp = ColsAvail.AddRow();
                cp.NameID = "col_R" + i.ToString();
                cp.Caption = "R" + i.ToString();
                cp.Width = 60;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                cp.ColType = TColType.colTypeRank;
                cp.OnGetSortKey2 = new TEventColGrid.TBaseGetTextEvent2(GetSortKeyPoints);
                cp.NumID = NumID_Race(i) + 1; //10000 + i * 1000 + 1;
                cp.BindingPath = "[" + i.ToString() + "]";
                cp.ColumnType = TColumnType.RaceColumn;
            }

            //GPoints
            cp = ColsAvail.AddRow();
            cp.NameID = "col_GPoints";
            cp.Caption = "GPoints";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.OnGetSortKey2 = new TEventColGrid.TBaseGetTextEvent2(GetSortKeyGPosR);
            cp.NumID = NumID_GPoints;
            cp.BindingPath = "GPoints";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //GRank
            cp = ColsAvail.AddRow();
            cp.NameID = "col_GRank";
            cp.Caption = "GRank";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_GRank;
            cp.BindingPath = "GRank";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //GPosR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_GPosR";
            cp.Caption = "GPosR";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_GPosR;
            cp.BindingPath = "GPosR";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //PLZ
            cp = ColsAvail.AddRow();
            cp.NameID = "col_PLZ";
            cp.Caption = "PLZ";
            cp.Width = 30;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_PLZ;
            cp.BindingPath = "PLZ";
            cp.ColumnType = TColumnType.PlainTextColumn;

            //Cup
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Cup";
            cp.Caption = "Cup";
            cp.Width = 45;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_Cup;
            cp.BindingPath = "Cup";
            cp.ColumnType = TColumnType.PlainTextColumn;
        }

        protected override void GetTextDefault(TEventRowCollectionItem cr, ref string Value)
        {
            Value = "";

            base.GetTextDefault(cr, ref Value);

            if (NumID == NumID_SNR)
            {
                Value = cr.SNR.ToString();
            }
            else if (NumID == NumID_Bib)
            {
                Value = cr.Bib.ToString();
            }
            else if (NumID == NumID_DN)
            {
                Value = cr.DN;
            }
            else if (NumID == NumID_NF4)
            {
                Value = cr.NC;
            }
            else if (NumID == NumID_GPoints)
            {
                Value = cr.GPoints;
            }
            else if (NumID == NumID_GRank)
            {
                Value = cr.GRank.ToString();
            }
            else if (NumID == NumID_GPosR)
            {
                Value = cr.GPosR.ToString();
            }
            else if (NumID == NumID_PLZ)
            {
                Value = Utils.IntToStr(cr.PLZ + 1);
            }
            else if (NumID == NumID_Cup)
            {
                Value = cr.RA.ToString("F2");
            }

            //{ Race[0] wird nicht angezeigt }
            else if (IsRaceNumID(NumID))
            {
                int i = RaceIndex(NumID);
                Value = cr.Race[i].RaceValue;
            }

            else if (NumID == NumID_NF1)
            {
                Value = cr.FN;
            }
            else if (NumID == NumID_NF2)
            {
                Value = cr.LN;
            }
            else if (NumID == NumID_NF3)
            {
                Value = cr.SN;
            }
            else if (NumID == NumID_NF5)
            {
                Value = cr.GR;
            }
            else if (NumID == NumID_NF6)
            {
                Value = cr.PB;
            }
        }

        public const int NumID_SNR = 1;
        public const int NumID_Bib = 2;
        public const int NumID_GPoints = 3;
        public const int NumID_GRank = 4;
        public const int NumID_GPosR = 5;
        public const int NumID_PLZ = 6;
        public const int NumID_Cup = 7;

        public const int NumID_DN = 10;
        public const int NumID_NF1 = 11;
        public const int NumID_NF2 = 12;
        public const int NumID_NF3 = 13;
        public const int NumID_NF4 = 14;
        public const int NumID_NF5 = 15;
        public const int NumID_NF6 = 16;

        public static int NumID_Race(int index)
        {
            return 10000 + index * 1000;
        }
        public static int RaceIndex(int numID)
        {
            return (numID - 10000) / 1000;
        }
        public static bool IsRaceNumID(int numID)
        {
            return numID > 10000;
        }

        public override bool IsGroupCol()
        {       
            if (TMain.BO.EventNode.UseFleets)
            {
                if (TMain.BO.EventNode.ColorMode == TColorMode.ColorMode_Error)
                {
                    if (IsRaceNumID(NumID))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }

}
