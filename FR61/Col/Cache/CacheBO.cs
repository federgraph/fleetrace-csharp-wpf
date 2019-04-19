namespace RiggVar.FR
{
    public class TCacheBO : TBaseColBO<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
        >
    {

        public TCacheBO()
            : base()
        {
        }

        public override void InitColsActive(TCacheColGrid g)
        {
            InitColsActiveLayout(g, 0);
        }

        public override void InitColsActiveLayout(TCacheColGrid g, int aLayout)
        {
            g.ColsActive.Clear();
            g.AddColumn("col_BaseID");
            g.AddColumn("col_Report");
            g.AddColumn("col_Race");
            g.AddColumn("col_IT");
            g.AddColumn("col_Mode");
            g.AddColumn("col_Sort");
            g.AddColumn("col_Age");
            g.AddColumn("col_Updates");
            g.AddColumn("col_Hits");
            g.AddColumn("col_Millies");
            g.AddColumn("col_TimeStamp");
            g.AddColumn("col_Request");
        }

    }

}
