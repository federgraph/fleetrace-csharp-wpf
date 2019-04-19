namespace RiggVar.FR
{
    public class TOutput03
    {
        private TTokenParser TokenParser;
        private TRaceBO RaceBO;
        private TRaceNode RaceNode;
        private TRaceReportGrid RaceGrid;
        private int SortColIndex;
        private int Layout;
        private string TableCaption;

        public int ReportID = 0;

        public TOutput03()
        {
            TokenParser = new TTokenParser();
            Layout = 1;

            RaceGrid = new TRaceReportGrid
            {
                Grid = new TSimpleHashGrid(),
                ColorSchema = TColGridColorSchema.colorRed,
                OnGetBaseNode = new TRaceColGrid.TGetBaseNodeFunction(RaceNodeFunction),
                UseHTML = true
            };
        }

        private TRaceNode RaceNodeFunction()
        {
            return RaceNode;
        }

        private void GetRaceHTML(TStrings SL)
        {
            int i;
            if (RaceBO == null)
            {
                return;
            }

            if (RaceNode == null)
            {
                return;
            }

            RaceGrid.SetColBOReference(RaceBO);
            RaceGrid.ColsAvail.Init();
            RaceBO.InitColsActiveLayout(RaceGrid, Layout); //Layout = beliebig

            if (SortColIndex > -1)
            {
                i = SortColIndex;
            }
            else
            {
                i = -1;
            }

            RaceGrid.ColsActive.SortColIndex = i;

            RaceGrid.AlwaysShowCurrent = true; //true=with Error-Colors and Current-Colors
            RaceGrid.UpdateAll();
            TEventProps ep = TMain.BO.EventProps;
            switch (ReportID)
            {
                case 4: RaceGrid.FRContent4(SL, ""); break;
                case 5: RaceGrid.FRContent5(SL, ""); break;
                case 8: RaceGrid.FRContent4(SL, ""); break;
                default: RaceGrid.FRContent0(SL, ""); break;
            }
        }

        /// <summary>
        /// Creates RaceColReports according to the request in PathInfo.
        /// The Report is added line by line to the StringList, which is not cleared.
        /// </summary>
        /// <param name="SL">the 'StringList'</param>
        /// <param name="PathInfo">expected to start with 'Race.'</param>
        public void GetMsg(TStrings SL, string PathInfo)
        {

            Layout = 0;
            SortColIndex = -1;
            TableCaption = PathInfo;
            TokenParser.sRest = PathInfo;

            if (PathInfo.StartsWith("Event."))
            {
                return;
            }

            if (PathInfo.StartsWith("Race."))
            {
                TokenParser.NextToken(); //Race.
                ReportID = TokenParser.NextTokenX("Report");
                int Race = TokenParser.NextTokenX("R");
                int IT = TokenParser.NextTokenX("IT");
                if (IT > -1)
                {
                    Layout = IT;
                }

                RaceNode = TMain.BO.FindNode("W" + Race.ToString());
                if (RaceNode == null)
                {
                    RaceNode = TMain.BO.RNode[0];
                }

                if (RaceNode == null)
                {
                    return;
                }

                RaceBO = RaceNode.ColBO;
                GetRaceHTML(SL);
            }

        }

    }

}
