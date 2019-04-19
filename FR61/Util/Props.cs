namespace RiggVar.FR
{

    public struct TProp
    {
        public string Key;
        public string Value;
    }

    public class TProps : TBOPersistent
    {
        private TStrings FProps = new TStringList();

        public TProps()
        {
        }

        public override void Assign(object Source)
        {
            if (Source is TProps)
            {
                TProps o = Source as TProps;
                FProps.Assign(o.Props);
            }
            else if (Source is TStrings)
            {
                FProps.Assign((TStrings) Source);
            }
            else
            {
                base.Assign(Source);
            }
        }

        public void GetProp(int Index, ref TProp Prop)
        {
            Prop.Key = FProps.Names(Index);
            Prop.Value = FProps.ValueFromIndex(Index);
        }

        public string this [string Key] //Value
        {
            get => FProps.Values(Key);
            set => FProps.Values(Key, value);
        }

        public TStrings Props
        {
            get 
            { 
                return FProps; 
            }
        }

        public int Count
        {
            get { return FProps.Count; }
        }

    }

}
