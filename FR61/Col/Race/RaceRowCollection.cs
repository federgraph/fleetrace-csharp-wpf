namespace RiggVar.FR
{
    public class TRaceRowCollection : TBaseRowCollection<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        public TRaceRowCollection()
            : base()
        {
        }

        public void UpdateItem(TRaceEntry e)
        {
            TRaceRowCollectionItem o = FindKey(e.SNR);
            if (o == null)
            {
                o = AddRow();
            }

            if (o != null)
            {
                o.Assign(e);
            }
        }

        public TRaceRowCollectionItem FindKey(int SNR)
        {
            for (int i = 0; i < Count; i++)
            {
                TRaceRowCollectionItem o = this[i];
                if (o != null && o.SNR == SNR)
                {
                    return o;
                }
            }
            return null;
        }

    }

}
