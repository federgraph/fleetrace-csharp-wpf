namespace RiggVar.FR
{
    public class TColCaptionBag
    {
        TStringList FSL;

        public TColCaptionBag()
        {
            FSL = new TStringList();
            IsPersistent = false;
        }

        public string GetCaption(string key)
        {
            return FSL.Values(key);
        }
        public void SetCaption(string key, string value)
        {
            FSL.Values(key, value);
            IsPersistent = true;
        }

        public string Text
        {
            get => FSL.Text;
            set => FSL.Text = value;
        }

        public int Count
        {
            get { return FSL.Count; }
        }

        public bool IsPersistent { get; set; }

    }
}
