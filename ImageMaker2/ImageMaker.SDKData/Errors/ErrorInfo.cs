using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.SDKData.Enums;

namespace ImageMaker.SDKData.Errors
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
