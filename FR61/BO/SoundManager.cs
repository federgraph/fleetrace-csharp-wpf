namespace RiggVar.FR
{
    public interface SoundIntf
    {
        void PlaySound(SoundID soundID);
    }

    public class SoundMock : SoundIntf
    {
        public virtual void PlaySound(SoundID soundID)
        { 
        }
    }

    public enum SoundID
    { 
        Welcome,
        Click01,
        Click02,
        Click03,
        Click04,
        Recycle
    }

    public class TSoundManager
    {
        public SoundIntf SoundImpl = new SoundMock();

        public void PlaySound(SoundID soundID)
        {
            if (Enabled)
            {
                SoundImpl.PlaySound(soundID);
            }
        }

        public bool Enabled { get; set; }
    }

}
