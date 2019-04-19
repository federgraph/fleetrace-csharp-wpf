using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace RiggVar.FR
{
    public class ViewModelBibItem : INotifyPropertyChanged
    {
        internal static Brush UsedBrush = new SolidColorBrush(Colors.AliceBlue);
        internal static Brush ActiveBrush = new SolidColorBrush(Colors.Orange);

        public int Tag { get; set; }
        public int Bib { get; set; }
        public int Pos { get; set; }

        public bool Used => Pos > 0;

        public Brush Fill {
            get
            {
                if (Used)
                {
                    return UsedBrush;
                }
                else
                {
                    return ActiveBrush;
                }
            }
            set
            {

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModelBibItem(int tag, int bib)
        {
            Tag = tag;
            Bib = bib;
        }

        public virtual void Clear()
        {
            Pos = 0;
        }

        public void Invalidate()
        {
            NotifyPropertyChanged("Fill");
        }

    }

    public class ViewModelBib
    {
        public ObservableCollection<ViewModelBibItem> Items = new ObservableCollection<ViewModelBibItem>();

        public ViewModelBib()
        {
            InitRowCount(8);
        }

        public void ClearRow(int row)
        {
            if (row >= 0 && row < Items.Count)
            {
                Items[row].Clear();
            }
        }

        public void InitRowCount(int value)
        {
            if (value < 0)
            {
                return;
            }

            while (Items.Count < value)
            {
                Items.Add(new ViewModelBibItem(Items.Count, Items.Count + 1));
            }

            while (Items.Count > value)
            {
                Items.Remove(Items[Items.Count - 1]);
            }
        }

        public void InvalidateAllItems()
        {
            foreach (ViewModelBibItem item in Items)
            {
                item.Invalidate();
            }
        }

        public void ClearAll()
        {
            foreach (ViewModelBibItem item in Items)
            {
                item.Clear();
            }
            InvalidateAllItems();
        }

        public ViewModelBibItem FindBibItem(int bib)
        {
            foreach (ViewModelBibItem item in Items)
            {
                if (item.Bib == bib)
                {
                    return item;
                }
            }
            return null;
        }

    }

}
