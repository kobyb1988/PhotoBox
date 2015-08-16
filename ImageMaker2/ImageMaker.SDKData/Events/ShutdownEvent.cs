namespace ImageMaker.SDKData.Events
{
    public class ShutdownEvent : CameraEventBase
    {
        public override string Message
        {
            get { return string.Format("Работа камеры была аварийно завершена"); }
        }

        public override CameraEventType EventType
        {
            get { return CameraEventType.Shutdown; }
        }
    }
}
