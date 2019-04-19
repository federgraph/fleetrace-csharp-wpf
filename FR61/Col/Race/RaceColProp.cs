namespace RiggVar.FR
{
    public class TRaceColProp : TBaseColProp<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        public TRaceColProp()
            : base()
        {
        }

        public string GetFieldCaptionDef(TStammdatenRowCollection cl, int index, string def)
        {
            return (cl != null) ? cl.GetFieldCaption(index) : def;
        }

        public override void InitColsAvail()
        {
            TRaceColProps ColsAvail = Collection;
            ColsAvail.UseCustomColCaptions = true;
            int ITCount = TMain.BO.BOParams.ITCount;
            TRaceColProp cp;
            TStammdatenRowCollection scl = TMain.BO.StammdatenNode.Collection;

            //Bib
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Bib";
            cp.Caption = "Bib";
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = NumID_Bib;
            cp.BindingPath = "Bib";

            //SNR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_SNR";
            cp.Caption = "SNR";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = NumID_SNR;
            cp.BindingPath = "SNR";

            //FN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FN";
            cp.Caption = this.GetFieldCaptionDef(scl, 1, FieldNames.FN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_FN;
            cp.BindingPath = "FN";

            //LN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_LN";
            cp.Caption = this.GetFieldCaptionDef(scl, 2, FieldNames.LN);
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_LN;
            cp.BindingPath = "LN";

            //SN
            cp = ColsAvail.AddRow();
            cp.NameID = "col_SN";
            cp.Caption = this.GetFieldCaptionDef(scl, 3, FieldNames.SN);
            cp.Width = 80; //100
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_SN;
            cp.BindingPath = "SN";

            //NC
            cp = ColsAvail.AddRow();
            cp.NameID = "col_NC";
            cp.Caption = this.GetFieldCaptionDef(scl, 4, FieldNames.NC);
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_NC;
            cp.BindingPath = "NC";

            //GR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_GR";
            cp.Caption = this.GetFieldCaptionDef(scl, 5, FieldNames.GR);
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_GR;
            cp.BindingPath = "GR";

            //PB
            cp = ColsAvail.AddRow();
            cp.NameID = "col_PB";
            cp.Caption = this.GetFieldCaptionDef(scl, 6, FieldNames.PB);
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taLeftJustify;
            cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_PB;
            cp.BindingPath = "PB";

            //QU
            cp = ColsAvail.AddRow();
            cp.NameID = "col_QU";
            cp.Caption = "QU";
            cp.Width = 30;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            //cp.ColType = TColType.colTypeString;
            cp.NumID = NumID_QU;
            cp.BindingPath = "QU";

            //DG
            cp = ColsAvail.AddRow();
            cp.NameID = "col_DG";
            cp.Caption = "DG";
            cp.Width = 30;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_DG;
            cp.BindingPath = "DG";

            //MRank
            cp = ColsAvail.AddRow();
            cp.NameID = "col_MRank";
            cp.Caption = "MRank";
            cp.Width = 45;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_MRank;
            cp.BindingPath = "MRank";

            //ST
            cp = ColsAvail.AddRow();
            cp.NameID = "col_ST";
            cp.Caption = "ST";
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            //cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_ST;
            cp.BindingPath = "ST";

            //IT
            string s;
            string sc;
            int NumIDBase;
            for (int i = 0; i <= ITCount; i++)
            {
                s = "col_IT" + i.ToString();
                if (i == 0) //channel_FT
                {
                    sc = "FT" + i.ToString();
                }
                else
                {
                    sc = "IT" + i.ToString();
                }

                NumIDBase = NumID_IT(i); //1000 + i * 100;

                //OTime
                cp = ColsAvail.AddRow();
                cp.NameID = s;
                cp.Caption = sc;
                cp.Width = 80;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 1;
                string.Format("[{0}].OTime", i);

                //Behind
                cp = ColsAvail.AddRow();
                cp.NameID = s + "B";
                cp.Caption = sc + "B";
                cp.Width = 80;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 2;
                string.Format("[{0}].B", i);

                //BFT
                cp = ColsAvail.AddRow();
                cp.NameID = s + "BFT";
                cp.Caption = sc + "BFT";
                cp.Width = 80;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 3;
                string.Format("[{0}].BFT", i);

                //BPL
                cp = ColsAvail.AddRow();
                cp.NameID = s + "BPL";
                cp.Caption = sc + "BPL";
                cp.Width = 80;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 4;
                string.Format("[{0}].BPL", i);

                //ORank
                cp = ColsAvail.AddRow();
                cp.NameID = s + "ORank";
                cp.Caption = sc + "ORank";
                cp.Width = 55;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 5;
                string.Format("[{0}].ORank", i);

                //Rank
                cp = ColsAvail.AddRow();
                cp.NameID = s + "Rank";
                cp.Caption = sc + "Rank";
                cp.Width = 55;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 6;
                string.Format("[{0}].Rank", i);

                //PosR
                cp = ColsAvail.AddRow();
                cp.NameID = s + "PosR";
                cp.Caption = sc + "PosR";
                cp.Width = 55;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 7;
                string.Format("[{0}].PosR", i);

                //PLZ
                cp = ColsAvail.AddRow();
                cp.NameID = s + "PLZ";
                cp.Caption = sc + "PLZ";
                cp.Width = 50;
                cp.Sortable = true;
                cp.Alignment = TColAlignment.taRightJustify;
                //cp.ColType = TColType.colTypeRank;
                cp.NumID = NumIDBase + 8;
                string.Format("[{0}].PLZ", i);
            }

            //OTime
            cp = ColsAvail.AddRow();
            cp.NameID = "col_FT";
            cp.Caption = "FT";
            cp.Width = 80;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            //cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_FT;
            cp.BindingPath = "FT.OTime";

            //ORank
            cp = ColsAvail.AddRow();
            cp.NameID = "col_ORank";
            cp.Caption = "ORank";
            cp.Width = 45;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_ORank;
            cp.BindingPath = "FT.ORank";

            //Rank
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Rank";
            cp.Caption = "Rank";
            cp.Width = 45;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_Rank;
            cp.BindingPath = "FT.Rank";

            //PosR
            cp = ColsAvail.AddRow();
            cp.NameID = "col_PosR";
            cp.Caption = "PosR";
            cp.Width = 35;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_PosR;
            cp.BindingPath = "FT.PosR";

            //PLZ
            cp = ColsAvail.AddRow();
            cp.NameID = "col_PLZ";
            cp.Caption = "PLZ";
            cp.Width = 30;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            //cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_PLZ;
            cp.BindingPath = "FT.PLZ";

            //Space
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Space";
            cp.Caption = "";
            cp.Width = 10;
            cp.Sortable = false; //true;
            cp.Alignment = TColAlignment.taRightJustify;
            //cp.ColType = TColType.colTypeRank;
            cp.NumID = NumID_Space;
            cp.BindingPath = "Space";

        }

        protected override void GetTextDefault(TRaceRowCollectionItem cr, ref string Value)
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
            else if (NumID == NumID_QU)
            {
                Value = cr.QU.ToString();
            }
            else if (NumID == NumID_DG)
            {
                Value = cr.DG.ToString();
            }
            else if (NumID == NumID_MRank)
            {
                Value = cr.MRank.ToString();
            }
            else if (NumID == NumID_ST)
            {
                Value = cr.ST.ToString();
            }
            else if (NumID == NumID_FT)
            {
                Value = cr.FT.OTime.ToString();
            }
            else if (NumID == NumID_ORank)
            {
                Value = cr.FT.ORank.ToString();
            }
            else if (NumID == NumID_Rank)
            {
                Value = cr.FT.Rank.ToString();
            }
            else if (NumID == NumID_PosR)
            {
                Value = cr.FT.PosR.ToString();
            }
            else if (NumID == NumID_PLZ)
            {
                Value = Utils.IntToStr(cr.FT.PLZ + 1);
            }
            else if (IsITNumID(NumID))
            {
                int i = ITIndex(NumID);
                TTimePoint TP = cr[i];
                int BaseNumID = NumID_IT(i);
                if (TP != null)
                {
                    if (NumID == BaseNumID + 1)
                    {
                        Value = TP.OTime.ToString();
                    }
                    else if (NumID == BaseNumID + 2)
                    {
                        Value = TP.Behind.ToString();
                    }
                    else if (NumID == BaseNumID + 3)
                    {
                        Value = TP.BFT.ToString();
                    }
                    else if (NumID == BaseNumID + 4)
                    {
                        Value = TP.BPL.ToString();
                    }
                    else if (NumID == BaseNumID + 5)
                    {
                        Value = TP.ORank.ToString();
                    }
                    else if (NumID == BaseNumID + 6)
                    {
                        Value = TP.Rank.ToString();
                    }
                    else if (NumID == BaseNumID + 7)
                    {
                        Value = TP.PosR.ToString();
                    }
                    else if (NumID == BaseNumID + 8)
                    {
                        Value = Utils.IntToStr(TP.PLZ + 1);
                    }
                }
            }
        }

        protected const int NumID_SNR = 1;
        protected const int NumID_Bib = 2;
        protected const int NumID_FN = 3;
        protected const int NumID_LN = 4;
        protected const int NumID_SN = 5;
        protected const int NumID_NC = 6;
        protected const int NumID_GR = 7;
        protected const int NumID_PB = 8;

        protected const int NumID_QU = 9;
        protected const int NumID_DG = 10;
        protected const int NumID_MRank = 11;
        protected const int NumID_ST = 12;
        protected const int NumID_FT = 13;

        protected const int NumID_ORank = 14;
        protected const int NumID_Rank = 15;
        protected const int NumID_PosR = 16;
        protected const int NumID_PLZ = 17;

        protected const int NumID_Space = 18;

        public static int NumID_IT(int index)
        {
            return 10000 + index * 10;
        }

        public static int ITIndex(int numID)
        {
            return (numID - 10000) / 10;
        }

        public static bool IsITNumID(int numID)
        {
            return numID > 10000;
        }

    }

}
