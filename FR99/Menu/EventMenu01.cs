
namespace FR62.Tabs
{

    public class EventMenu01 : IEventMenu
    {
        private readonly string ImageRoot1 = @"http://gsmac/CubeImages/Images02/";
        private readonly string ImageRoot2 = @"http://gsmac/CubeImages/Images03/";
        private readonly string ResultRoot = @"http://gsmac/CubeImages/Results05/";

        public string ComboCaption => "Test Data";

        public int Count => 11;

        public string GetCaption(int i)
        {

            switch (i)
            {
                case 1: return "49er";
                case 2: return "470 M";
                case 3: return "470 W";
                case 4: return "Europe";
                case 5: return "Finn";
                case 6: return "Laser";
                case 7: return "Mistral M";
                case 8: return "Mistral W";
                case 9: return "Star";
                case 10: return "Tornado";
                case 11: return "Yngling";
                default: return "B" + i;
            }
        }

        public string GetImageUrl(int i)
        {
            switch (i)
            {
                case 1: return ImageRoot1 + "Seite-1.png";
                case 2: return ImageRoot1 + "Seite-2.png";
                case 3: return ImageRoot1 + "Seite-3.png";
                case 4: return ImageRoot1 + "Seite-4.png";
                case 5: return ImageRoot1 + "Seite-5.png";
                case 6: return ImageRoot1 + "Seite-6.png";

                case 7: return ImageRoot2 + "Seite-1.png";
                case 8: return ImageRoot2 + "Seite-2.png";
                case 9: return ImageRoot2 + "Seite-3.png";
                case 10: return ImageRoot2 + "Seite-4.png";
                case 11: return ImageRoot2 + "Seite-5.png";
                default: return string.Empty;
            }
        }

        public string GetDataUrl(int i)
        {
            if (i > 0 && i <= Count)
            {
                return ResultRoot + string.Format("Event-{0:00}.xml", i);
            }
            return string.Empty;
        }

        public void Load(string data)
        {
            //do nothing, this is a hardcoded menu
        }

        public bool IsMock()
        {
            return true;
        }

    }
}
