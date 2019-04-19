namespace RiggVar.FR
{
    public class TOutput00
    {

        protected TBO BO;
        protected TStrings SL;

        public TOutput00()
        {
            BO = TMain.BO;
            SL = TMain.BO.Output.SL;
        }

        public bool WantPageHeader
        {
            get => BO.Output.WantPageHeader;
            set => BO.Output.WantPageHeader = value;
        }

        public TOutputType OutputType
        {
            get => BO.Output.OutputType;
            set => BO.Output.OutputType = value;
        }

        public bool XMLSection
        {
            get => BO.Output.XMLSection;
            set => BO.Output.XMLSection = value;
        }

    }
}
