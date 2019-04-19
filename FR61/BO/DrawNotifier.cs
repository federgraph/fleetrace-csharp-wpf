using System;

namespace RiggVar.FR
{
    public class DrawNotifierEventArgs : EventArgs
    {
        public const int DrawTargetEvent = 1;
        public const int DrawTargetRace = 2;
        private int FDrawTarget = 0;

        public DrawNotifierEventArgs(int aDrawTarget)
        {
            FDrawTarget = aDrawTarget;
        }

        public int DrawTarget
        {
            get { return FDrawTarget; }
        }
    }

    public interface IDrawNotifier
    {
        void ScheduleDraw(object sender, DrawNotifierEventArgs e);
        void ScheduleFullUpdate(object sender, DrawNotifierEventArgs e);
    }

    public class TDrawNotifier : IDrawNotifier
    {
        public TDrawNotifier()
        {
        }
        public void ScheduleDraw(object sender, DrawNotifierEventArgs e)
        {
        }
        public void ScheduleFullUpdate(object sender, DrawNotifierEventArgs e)
        {
        }
    }

}
