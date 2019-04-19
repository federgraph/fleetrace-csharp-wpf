namespace RiggVar.FR
{
    public class Publisher
    {
        private TStrings SL = new TDBStringList();

        public void PublishAll()
        {
            string s;
            string fn;
            string pp;
            string en;

            en = TMain.DocManager.EventName;
            pp = TMain.FolderInfo.PublishPath + en + '_';

            s = TMain.BO.Output.GetMsg("FR.*.Output.Report.FinishReport");
            SL.Text = s;
            fn = pp + "FinishReport.htm";
            SL.SaveToFile(fn);

            s = TMain.BO.Output.GetMsg("FR.*.Output.Report.PointsReport");
            SL.Text = s;
            fn = pp + "PointsReport.htm";
            SL.SaveToFile(fn);

            //  s = Main.BO.Output.GetMsg("FR.*.Output.Report.TimePointReport");
            //  SL.Text = s;
            //  fn = pp + "TimePointReport.xml";
            //  SL.SaveToFile(fn);

            TMain.BO.RaceDataXML.GetXML(SL);
            fn = pp + "RaceDataReport.xml";
            SL.SaveToFile(fn);

            SL.Clear();
        }

    }
}
