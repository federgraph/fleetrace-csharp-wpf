namespace RiggVar.FR
{

    public enum TFinishError
    {
        error_OutOfRange_OTime_Min,
        error_OutOfRange_OTime_Max,
        error_Duplicate_OTime,
        error_Contiguous_OTime
    }

    public enum TEntryError
    {
        error_Duplicate_SNR,
        error_Duplicate_Bib,
        error_OutOfRange_Bib,
        error_OutOfRange_SNR
    }

    public enum TColorMode
    {
        ColorMode_None,
        ColorMode_Error,
        ColorMode_Fleet
    }

    public enum TColumnType
    {
        PlainTextColumn,
        BackgroundTextColumn,
        EllipseTextColumn,
        TriangleTextColumn,
        RectangleTextColumn,
        RaceColumn
    }

}
