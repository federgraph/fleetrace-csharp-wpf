namespace RiggVar.FR
{

    public enum BackupFormat
    {
        Default,
        MsgList,
        Compact
    }

    public enum TColType
    {
        colTypeInteger,
        colTypeString,
        colTypeRank
    }

    public enum TColAlignment
    {
        taLeftJustify, 
        taRightJustify, 
        taCenter
    }

    public enum TColGridColorClass
    {
        Blank,
        DefaultColor,
        AlternatingColor,
        FocusColor,
        EditableColor,
        AlternatingEditableColor,
        CurrentColor,
        TransColor,
        HeaderColor,
        CustomColor
    }

    public enum TColGridColorSchema
    {
        color256,
        colorRed,
        colorBlue,
        colorMoneyGreen
    }

}
