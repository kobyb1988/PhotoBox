using System;
using EDSDKLib.CommandQueue;
using EDSDKLib.Invokes;
using EDSDKLib.Managers;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;
using ImageMaker.SDKData.Structs;

namespace EDSDKLib
{
    public class Camera : IDisposable
    {
        public IntPtr Ref { get; private set; }
        public event EventHandler<ErrorEvent> ErrorEvent;

        public DeviceInformation Info { get; private set; }
        private readonly ReturnValueManager _returnValueManager;
        private readonly ActionFactory _actionFactory;

        public Camera(IntPtr reference, DeviceInformation info)
        {
            if (reference == IntPtr.Zero) throw new ArgumentNullException("Camera pointer is zero");

            //_returnValueManager = new ReturnValueManager(RaiseErrorEvent);
            //_actionFactory = new ActionFactory();

            this.Ref = reference;
            this.Info = info;
        }

        private void RaiseErrorEvent(ReturnValue errorCode)
        {
            var handler = ErrorEvent;
            if (handler != null)
                handler(this, new ErrorEvent(errorCode));
        }

        

        /// <summary>
        /// Tells the camera that there is enough space on the HDD if SaveTo is set to Host
        /// This method does not use the actual free space!
        /// </summary>
        public void SetCapacity()
        {
            //create new capacity struct
            Capacity capacity = new Capacity();

            //set big enough values
            capacity.Reset = 1;
            capacity.BytesPerSector = 0x1000;
            capacity.NumberOfFreeClusters = 0x7FFFFFFF;

            //set the values to camera
            SendSDKCommand(() =>
            {
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetCapacity(this.Ref, capacity));
                //DequeueItem();
            });
        }

        private void QueueItem(Action queueAction, PriorityValue priority)
        {
          //  _actionFactory.Enqueue(queueAction, priority);
        }

        private void DequeueItem()
        {
            _actionFactory.Dequeue();
        }

        /// <summary>
        /// Sends a command to the camera safely
        /// </summary>
        private void SendSDKCommand(Action command, PriorityValue priorityValue = PriorityValue.High)
        {
            QueueItem(() =>
            {
                command();
                DequeueItem();
            }, priorityValue);
        }

        public virtual void Dispose()
        {
            //if (this.Ref != IntPtr.Zero)
            //{
            //    UInt32 returnValue = EDSDK.EdsRelease(this.Ref);

            //    this.Ref = IntPtr.Zero;
            //}
        }
    }
}