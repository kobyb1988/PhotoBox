using System;
using ImageMaker.SDKData.Enums;

namespace EDSDKLib.Managers
{
    public class ReturnValueManager
    {
        private readonly Action<ReturnValue> _onError;

        #region Methods

        public ReturnValueManager(Action<ReturnValue> onError)
        {
            _onError = onError;
        }

        /// <summary>
        /// Handles the function return value.
        /// May throw Exception if the result is not Ok
        /// </summary>
        /// <param name="functionReturnValue">EDSDK function return value.</param>
        /// <remarks></remarks>
        public void HandleFunctionReturnValue(UInt32 functionReturnValue)
        {
            // Check if the functionReturnValue exists in the ReturnValue Enum
            // Fastest way to Do this is to cast the numeric value into enum 
            // and asking for the enum Name. if the result is null - > this value is not defined in the enum
            ReturnValue returnValue = (ReturnValue)functionReturnValue;
            String name = Enum.GetName(typeof(ReturnValue), returnValue);

            if (name == null)
            {
                returnValue = ReturnValue.UnknownReturnValue;
            }

            HandleFunctionReturnValue(returnValue);
        }

        /// <summary>
        /// Handles the function return value.
        /// May throw Exception if the result is not Ok
        /// </summary>
        /// <param name="functionReturnValue">EDSDK function return value.</param>
        /// <exception cref="EDSDKException">if the functionReturnValue is not "Ok".</exception>
        /// <remarks></remarks>
        public void HandleFunctionReturnValue(ReturnValue functionReturnValue)
        {
            // This is a good return value
            if (functionReturnValue == ReturnValue.Ok || functionReturnValue == ReturnValue.DeviceBusy)
            {
                // Return and proceed
                return;
            }

            _onError(functionReturnValue);
            // If we end up with this section, the return value represents one of the Failure Return values.
            //throw new EDSDKException(functionReturnValue); //todo maybe later
        }

        #endregion
    }
}
