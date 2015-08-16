using System;

namespace EDSDKLib.CommandQueue
{
    internal class Priority : IComparable<Priority>
    {
        public Priority(PriorityValue priority)
        {
            PriorityValue = priority;
        }

        public PriorityValue PriorityValue { get; private set; }

        public int CompareTo(Priority other)
        {
            return PriorityValue.CompareTo(other.PriorityValue);
        }
    }

    internal enum PriorityValue
    {
        Normal = 0,
        High = 1,
        Critical = 2
    }
}
