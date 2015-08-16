using System;
using ImageMaker.SDKData.Enums;

namespace EDSDKLib
{
    public class ErrorInfo : EventArgs
    {
        public ErrorInfo(string error, ReturnValue errorCode)
        {
            ErrorCode = errorCode;
            Error = error;
        }

        public ReturnValue ErrorCode { get; private set; }

        public string Error { get; private set; }
    }
}
