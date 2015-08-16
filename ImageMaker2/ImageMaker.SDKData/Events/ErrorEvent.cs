using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.Common.Extensions;
using ImageMaker.SDKData.Enums;

namespace ImageMaker.SDKData.Events
{
    public class ErrorEvent : CameraEventBase
    {
        public ErrorEvent(ReturnValue returnValue)
        {
            ErrorCode = returnValue;
        }

        public ReturnValue ErrorCode { get; private set; }
        public override string Message
        {
            get { return string.Format("При работе камеры возникла ошибка: {0}", ErrorCode.GetDescription()); }
        }

        public override CameraEventType EventType
        {
            get { return CameraEventType.Error; }
        }
    }
}
