using System.Windows;
using System.Windows.Controls;

namespace RiggVar.FR
{
    public class TimingParams
    {
        public int Race { get; set; }
        public int TP { get; set; }
        public int Bib { get; set; }
    }

    public class TStammdatenGridModel : TGridModel<
    TStammdatenColGrid,
    TStammdatenBO,
    TStammdatenNode,
    TStammdatenRowCollection,
    TStammdatenRowCollectionItem,
    TStammdatenColProps,
    TStammdatenColProp
        >
    {
        public TStammdatenGridModel(Grid cc)
            : base(cc)
        {
        }
    }

    public class TRaceGridModel : TGridModel<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
            >
    {
        public TRaceGridModel(Grid cc)
            : base(cc)
        {
        }
    }

    public class TEventGridModel : TGridModel<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
            >
    {
        public TEventGridModel(Grid cc)
            : base(cc)
        {
        }
    }

    public class TCacheGridModel : TGridModel<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
            >
    {
        public TCacheGridModel(Grid cc)
            : base(cc)
        {
        }
    }

    public class TNameFieldGridModel : TGridModel<
        TNameFieldColGrid,
        TNameFieldBO,
        TNameFieldNode,
        TNameFieldRowCollection,
        TNameFieldRowCollectionItem,
        TNameFieldColProps,
        TNameFieldColProp
            >
    {
        public TNameFieldGridModel(Grid cc)
            : base(cc)
        {
        }
    }

    public class TNameValueGridModel : TGridModel<
        TNameValueColGrid,
        TNameValueBO,
        TNameValueNode,
        TNameValueRowCollection,
        TNameValueRowCollectionItem,
        TNameValueColProps,
        TNameValueColProp
            >
    {
        public TNameValueGridModel(Grid cc)
            : base(cc)
        {
        }
    }

}
