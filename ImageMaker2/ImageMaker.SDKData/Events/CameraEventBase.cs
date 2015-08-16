using System;

namespace ImageMaker.SDKData.Events
{
    public abstract class CameraEventBase : EventArgs
    {
        public abstract string Message { get; }

        public abstract CameraEventType EventType { get; }
    }
}
