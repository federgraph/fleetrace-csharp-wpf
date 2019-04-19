namespace RiggVar.FR
{
    public class TCacheColProp : TBaseColProp<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
        >
    {

        public const int NumID_Report = 1;
        public const int NumID_Race = 2;
        public const int NumID_IT = 3;
        public const int NumID_Sort = 4;
        public const int NumID_Mode = 5;
        public const int NumID_Age = 6;
        public const int NumID_Updates = 7;
        public const int NumID_Hits = 8;
        public const int NumID_Millies = 9;
        public const int NumID_Request = 10;
        public const int NumID_TimeStamp = 11;

        public TCacheColProp()
            : base()
        {
        }

        public override void InitColsAvail()
        {
            TCacheColProps ColsAvail = Collection;

            TCacheColProp cp;

            //Report
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Report";
            cp.Caption = "Report";
            cp.Width = 50;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Report;

            //Race
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Race";
            cp.Caption = "Race";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Race;

            //IT
            cp = ColsAvail.AddRow();
            cp.NameID = "col_IT";
            cp.Caption = "IT";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_IT;

            //Sort
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Sort";
            cp.Caption = "Sort";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Sort;

            //Mode
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Mode";
            cp.Caption = "Mode";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Mode;

            //Age
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Age";
            cp.Caption = "Age";
            cp.Width = 30;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Age;

            //Updates
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Updates";
            cp.Caption = "Updates";
            cp.Width = 70;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Updates;

            //Hits
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Hits";
            cp.Caption = "Hits";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.Descending = true;
            cp.NumID = TCacheColProp.NumID_Hits;

            //Millies
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Millies";
            cp.Caption = "Millies";
            cp.Width = 40;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.Descending = true;
            cp.NumID = TCacheColProp.NumID_Millies;

            //TimeStamp
            cp = ColsAvail.AddRow();
            cp.NameID = "col_TimeStamp";
            cp.Caption = "TimeStamp";
            cp.Width = 120;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_TimeStamp;

            //Request
            cp = ColsAvail.AddRow();
            cp.NameID = "col_Request";
            cp.Caption = "Request";
            cp.Width = 300;
            cp.Sortable = true;
            cp.Alignment = TColAlignment.taRightJustify;
            cp.NumID = TCacheColProp.NumID_Request;

        }
        protected override void GetTextDefault(TCacheRowCollectionItem cr, ref string Value)
        {
            Value = "";

            base.GetTextDefault(cr, ref Value);

            if (NumID == NumID_Report)
            {
                Value = cr.Report.ToString();
            }
            else if (NumID == NumID_Race)
            {
                Value = cr.Race.ToString();
            }
            else if (NumID == NumID_IT)
            {
                Value = cr.IT.ToString();
            }
            else if (NumID == NumID_Sort)
            {
                Value = cr.Sort.ToString();
            }
            else if (NumID == NumID_Mode)
            {
                Value = cr.Mode.ToString();
            }
            else if (NumID == NumID_Age)
            {
                Value = cr.Age.ToString();
            }
            else if (NumID == NumID_Updates)
            {
                Value = cr.Updates.ToString();
            }
            else if (NumID == NumID_Hits)
            {
                Value = cr.Hits.ToString();
            }
            else if (NumID == NumID_Millies)
            {
                Value = cr.Millies.ToString();
            }
            else if (NumID == NumID_Request)
            {
                Value = cr.Request;
            }
            else if (NumID == NumID_IT)
            {
                Value = cr.TimeStamp.ToString();
            }
        }

    }

}
