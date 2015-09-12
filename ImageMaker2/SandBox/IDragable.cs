using System;

namespace SandBox
{
    public interface IDragable
    {
        Type DataType { get; }

        void Update(double x, double y);
    }
}
