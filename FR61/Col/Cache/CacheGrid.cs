namespace RiggVar.FR
{

    public class TCacheGrid
    {
        private TStringList SL;

        public TCacheBO ColBO;
        public TCacheNode Node;
        public TCacheColGrid webGrid;

        public TCacheGrid()
        {
            SL = new TDBStringList();
            InitNode();
        }

        protected TCacheNode GetBaseNode()
        {
            return Node;
        }

        public void InitNode()
        {
            ColBO = new TCacheBO();
            Node = new TCacheNode
            {
                ColBO = ColBO,
                NameID = "CacheGrid"
            };
            ColBO.CurrentNode = Node;
        }

        public void DestroyNode()
        {
            ColBO = null;
            Node = null;
        }

        internal TCacheColGrid WebGrid
        {
            get
            {
                if (webGrid == null)
                {
                    CreateWebGrid();
                }

                return webGrid;
            }
        }

        private void CreateWebGrid()
        {
            webGrid = new TCacheColGrid
            {
                Grid = new TSimpleHashGrid(),
                ColorSchema = TColGridColorSchema.colorMoneyGreen,
                OnGetBaseNode = new TCacheColGrid.TGetBaseNodeFunction(GetBaseNode)
            };
            webGrid.SetColBOReference(ColBO);
            webGrid.ColsActive.Init();
            ColBO.InitColsActiveLayout(webGrid, 0);
            webGrid.UseHTML = true;
        }

        public string GetHTM()
        {
            WebGrid.UpdateAll();
            return webGrid.ToString();
        }

        public void SaveHTM(string FileName)
        {
            SL.Text = GetHTM();
            SL.SaveToFile(FileName);
            SL.Clear();
        }

    }

}
