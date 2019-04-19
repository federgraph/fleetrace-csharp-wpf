namespace RiggVar.FR
{

    public class BOTest
    {
        public static string BOData;
        public static TBOComputer BOComputer;
        public static string BOResult;

        static BOTest()
        {
            BOData = new DefaultData().FR();
            BOComputer = new TBOComputer();
            //BOResult = BOContainer.CalcStatefull(BOData);
        }
        public BOTest()
        {
        }
    }

    public class TBOComputer : IBOComputer
    {
        public static TExcelImporter ExcelImporter = new TExcelImporter();
        private TStrings SL = new TStringList();
        private TBO BO;
        public TBO StateFullBO;

        public TBOComputer()
        {
            InitGlobals();
        }
        private void InitGlobals()
        {
            TMain.MsgFactory = new TBOMsgFactory();
        }
        public string Test(string EventData, bool IsWebService)
        {            
            if (IsWebService)
            {
                SL.Text = Utils.SwapLineFeed(EventData);
            }
            else
            {
                SL.Text = EventData;
            }

            for (int i = 0; i < SL.Count; i++)
            {
                SL[i] = i.ToString() + ": " + SL[i];
            }

            return SL.Text;
        }
        public string CalcStatefull(string EventData, bool IsWebService)
        {
            return Calc(EventData, false, IsWebService);
        }
        public string CalcStateless(string EventData, bool IsWebService)
        {
            InitGlobals();
            return Calc(EventData, true, IsWebService);
        }
        public string Calc(string EventData, bool IsStateless, bool IsWebService)
        {            
            string t;

            if (IsWebService)
            {
                t = Utils.SwapLineFeed(EventData);
            }
            else
            {
                t = EventData;
            }

            ExcelImporter.RunImportFilter(t, SL);

            if (IsStateless)
            {
                if (SL.Count > 2)
                {
                    TMain.BOManager.CreateNew(SL);
                    BO = TMain.BOManager.BO;
                    SL.Text = BO.InputServer.Server.Connect("Calc.Input.Transient").HandleMsg(SL.Text);
                }
                else
                {
                    SL.Text = ""; //--> return empty string
                }
            }
            else
            {
                if (StateFullBO == null)
                {
                    TMain.BOManager.CreateNew(SL);
                    BO = TMain.BOManager.BO;
                    BO.Load(SL.Text); //load data, but ignore the params in the file
                    StateFullBO = BO; //cache this instace of TBO for later use
                }
                else
                {
                    BO = StateFullBO;
                    BO.Load(SL.Text);
                }
                string s = SL[0];
                SL.Text = BO.InputServer.Server.Connect("BOComputer.Calc.Input.Transient").HandleMsg(s);
            }

            return SL.Text;
        }

    }
}
