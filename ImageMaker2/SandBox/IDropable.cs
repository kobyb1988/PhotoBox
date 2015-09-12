using System;

namespace SandBox
{
    public interface IDropable
    {
        Type DataType { get; }

        void Drop(object data);
    }
}
