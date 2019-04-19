
namespace FR62.Tabs
{

    public interface IEventMenu
    {
        void Load(string data);

        string ComboCaption { get; }
        int Count { get; }

        string GetCaption(int i);
        string GetImageUrl(int i);
        string GetDataUrl(int i);
        bool IsMock();
    }

}
