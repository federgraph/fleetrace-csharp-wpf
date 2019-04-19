namespace RiggVar.FR
{
    public class TColCaptions
    {
        public static TColCaptionBag ColCaptionBag = new TColCaptionBag();

        public static void InitDefaultColCaptions()
        {
            ColCaptionBag.SetCaption("E_col_SNR", "SNR");
            ColCaptionBag.SetCaption("E_col_Bib", "Bib");

            ColCaptionBag.SetCaption("E_col_GPoints", "Total");
            ColCaptionBag.SetCaption("E_col_GRank", "Rank");
            ColCaptionBag.SetCaption("E_col_GPosR", "PosR");
            ColCaptionBag.SetCaption("E_col_Cup", "RLP");
            
            ColCaptionBag.SetCaption("RaceGrid_col_QU", "QU");
            ColCaptionBag.SetCaption("RaceGrid_col_DG", "DG");
            ColCaptionBag.SetCaption("RaceGrid_col_MRank", "MRank");
            ColCaptionBag.SetCaption("RaceGrid_col_ORank", "ORank");
            ColCaptionBag.SetCaption("RaceGrid_col_Rank", "Rank");
            ColCaptionBag.SetCaption("RaceGrid_col_PosR", "PosR");

            // set the persistent flag back to false,
            // do not save default values if these are the only overrides present
            ColCaptionBag.IsPersistent = false;
        }
    }
}
