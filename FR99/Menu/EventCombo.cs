using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FR62.Tabs
{
    public class EventCombo : IEventMenu
    {
        private bool FHasMock;
        public int index = 0;

        public bool LoadingCompleted;

        public EventCombo()
        {
            MenuCollection = new List<EventMenu02>();
        }

        public List<EventMenu02> MenuCollection { get; }

        public int ComboCount => MenuCollection.Count;

        public int MenuIndex
        {
            get => index;
            set
            {
                if (value >= 0 && value < MenuCollection.Count)
                {
                    index = value;
                }
            }
        }

        public IEventMenu CurrentMenu
        {
            get
            {
                if (MenuCollection.Count == 0)
                {
                    FHasMock = true;
                    MenuCollection.Add(new EventMenu02());
                    index = 0;
                }
                return MenuCollection[index];
            }
        }

        void Clear()
        {
            MenuCollection.Clear();
            index = 0;
        }

        string IEventMenu.ComboCaption => CurrentMenu.ComboCaption;

        int IEventMenu.Count => CurrentMenu.Count;

        string IEventMenu.GetCaption(int i)
        {
            return CurrentMenu.GetCaption(i);
        }

        string IEventMenu.GetImageUrl(int i)
        {
            return CurrentMenu.GetImageUrl(i);
        }

        string IEventMenu.GetDataUrl(int i)
        {
            return CurrentMenu.GetDataUrl(i);
        }

        void IEventMenu.Load(string data)
        {
            try
            {
                FHasMock = false;
                Clear();
                XElement xe = XElement.Parse(data);
                string root = EventMenu02.ParseRoot(xe);
                foreach (XElement cr in xe.Elements("ComboEntry"))
                {
                    EventMenu02 em = new EventMenu02();
                    em.ParseComboEntry(cr, root);
                    MenuCollection.Add(em);
                }
                LoadingCompleted = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        bool IEventMenu.IsMock()
        {
            return FHasMock;
        }

    }

}
