using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RiggVar.FR
{
    public class ViewModelEventParams : INotifyPropertyChanged
    {
        public int CurrentRace { get; set; } = 1;
        public int CurrentTP { get; set; } = 0;
        public int CurrentBib { get; set; } = 0;

        public string BibCaption => "B" + CurrentBib;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyRaceChanged()
        {
            NotifyPropertyChanged("CurrentRace");
        }

        public void NotifyTimePointChanged()
        {
            NotifyPropertyChanged("CurrentTimePoint");
        }

        public void NotifyBibChanged()
        {
            NotifyPropertyChanged("CurrentBib");
        }

        public void UpdateBib(int bib)
        {
            CurrentBib = bib;
            NotifyPropertyChanged("BibCaption");
        }

    }

}
