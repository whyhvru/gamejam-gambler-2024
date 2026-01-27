using System;

namespace Module.Core
{
    public interface ITimer
    {
        void Schedule(float delaySeconds, Action action);
    }
}
