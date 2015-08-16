using System;

namespace ImageMaker.Common.Camera.Events
{
    public class CameraEventBase : EventArgs
    {
        public CameraEventBase(string message)
        {
            Message = message;
        }

        public CameraEventBase(string message, CameraEventType eventType)
        {
            Message = message;
            EventType = eventType;
        }

        public string Message { get; private set; }

        public CameraEventType EventType { get; private set; }
    }
}
