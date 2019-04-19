namespace RiggVar.FR
{
    public interface ICacheMotor
    {
        int IdleDelay
        {
            get;
            set;
        }
        bool Active
        {
            get;
            set;
        }
    }
}
