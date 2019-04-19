namespace RiggVar.FR
{

    public class TOutput02
    {
        private TTokenParser TokenParser;
        private TEventBO ColBO;
        private TEventNode Node;
        private TEventReportGrid ColGrid;
        private int SortColIndex;
        private string Modus = "Points";
        private int Layout;
        private string TableCaption;

        public int ReportID = 0;

        public TOutput02()
        {
            TokenParser = new TTokenParser();
            Layout = 1;

            ColGrid = new TEventReportGrid
            {
                Grid = new TSimpleHashGrid(),
                ColorSchema = TColGridColorSchema.colorRed,
                OnGetBaseNode = new TEventColGrid.TGetBaseNodeFunction(EventNodeFunction),
                UseHTML = true
            };
        }

        TEventNode EventNodeFunction() => Node;

        private void GetHTML(TStrings SL)
        {
            int i;
            TEventColProp cp;
            if (ColBO == null)
            {
                return;
            }

            if (Node == null)
            {
                return;
            }

            ColGrid.SetColBOReference(ColBO);
            ColGrid.ColsAvail.Init();
            ColBO.InitColsActiveLayout(ColGrid, Layout); //Layout = beliebig

            if (ReportID == 9)
            {
                cp = ColGrid.ColsActive.ByName(TMain.BO.EventProps.SortColName);
                if (cp != null)
                {
                    i = cp.Index;
                }
                else
                {
                    i = -1;
                }

                ColGrid.ColsActive.SortColIndex = i;
                ColGrid.AlwaysShowCurrent = false;
                ColGrid.SetupGrid();
                ColGrid.InitDisplayOrder(ColGrid.ColsActive.SortColIndex);
                ColGrid.FRContent6(SL);
            }
            else
            {
                if (SortColIndex > -1)
                {
                    i = SortColIndex;
                }
                else
                {
                    i = -1;
                }
                ColGrid.ColsActive.SortColIndex = i;

                ColGrid.AlwaysShowCurrent = true; //true: with Error-Colors and Current-Colors
                ColGrid.UpdateAll();
                TEventProps ep = TMain.BO.EventProps;
                switch (ReportID)
                {
                    case 1: ColGrid.FRContent1(SL, "", Modus, ep); break;
                    case 2: ColGrid.FRContent2(SL, "", Modus, ep); break;
                    case 3: ColGrid.FRContent3(SL, "", Modus, ep); break;
                    case 4: ColGrid.FRContent4(SL, ""); break;
                    case 5: ColGrid.FRContent5(SL, ""); break;
                    case 8: ColGrid.FRContent4(SL, ""); break;
                    case 11: ColGrid.FRContent11(SL, "", Modus, ep); break;

                    default: ColGrid.FRContent0(SL, ""); break;
                }
            }
        }

        /// <summary>
        /// Creates EventColReports according to the request in PathInfo.
        /// The Report is added line by line to the StringList, which is not cleared.
        /// </summary>
        /// <param name="SL">the 'StringList'</param>
        /// <param name="PathInfo">expected to start with 'Race'</param>
        public void GetMsg(TStrings SL, string PathInfo)
        {

            Layout = 0;
            SortColIndex = -1;
            TableCaption = PathInfo;
            TokenParser.sRest = PathInfo;

            if (PathInfo.StartsWith("Event."))
            {
                TokenParser.NextToken(); //Event.
                ReportID = TokenParser.NextTokenX("Report");
                SortColIndex = TokenParser.NextTokenX("Sort");
                TokenParser.NextToken();
                Modus = TokenParser.sToken;

                TEventNode en = TMain.BO.EventNode;
                Node = en;
                if (Modus == "Finish")
                {
                    en.WebLayout = 1; //Layout_Finish
                }
                else
                {
                    en.WebLayout = 2; //--> default Layout_Points = 0 will be used used
                }

                try
                {
                    ColBO = Node.ColBO;
                    GetHTML(SL);
                }
                finally
                {
                    en.WebLayout = 0;
                }
            }

            if (PathInfo.StartsWith("Race."))
            {
                return;
            }

        }

    }

}
