using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EDSDKLib.CommandQueue;
using EDSDKLib.Invokes;
using EDSDKLib.Managers;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;
using ImageMaker.SDKData.Structs;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace EDSDKLib
{
    public class SDKHandler : IDisposable
    {
        #region Variables

        /// <summary>
        /// The used camera
        /// </summary>
        public Camera MainCamera { get; private set; }
        /// <summary>
        /// States if a session with the MainCamera is opened
        /// </summary>
        public bool CameraSessionOpen { get; private set; }
        /// <summary>
        /// States if the live view is on or not
        /// </summary>
        public bool IsLiveViewOn { get; private set; }
        /// <summary>
        /// States if camera is recording or not
        /// </summary>
        public bool IsFilming { get; private set; }
        /// <summary>
        /// Directory to where photos will be saved
        /// </summary>
        public string ImageSaveDirectory { get; set; }
        /// <summary>
        /// The focus and zoom border rectangle for live view (set after first use of live view)
        /// </summary>
        //public EdsdkInvokes.Rect Evf_ZoomRect { get; private set; }
        ///// <summary>
        ///// The focus and zoom border position of the live view (set after first use of live view)
        ///// </summary>
        //public EdsdkInvokes.Point Evf_ZoomPosition { get; private set; }
        ///// <summary>
        ///// The cropping position of the enlarged live view image (set after first use of live view)
        ///// </summary>
        //public EdsdkInvokes.Point Evf_ImagePosition { get; private set; }
        ///// <summary>
        ///// The live view coordinate system (set after first use of live view)
        ///// </summary>
        //public EdsdkInvokes.Size Evf_CoordinateSystem { get; private set; }

        /// <summary>
        /// States if the Evf_CoordinateSystem is already set
        /// </summary>
        public bool IsCoordSystemSet = false;

        ///// <summary>
        ///// Handles errors that happen with the SDK
        ///// </summary>
        //public ReturnValue Error
        //{
        //    get { return ReturnValue.Ok; }
        //    set
        //    {
        //        if (value != ReturnValue.Ok)
        //        {
        //            RaiseErrorEvent(string.Format("Произошла ошибка при работе с камерой. Код ошибки: {0}", value), value);
        //        }
        //    }
        //}

        /// <summary>
        /// States if a finished video should be downloaded from the camera
        /// </summary>
        private bool DownloadVideo;
        /// <summary>
        /// For video recording, SaveTo has to be set to Camera. This is to store the previous setting until after the filming.
        /// </summary>
        private uint PrevSaveTo;
      
        /// <summary>
        /// If true, the live view will be shut off completely. If false, live view will go back to the camera.
        /// </summary>
        private bool LVoff;

        #endregion

        #region Events

        #region SDK Events

        public event EdsdkInvokes.EdsCameraAddedHandler SDKCameraAddedEvent;
        public event EdsdkInvokes.EdsObjectEventHandler SDKObjectEvent;
        public event EdsdkInvokes.EdsProgressCallback SDKProgressCallbackEvent;
        public event EdsdkInvokes.EdsPropertyEventHandler SDKPropertyEvent;
        public event EdsdkInvokes.EdsStateEventHandler SDKStateEvent;

        #endregion

        #region Custom Events

        public event EventHandler<ErrorEvent> ErrorEvent;
 
        public delegate void CameraAddedHandler();
        public delegate void ProgressHandler(int Progress);
        public delegate void StreamUpdate(Stream img);

        /// <summary>
        /// Fires if a camera is added
        /// </summary>
        public event CameraAddedHandler CameraAdded;
        /// <summary>
        /// Fires if any process reports progress
        /// </summary>
        public event ProgressHandler ProgressChanged;
        ///// <summary>
        ///// Fires if the live view image has been updated
        ///// </summary>
        //public event StreamUpdate LiveViewUpdated;

        /// <summary>
        /// Fires if the live view image has been updated
        /// </summary>
        public event EventHandler<byte[]> LiveViewUpdated;

        /// <summary>
        /// If the camera is disconnected or shuts down, this event is fired
        /// </summary>
        public event EventHandler CameraHasShutdown;

        #endregion

        #endregion

        #region Basic SDK and Session handling

        private readonly ActionFactory _actionFactory;
        private readonly ReturnValueManager _returnValueManager;

        /// <summary>
        /// Initializes the SDK and adds events
        /// </summary>
        public SDKHandler()
        {
            _returnValueManager = new ReturnValueManager(RaiseErrorEvent);
            _actionFactory = new ActionFactory();
        }

        static SDKHandler()
        {
            AddEnvironmentPaths(new[] {AppDomain.CurrentDomain.BaseDirectory});
        }

        static void AddEnvironmentPaths(IEnumerable<string> paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };

            string newPath = string.Join(Path.PathSeparator.ToString(), path.Concat(paths));

            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        public void Terminate(bool isAsync = true)
        {
            Action<Camera> terminate = camera =>
            {
                Debug.WriteLine("queue: {0}", _actionFactory.Count);
                Debug.WriteLine("Terminatinng SDK");
                EdsdkInvokes.TerminateSDK();
                _actionFactory.Clear();
            };
            
            if (isAsync)
                QueueItem(terminate, MainCamera, PriorityValue.Critical);
            else
            {
                terminate(MainCamera);
            }

            SDKCameraAddedEvent -= SDKHandler_CameraAddedEvent;
            SDKStateEvent -= Camera_SDKStateEvent;
            SDKPropertyEvent -= Camera_SDKPropertyEvent;
            SDKProgressCallbackEvent -= Camera_SDKProgressCallbackEvent;
            SDKObjectEvent -= Camera_SDKObjectEvent;
        }

        public void Initialize()
        {
            _block = false;
            Debug.WriteLine("Initializing SDK");
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.InitializeSDK());
            //subscribe to camera added event (the C# event and the SDK event)

            SDKCameraAddedEvent += SDKHandler_CameraAddedEvent;
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetCameraAddedHandler(SDKCameraAddedEvent, IntPtr.Zero));

            //subscribe to the camera events (for the C# events)
            SDKStateEvent += Camera_SDKStateEvent;
            SDKPropertyEvent += Camera_SDKPropertyEvent;
            SDKProgressCallbackEvent += Camera_SDKProgressCallbackEvent;
            SDKObjectEvent += Camera_SDKObjectEvent;

            //QueueItem(cam =>
            //{
            //    _block = false;
            //    Debug.WriteLine("Initializing SDK");
            //    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.InitializeSDK());
            //    //subscribe to camera added event (the C# event and the SDK event)

            //    SDKCameraAddedEvent += SDKHandler_CameraAddedEvent;
            //    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetCameraAddedHandler(SDKCameraAddedEvent, IntPtr.Zero));

            //    //subscribe to the camera events (for the C# events)
            //    SDKStateEvent += Camera_SDKStateEvent;
            //    SDKPropertyEvent += Camera_SDKPropertyEvent;
            //    SDKProgressCallbackEvent += Camera_SDKProgressCallbackEvent;
            //    SDKObjectEvent += Camera_SDKObjectEvent;
                
            //    DequeueItem();
            //}, null, PriorityValue.Critical);
        }

        [HandleProcessCorruptedStateExceptions]
        public byte[] GetLiveViewImage(Camera camera)
        {
            try
            {

                IntPtr streamPointer = IntPtr.Zero;
                IntPtr imagePointer = IntPtr.Zero;

                UInt32 returnValue;
                if (!WrapCommand(() =>
                {
                    returnValue = EdsdkInvokes.CreateMemoryStream(0, out streamPointer);
                })) return null;

                //ReturnValueManager.HandleFunctionReturnValue(returnValue);

                try
                {
                    if (!WrapCommand(() =>
                    {
                        returnValue = EdsdkInvokes.CreateEvfImageRef(streamPointer, out imagePointer);
                    }, "creating image")) return null;

                    //ReturnValueManager.HandleFunctionReturnValue(returnValue);
                    try
                    {
                        if (!WrapCommand(() =>
                        {
                            returnValue = EdsdkInvokes.DownloadEvfImage(camera.Ref, imagePointer);
                        }, "downloading image")) return null;

                        //ReturnValueManager.HandleFunctionReturnValue(returnValue);

                        IntPtr imageBlob = new IntPtr();
                        if (!WrapCommand(() =>
                        {
                            returnValue = EdsdkInvokes.GetPointer(streamPointer, out imageBlob);
                        })) return null;


                        //ReturnValueManager.HandleFunctionReturnValue(returnValue);

                        try
                        {
                            uint imageBlobLength = 0;
                            if (!WrapCommand(() =>
                            {
                                returnValue = EdsdkInvokes.GetLength(streamPointer, out imageBlobLength);
                            })) return null;

                            byte[] buffer = new byte[imageBlobLength];
                            Marshal.Copy(imageBlob, buffer, 0, (int) imageBlobLength);

                            return buffer;
                        }
                        finally
                        {
                            EdsdkInvokes.Release(imageBlob);
                            //WrapCommand(() => EdsdkInvokes.Release(imageBlob));
                        }
                    }
                    finally
                    {
                        EdsdkInvokes.Release(imagePointer);
                        //WrapCommand(() => EdsdkInvokes.Release(imagePointer));
                    }
                }
                finally
                {
                    EdsdkInvokes.Release(streamPointer);
                    //WrapCommand(() => EdsdkInvokes.Release(streamPointer));
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("message: {0}; stacktrace: {1}", e.Message, e.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// Get a list of all connected cameras
        /// </summary>
        /// <returns>The camera list</returns>
        public List<Camera> GetCameraList()
        {
            IntPtr camlist;
            //get list of cameras
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetCameraList(out camlist));

            //get each camera from camlist
            int c;
            //get amount of connected cameras
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetChildCount(camlist, out c));
            List<Camera> outCamList = new List<Camera>();
            for (int i = 0; i < c; i++)
            {
                IntPtr cptr;
                //get pointer to camera at index i
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetChildAtIndex(camlist, i, out cptr));

                DeviceInformation dinfo;
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetDeviceInformation(cptr, out dinfo));

                outCamList.Add(new Camera(cptr, dinfo));
            }

            return outCamList;
        }

        /// <summary>
        /// Opens a session with given camera
        /// </summary>
        /// <param name="newCamera">The camera which will be used</param>
        public void OpenSession(Camera newCamera)
        {
            if (CameraSessionOpen) CloseSession();
            if (newCamera != null)
            {
                QueueItem((camera) =>
                {
                    MainCamera = camera;

                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetCameraStateEventHandler(
                        camera.Ref, (uint)StateEvent.All, SDKStateEvent, IntPtr.Zero));
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetObjectEventHandler(camera.Ref,
                        (uint)ObjectEvent.All, SDKObjectEvent, IntPtr.Zero));
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyEventHandler(camera.Ref,
                        (uint)PropertyEvent.All, SDKPropertyEvent, IntPtr.Zero));
                    //open a session
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.OpenSession(camera.Ref));

                    //subscribe to the camera events (for the SDK)

                    ImageSaveDirectory =
                        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                            "RemotePhoto");

                    //Error = EDSDK.OpenSession(MainCamera.Ref);
                    CameraSessionOpen = true;
                    DequeueItem();
                }, newCamera, PriorityValue.Critical);

                SetSettingInternal((uint)PropertyId.SaveTo, (uint)SaveTo.Host, newCamera);
                SetCapacity(newCamera);
            }
        }

        private bool _dispose;

        /// <summary>
        /// Closes the session with the current camera
        /// </summary>
        public void CloseSession(bool dispose = false)
        {
            _dispose = dispose;
            if (CameraSessionOpen)
            {
                CameraSessionOpen = false;
                //if live view is still on, stop it and wait till the thread has stopped
                if (IsLiveViewOn)
                {
                    StopLiveView();
                }
                else
                {
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CloseSession(MainCamera.Ref));
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(MainCamera.Ref));
                    //QueueItem(closeSession, MainCamera, PriorityValue.Critical);

                    if (_dispose)
                        Terminate(false);

                    //QueueItem((cam) =>
                    //{
                    //    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CloseSession(cam.Ref));
                    //    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(cam.Ref));

                    //    DequeueItem();
                    //}, MainCamera, PriorityValue.Critical);

                    //if (_dispose)
                    //    Terminate();
                }
            }
            else
            {
                if (_dispose)
                    Terminate(false);
            }
        }

        /// <summary>
        /// Closes open session and terminates the SDK
        /// </summary>
        public void Dispose()
        {
            //close session
            CloseSession(true);
        }

        private bool _block;


        public void ShutDown()
        {
            _block = true;
    
            _actionFactory.Clear();

            Dispose();
        }

        #endregion

        #region Eventhandling

        /// <summary>
        /// A new camera was plugged into the computer
        /// </summary>
        /// <param name="inContext">The pointer to the added camera</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint SDKHandler_CameraAddedEvent(IntPtr inContext)
        {
            //Handle new camera here
            if (CameraAdded != null) CameraAdded();
            return (uint) ReturnValue.Ok;
        }

        private event Action<byte[]> ItemDownloaded;

        private void RaiseItemDownloaded(byte[] stream)
        {
            var handler = ItemDownloaded;
            if (handler != null)
                handler(stream);
        }

        private void RaiseErrorEvent(ReturnValue errorCode)
        {
            var handler = ErrorEvent;
            if (handler != null)
                handler(this, new ErrorEvent(errorCode));
        }

        /// <summary>
        /// An Objectevent fired
        /// </summary>
        /// <param name="inEvent">The ObjectEvent id</param>
        /// <param name="inRef">Pointer to the object</param>
        /// <param name="inContext"></param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKObjectEvent(uint inEvent, IntPtr inRef, IntPtr inContext)
        {
            //handle object event here
            switch ((ObjectEvent) inEvent)
            {
                case ObjectEvent.All:
                    break;
                case ObjectEvent.DirItemCancelTransferDT:
                    break;
                case ObjectEvent.DirItemContentChanged:
                    break;
                case ObjectEvent.DirItemCreated:
                    if (DownloadVideo) { DownloadImage(inRef, ImageSaveDirectory); DownloadVideo = false; }
                    break;
                case ObjectEvent.DirItemInfoChanged:
                    break;
                case ObjectEvent.DirItemRemoved:
                    break;
                case ObjectEvent.DirItemRequestTransfer:
                    byte[] bmp = DownloadImage(inRef);
                    //MemoryStream stream = new MemoryStream();
                    //bmp.Save(stream, bmp.RawFormat);
                    RaiseItemDownloaded(bmp);
                    //DownloadImage(inRef, ImageSaveDirectory);
                    break;
                case ObjectEvent.DirItemRequestTransferDT:
                    break;
                case ObjectEvent.FolderUpdateItems:
                    break;
                case ObjectEvent.VolumeAdded:
                    break;
                case ObjectEvent.VolumeInfoChanged:
                    break;
                case ObjectEvent.VolumeRemoved:
                    break;
                case ObjectEvent.VolumeUpdateItems:
                    break;
            }

            //DequeueItem();
            return (uint) ReturnValue.Ok;
        }

        /// <summary>
        /// A progress was made
        /// </summary>
        /// <param name="inPercent">Percent of progress</param>
        /// <param name="inContext">...</param>
        /// <param name="outCancel">Set true to cancel event</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKProgressCallbackEvent(uint inPercent, IntPtr inContext, ref bool outCancel)
        {
            //Handle progress here
            if (ProgressChanged != null) ProgressChanged((int)inPercent);
            return (uint) ReturnValue.Ok;
        }

        /// <summary>
        /// A property changed
        /// </summary>
        /// <param name="inEvent">The PropetyEvent ID</param>
        /// <param name="inPropertyID">The Property ID</param>
        /// <param name="inParameter">Event Parameter</param>
        /// <param name="inContext">...</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKPropertyEvent(uint inEvent, uint inPropertyID, uint inParameter, IntPtr inContext)
        {
            //Handle property event here
            switch ((PropertyEvent)inEvent)
            {
                case PropertyEvent.All:
                    break;
                case PropertyEvent.PropertyChanged:
                    break;
                case PropertyEvent.PropertyDescChanged:
                    break;
            }

            switch ((PropertyId) inPropertyID)
            {
                case PropertyId.AEBracket:
                    break;
                case PropertyId.AEMode:
                    break;
                case PropertyId.AFMode:
                    break;
                case PropertyId.Artist:
                    break;
                case PropertyId.AtCapture_Flag:
                    break;
                case PropertyId.Av:
                    break;
                case PropertyId.AvailableShots:
                    break;
                case PropertyId.BatteryLevel:
                    break;
                case PropertyId.BatteryQuality:
                    break;
                case PropertyId.BodyIDEx:
                    break;
                case PropertyId.Bracket:
                    break;
                case PropertyId.CFn:
                    break;
                case PropertyId.ClickWBPoint:
                    break;
                case PropertyId.ColorMatrix:
                    break;
                case PropertyId.ColorSaturation:
                    break;
                case PropertyId.ColorSpace:
                    break;
                case PropertyId.ColorTemperature:
                    break;
                case PropertyId.ColorTone:
                    break;
                case PropertyId.Contrast:
                    break;
                case PropertyId.Copyright:
                    break;
                case PropertyId.DateTime:
                    break;
                case PropertyId.DepthOfField:
                    break;
                case PropertyId.DigitalExposure:
                    break;
                case PropertyId.DriveMode:
                    break;
                case PropertyId.EFCompensation:
                    break;
                case PropertyId.LiveViewAFMode:
                    break;
                case PropertyId.LiveViewColorTemperature:
                    break;
                case PropertyId.LiveViewDepthOfFieldPreview:
                    break;
                case PropertyId.LiveViewFocusAid:
                    break;
                case PropertyId.LiveViewHistogram:
                    break;
                case PropertyId.LiveViewHistogramStatus:
                    break;
                case PropertyId.LiveViewImagePosition:
                    break;
                case PropertyId.LiveViewMode:
                    break;
                case PropertyId.LiveViewOutputDevice:
                    //if (IsLiveViewOn == true) DownloadEvf();
                    break;
                case PropertyId.LiveViewWhiteBalance:
                    break;
                case PropertyId.LiveViewZoom:
                    break;
                case PropertyId.LiveViewZoomPosition:
                    break;
                case PropertyId.ExposureCompensation:
                    break;
                case PropertyId.FEBracket:
                    break;
                case PropertyId.FilterEffect:
                    break;
                case PropertyId.FirmwareVersion:
                    break;
                case PropertyId.FlashCompensation:
                    break;
                case PropertyId.FlashMode:
                    break;
                case PropertyId.FlashOn:
                    break;
                case PropertyId.FocalLength:
                    break;
                case PropertyId.FocusInfo:
                    break;
                case PropertyId.GPSAltitude:
                    break;
                case PropertyId.GPSAltitudeRef:
                    break;
                case PropertyId.GPSDateStamp:
                    break;
                case PropertyId.GPSLatitude:
                    break;
                case PropertyId.GPSLatitudeRef:
                    break;
                case PropertyId.GPSLongitude:
                    break;
                case PropertyId.GPSLongitudeRef:
                    break;
                case PropertyId.GPSMapDatum:
                    break;
                case PropertyId.GPSSatellites:
                    break;
                case PropertyId.GPSStatus:
                    break;
                case PropertyId.GPSTimeStamp:
                    break;
                case PropertyId.GPSVersionID:
                    break;
                case PropertyId.HDDirectoryStructure:
                    break;
                case PropertyId.ICCProfile:
                    break;
                case PropertyId.ImageQuality:
                    break;
                case PropertyId.ISOBracket:
                    break;
                case PropertyId.ISOSpeed:
                    break;
                case PropertyId.JpegQuality:
                    break;
                case PropertyId.LensName:
                    break;
                case PropertyId.LensStatus:
                    break;
                case PropertyId.Linear:
                    break;
                case PropertyId.MakerName:
                    break;
                case PropertyId.MeteringMode:
                    break;
                case PropertyId.NoiseReduction:
                    break;
                case PropertyId.Orientation:
                    break;
                case PropertyId.OwnerName:
                    break;
                case PropertyId.ParameterSet:
                    break;
                case PropertyId.PhotoEffect:
                    break;
                case PropertyId.PictureStyle:
                    break;
                case PropertyId.PictureStyleCaption:
                    break;
                case PropertyId.PictureStyleDesc:
                    break;
                case PropertyId.ProductName:
                    break;
                //case PropertyId.Re:
                //    break;
                case PropertyId.RedEye:
                    break;
                case PropertyId.SaveTo:
                    break;
                case PropertyId.Sharpness:
                    break;
                case PropertyId.ToneCurve:
                    break;
                case PropertyId.ToningEffect:
                    break;
                case PropertyId.Tv:
                    break;
                case PropertyId.Unknown:
                    break;
                case PropertyId.WBCoeffs:
                    break;
                case PropertyId.WhiteBalance:
                    break;
                case PropertyId.WhiteBalanceBracket:
                    break;
                case PropertyId.WhiteBalanceShift:
                    break;
            }

            //DequeueItem();
            return (uint) ReturnValue.Ok;
        }

        /// <summary>
        /// The camera state changed
        /// </summary>
        /// <param name="inEvent">The StateEvent ID</param>
        /// <param name="inParameter">Parameter from this event</param>
        /// <param name="inContext">...</param>
        /// <returns>An EDSDK errorcode</returns>
        private uint Camera_SDKStateEvent(uint inEvent, uint inParameter, IntPtr inContext)
        {
            //Handle state event here
            switch ((StateEvent) inEvent)
            {
                case StateEvent.All:
                    break;
                case StateEvent.AfResult:
                    break;
                case StateEvent.BulbExposureTime:
                    break;
                case StateEvent.CaptureError:
                    break;
                case StateEvent.InternalError:
                    break;
                case StateEvent.JobStatusChanged:
                    break;
                case StateEvent.Shutdown:
                    if (CameraHasShutdown != null) CameraHasShutdown(this, new EventArgs());
                    break;
                case StateEvent.ShutDownTimerUpdate:
                    break;
                case StateEvent.WillSoonShutDown:
                    break;
            }
            return (uint) ReturnValue.Ok;
        }

        #endregion

        #region Camera commands

        #region Download data

        /// <summary>
        /// Downloads an image to given directory
        /// </summary>
        /// <param name="Info">Pointer to the object. Get it from the SDKObjectEvent.</param>
        /// <param name="directory"></param>
        public void DownloadImage(IntPtr ObjectPointer, string directory)
        {
            SendSDKCommand((cam) =>
            {
                DirectoryItemInformation dirInfo;
                IntPtr streamRef;
                //get information about object
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetDirectoryItemInformation(ObjectPointer, out dirInfo));
                string currentPhoto = Path.Combine(directory, dirInfo.FileName);
                //create filestream to data
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CreateFileStream(currentPhoto, FileCreateDisposition.CreateAlways, Access.ReadWrite, out streamRef));
                //download file
                DownloadData(ObjectPointer, streamRef);
                //release stream
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(streamRef));
            }, MainCamera);
        }

        /// <summary>
        /// Downloads a jpg image from the camera into a Bitmap
        /// </summary>
        /// <param name="Info">The DownloadInfo that is provided by the "DownloadReady" event</param>
        /// <returns>A Bitmap containing the jpg or null if not a jpg</returns>
        public byte[] DownloadImage(IntPtr objectPointer)
        {
            //get information about image
            DirectoryItemInformation dirInfo = new DirectoryItemInformation();
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetDirectoryItemInformation(objectPointer, out dirInfo));

            //check the extension. Raw data cannot be read by the bitmap class
            string ext = Path.GetExtension(dirInfo.FileName).ToLower();
            if (ext == ".jpg" || ext == ".jpeg")
            {
                IntPtr streamRef, jpgPointer;
                uint length;
                Bitmap bmp = null;
                //create memory stream
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CreateMemoryStream(dirInfo.Size, out streamRef));
                //download data to the stream
                DownloadData(objectPointer, streamRef);
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPointer(streamRef, out jpgPointer));
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetLength(streamRef, out length));

                byte[] buffer = new byte[length];
                Marshal.Copy(jpgPointer, buffer, 0, (int)length);
                
                //release data
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(streamRef));

                return buffer;
            }
            else
            {
                //if it's a RAW image, cancel the download and release the image
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.DownloadCancel(objectPointer));
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(objectPointer));
                return null;
            }
        }

        ///// <summary>
        ///// Downloads a jpg image from the camera into a Bitmap
        ///// </summary>
        ///// <param name="Info">The DownloadInfo that is provided by the "DownloadReady" event</param>
        ///// <returns>A Bitmap containing the jpg or null if not a jpg</returns>
        //public Bitmap DownloadImage(IntPtr ObjectPointer)
        //{
        //    //get information about image
        //    EdsdkInvokes.DirectoryItemInfo dirInfo = new EdsdkInvokes.DirectoryItemInfo();
        //    Error = EdsdkInvokes.GetDirectoryItemInfo(ObjectPointer, out dirInfo);

        //    //check the extension. Raw data cannot be read by the bitmap class
        //    string ext = Path.GetExtension(dirInfo.szFileName).ToLower();
        //    if (ext == ".jpg" || ext == ".jpeg")
        //    {
        //        IntPtr streamRef, jpgPointer;
        //        uint length;
        //        Bitmap bmp = null;
        //        //create memory stream
        //        Error = EdsdkInvokes.CreateMemoryStream(dirInfo.Size, out streamRef);
        //        //download data to the stream
        //        DownloadData(ObjectPointer, streamRef);
        //        Error = EdsdkInvokes.GetPointer(streamRef, out jpgPointer);
        //        Error = EdsdkInvokes.GetLength(streamRef, out length);

        //        byte[] data = new byte[length];
        //        Marshal.Copy(jpgPointer, data, 0, (int) length);
        //        //unsafe
        //        //{
        //        //    //create a System.IO.Stream from the pointer
        //        //    using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream((byte*)jpgPointer.ToPointer(), length, length, FileAccess.Read))
        //        //    {
        //        //        //create bitmap from stream (it's a normal jpeg image)
        //        //        bmp = new Bitmap(ums);
        //        //    }
        //        //}

        //        //release data
        //        Error = EdsdkInvokes.Release(streamRef);

        //        return bmp;
        //    }
        //    else
        //    {
        //        //if it's a RAW image, cancel the download and release the image
        //        Error = EdsdkInvokes.DownloadCancel(ObjectPointer);
        //        Error = EdsdkInvokes.Release(ObjectPointer);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Gets the thumbnail of an image (can be raw or jpg)
        /// </summary>
        /// <param name="filepath">The filename of the image</param>
        /// <returns>The thumbnail of the image</returns>
        public Bitmap GetFileThumb(string filepath)
        {
            IntPtr stream;
            //create a filestream to given file
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CreateFileStream(filepath, FileCreateDisposition.OpenExisting, Access.Read, out stream));
            return GetImage(stream, ImageSource.Thumbnail);
        }

        /// <summary>
        /// Downloads data from the camera
        /// </summary>
        /// <param name="objectPointer">Pointer to the object</param>
        /// <param name="stream">Pointer to the stream created in advance</param>
        private void DownloadData(IntPtr objectPointer, IntPtr stream)
        {
            //get information about the object
            DirectoryItemInformation dirInfo;
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetDirectoryItemInformation(objectPointer, out dirInfo));

            try
            {
                //set progress event
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetProgressCallback(stream, SDKProgressCallbackEvent, ProgressOption.Periodically, objectPointer));
                //download the data
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Download(objectPointer, dirInfo.Size, stream));
            }
            finally
            {
                //set the download as complete
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.DownloadComplete(objectPointer));
                //release object
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(objectPointer));
            }
        }

        /// <summary>
        /// Creates a Bitmap out of a stream
        /// </summary>
        /// <param name="img_stream">Image stream</param>
        /// <param name="imageSource">Type of image</param>
        /// <returns>The bitmap from the stream</returns>
        private Bitmap GetImage(IntPtr imgStream, ImageSource imageSource)
        {
            IntPtr stream = IntPtr.Zero;
            IntPtr imgRef = IntPtr.Zero;
            IntPtr streamPointer = IntPtr.Zero;
            ImageInformation imageInfo;

            try
            {
                //create reference and get image info
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CreateImageReference(imgStream, out imgRef));
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetImageInformation(imgRef, imageSource, out imageInfo));

                ImageMaker.SDKData.Structs.Size outputSize = new ImageMaker.SDKData.Structs.Size();
                outputSize.Width = imageInfo.EffectiveRectangle.Width;
                outputSize.Width = imageInfo.EffectiveRectangle.Height;

                //calculate amount of data
                int datalength = outputSize.Height * outputSize.Width * 3;
                //create buffer that stores the image
                byte[] buffer = new byte[datalength];
                //create a stream to the buffer
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CreateMemoryStreamFromPointer(streamPointer, (uint)datalength, out stream));
                //load image into the buffer
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetImage(imgRef, imageSource, TargetImageType.RGB, imageInfo.EffectiveRectangle, outputSize, stream));

                //make BGR from RGB (System.Drawing (i.e. GDI+) uses BGR)
                unsafe
                {
                    byte tmp;
                    fixed (byte* pix = buffer)
                    {
                        for (long i = 0; i < datalength; i += 3)
                        {
                            tmp = pix[i];        //Save B value
                            pix[i] = pix[i + 2]; //Set B value with R value
                            pix[i + 2] = tmp;    //Set R value with B value
                        }
                    }
                }

                //Get pointer to stream
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPointer(stream, out streamPointer));
                //Create bitmap with the data in the buffer
                return new Bitmap(outputSize.Width, outputSize.Height, datalength, PixelFormat.Format24bppRgb, streamPointer);
            }
            finally
            {
                //Release all data
                if (imgStream != IntPtr.Zero) _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(imgStream));
                if (imgRef != IntPtr.Zero) _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(imgRef));
                if (stream != IntPtr.Zero) _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(stream));
            }
        }

        #endregion

        #region Get Settings

        /// <summary>
        /// Gets the list of possible values for the current camera to set.
        /// Only the PropertyIDs "AEModeSelect", "ISO", "Av", "Tv", "MeteringMode" and "ExposureCompensation" are allowed.
        /// </summary>
        /// <param name="propId">The property ID</param>
        /// <returns>A list of available values for the given property ID</returns>
        public List<int> GetSettingsList(PropertyId propId)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                //a list of settings can only be retrieved for following properties
                if (propId == PropertyId.AEMode || propId == PropertyId.ISOSpeed || propId == PropertyId.Av
                    || propId == PropertyId.Tv || propId == PropertyId.MeteringMode || propId == PropertyId.ExposureCompensation)
                {
                    //get the list of possible values
                    PropertyDescription des = new PropertyDescription();
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPropertyDescription(MainCamera.Ref, (uint) propId, out des));
                    return des.Elements.Take(des.NumberOfElements).ToList();
                }
                else throw new ArgumentException("Method cannot be used with this Property ID");
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Gets the current setting of given property ID as an uint
        /// </summary>
        /// <param name="propId">The property ID</param>
        /// <returns>The current setting of the camera</returns>
        public uint GetSetting(uint propId)
        {
            //todo not working
            if (MainCamera.Ref != IntPtr.Zero)
            {
                IntPtr propertyData = IntPtr.Zero;
                int size = 0;
                DataType dataType;
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPropertySize(MainCamera.Ref, propId, 0, out dataType, out size));
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPropertyData(MainCamera.Ref, propId, 0, size, propertyData));
                return 0;
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        ///// <summary>
        ///// Gets the current setting of given property ID as a string
        ///// </summary>
        ///// <param name="PropID">The property ID</param>
        ///// <returns>The current setting of the camera</returns>
        //public string GetStringSetting(uint PropID)
        //{
        //    if (MainCamera.Ref != IntPtr.Zero)
        //    {
        //        string data = String.Empty;
        //        EdsdkInvokes.GetPropertyData(MainCamera.Ref, PropID, 0, out data);
        //        return data;
        //    }
        //    else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        //}

        /// <summary>
        /// Gets the current setting of given property ID as a struct
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <typeparam name="T">One of the EDSDK structs</typeparam>
        /// <returns>The current setting of the camera</returns>
        public T GetStructSetting<T>(uint PropID) where T : struct
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                //get type and size of struct
                Type structureType = typeof(T);
                int bufferSize = Marshal.SizeOf(structureType);

                //allocate memory
                IntPtr ptr = Marshal.AllocHGlobal(bufferSize);
                //retrieve value
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPropertyData(MainCamera.Ref, PropID, 0, bufferSize, ptr));

                try
                {
                    //convert pointer to managed structure
                    T data = (T)Marshal.PtrToStructure(ptr, structureType);
                    return data;
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                    {
                        //free the allocated memory
                        Marshal.FreeHGlobal(ptr);
                        ptr = IntPtr.Zero;
                    }
                }
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        #endregion

        #region Set Settings

        internal void SetSettingInternal(uint PropID, uint Value, Camera camera)
        {
            if (camera.Ref != IntPtr.Zero)
            {
                SendSDKCommand((cam) =>
                {
                    int propsize;
                    DataType proptype;
                    //get size of property

                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPropertySize(cam.Ref, PropID, 0, out proptype, out propsize));
                    //set given property
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyData(cam.Ref, PropID, 0, propsize, Value));
                }, camera);
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Sets an uint value for the given property ID
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <param name="Value">The value which will be set</param>
        public void SetSetting(uint PropID, uint Value, Camera camera)
        {
            //if (MainCamera.Ref != IntPtr.Zero)
            {
                SendSDKCommand((cam) =>
                {
                    if (cam.Ref != IntPtr.Zero)
                    {
                        int propsize;
                        DataType proptype;
                        //get size of property

                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetPropertySize(cam.Ref, PropID, 0, out proptype, out propsize));
                        //set given property
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyData(cam.Ref, PropID, 0, propsize, Value));
                    }
                    else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
                }, camera);
            }
            
        }

        /// <summary>
        /// Sets a string value for the given property ID
        /// </summary>
        /// <param name="PropID">The property ID</param>
        /// <param name="Value">The value which will be set</param>
        public void SetStringSetting(uint PropID, string Value)
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                if (Value == null) throw new ArgumentNullException("String must not be null");

                //convert string to byte array
                byte[] propertyValueBytes = System.Text.Encoding.ASCII.GetBytes(Value + '\0');
                int propertySize = propertyValueBytes.Length;

                //check size of string
                if (propertySize > 32) throw new ArgumentOutOfRangeException("Value must be smaller than 32 bytes");

                //set value
                SendSDKCommand(
                    (cam) =>
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyData(cam.Ref,
                            PropID, 0, 32, propertyValueBytes)), MainCamera);
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        /// <summary>
        /// Sets a struct value for the given property ID
        /// </summary>
        public void SetStructSetting<T>(uint propId, T value) where T : struct
        {
            if (MainCamera.Ref != IntPtr.Zero)
            {
                SendSDKCommand(
                    (cam) =>
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyData(cam.Ref,
                            propId, 0, Marshal.SizeOf(typeof (T)), value)), MainCamera);
            }
            else { throw new ArgumentNullException("Camera or camera reference is null/zero"); }
        }

        #endregion

        #region Live view

        /// <summary>
        /// Starts the live view
        /// </summary>
        public void StartLiveView(Camera camera)
        {
            if (!IsLiveViewOn)
            {
                IsLiveViewOn = true;
                LVoff = false;

                WrapCommand(() =>
                {
                    SetSetting((uint) PropertyId.LiveViewOutputDevice, (uint) LiveViewOutputDevice.Computer, camera);
                });

                WrapCommand(() => SetSetting((uint) PropertyId.LiveViewMode, 1, camera));
                DownloadEvf(camera);
                
                //DownloadEvf();
            }
        }

        /// <summary>
        /// Stops the live view
        /// </summary>
        public void StopLiveView(bool LVoff = true)
        {
            if (!IsLiveViewOn)
                return;

            this.LVoff = LVoff;
            IsLiveViewOn = false;

            QueueItem((cam) =>
            {
                if (_tokenSource != null)
                    _tokenSource.Cancel();
                else
                {
                    Terminate();
                }

                DequeueItem();
            }, MainCamera, PriorityValue.Critical);
        }

        private CancellationTokenSource _tokenSource;

        private Task _lvTask;

        /// <summary>
        /// Downloads the live view image
        /// </summary>
        private void DownloadEvf(Camera camera)
        {
            if (_lvTask != null && !_lvTask.IsCompleted && !_lvTask.IsCanceled && !_lvTask.IsFaulted)
            {
                Debug.WriteLine("what da fuck????");
                return;
            }

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            _lvTask = Task.Factory.StartNew(() =>
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    Task.Delay(100).Wait();

                    QueueItem((cam) =>
                    {
                        if (_tokenSource.IsCancellationRequested || _block)
                        {
                            Debug.WriteLine("Cancelling");
                            DequeueItem();
                            return;
                        }

                        byte[] buffer = GetLiveViewImage(cam);
                        if (LiveViewUpdated != null) LiveViewUpdated(this, buffer);

                        DequeueItem();
                    }, camera, PriorityValue.Normal);
                }

                SetSetting((uint) PropertyId.LiveViewOutputDevice,
                    (uint) (LVoff ? LiveViewOutputDevice.Computer : LiveViewOutputDevice.Camera), camera);
            }, token, TaskCreationOptions.None,
                TaskScheduler.Default
                ).ContinueWith(t =>
                {
                    if (!CameraSessionOpen)
                    QueueItem((cam) =>
                    {
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CloseSession(cam.Ref));
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(cam.Ref));
                        DequeueItem();
                    }, camera, PriorityValue.Critical);
                    
                    if (_dispose)
                        Terminate();
                }, TaskContinuationOptions.None);
        }

        /// <summary>
        /// Sends a command to the camera safely
        /// </summary>
        private void SendSDKCommand(Action<Camera> command, Camera camera, PriorityValue priorityValue = PriorityValue.High)
        {
            QueueItem((cam) =>
            {
                command(cam);
                DequeueItem();
            }, camera, priorityValue);
        }

        private bool WrapCommand(Action sdkCommand, string name = null)
        {
            if (_dispose || _block)
                return false;

            sdkCommand();
            return true;
        }


        /// <summary>
        /// Get the live view ZoomRect value
        /// </summary>
        /// <param name="imgRef">The live view reference</param>
        /// <returns>ZoomRect value</returns>
        private Rectangle GetEvfZoomRect(IntPtr imgRef)
        {
            int size = Marshal.SizeOf(typeof(Rectangle));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            ReturnValue err = (ReturnValue) EdsdkInvokes.GetPropertyData(imgRef, (uint) PropertyId.LiveViewZoomRectangle, 0, size, ptr);
            Rectangle rect = (Rectangle)Marshal.PtrToStructure(ptr, typeof(Rectangle));
            Marshal.FreeHGlobal(ptr);
            if (err == ReturnValue.Ok) return rect;
            else return new Rectangle();
        }

        /// <summary>
        /// Get the live view coordinate system
        /// </summary>
        /// <param name="imgRef">The live view reference</param>
        /// <returns>the live view coordinate system</returns>
        private Size GetEvfCoord(IntPtr imgRef)
        {
            int size = Marshal.SizeOf(typeof(Size));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            ReturnValue err = (ReturnValue) EdsdkInvokes.GetPropertyData(imgRef, (uint) PropertyId.LiveViewCoordinateSystem, 0, size, ptr);
            _returnValueManager.HandleFunctionReturnValue(err);
            Size coord = (Size)Marshal.PtrToStructure(ptr, typeof(Size));
            Marshal.FreeHGlobal(ptr);
            if (err == ReturnValue.Ok) return coord;
            else return new Size();
        }

        /// <summary>
        /// Get a live view EdsPoint value
        /// </summary>
        /// <returns>a live view EdsPoint value</returns>
        private Point GetEvfPoints(uint propId, IntPtr imgRef)
        {
            int size = Marshal.SizeOf(typeof(Point));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            ReturnValue error = (ReturnValue) EdsdkInvokes.GetPropertyData(imgRef, propId, 0, size, ptr);
            _returnValueManager.HandleFunctionReturnValue(error);
            Point data = (Point)Marshal.PtrToStructure(ptr, typeof(Point));
            Marshal.FreeHGlobal(ptr);
            if (error == ReturnValue.Ok) return data;
            else return new Point();
        }

        #endregion

        #region Filming

        /// <summary>
        /// Starts recording a video and downloads it when finished
        /// </summary>
        /// <param name="FilePath">Directory to where the final video will be saved to</param>
        public void StartFilming(string FilePath)
        {
            if (!IsFilming)
            {
                StartFilming();
                this.DownloadVideo = true;
                ImageSaveDirectory = FilePath;
            }
        }

        /// <summary>
        /// Starts recording a video
        /// </summary>
        public void StartFilming()
        {
            if (!IsFilming)
            {
                //todo doesnt work
                //Check if the camera is ready to film
                //if (GetSetting(EDSDK.PropID_Record) != 3) throw new InvalidOperationException("Camera is not in film mode");

                IsFilming = true;

                //to restore the current setting after recording
                PrevSaveTo = GetSetting((uint) PropertyId.SaveTo);
                //when recording videos, it has to be saved on the camera internal memory
                //SetSetting(EDSDK.PropID_SaveTo, (uint)SaveTo.Camera);
                this.DownloadVideo = false;
                //start the video recording
                //SendSDKCommand(
                //    () =>
                //        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyData(MainCamera.Ref,
                //            EDSDK.PropID_Record, 0, 4, 4)));
            }
        }

        /// <summary>
        /// Stops recording a video
        /// </summary>
        public void StopFilming()
        {
            if (IsFilming)
            {
                //todo doesnt work
                SendSDKCommand((cam) =>
                {
                    //Shut off live view (it will hang otherwise)
                    StopLiveView(false);
                    //stop video recording
                    //_returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetPropertyData(MainCamera.Ref, EDSDK.PropID_Record, 0, 4, 0));
                }, MainCamera);
                //set back to previous state
                //This does not work at the moment, the SDK will hang for unknown reasons
                //SetSetting(EDSDK.PropID_SaveTo, PrevSaveTo);
                IsFilming = false;
            }
        }

        #endregion

        #region Taking photos

        /// <summary>
        /// Press the shutter button
        /// </summary>
        /// <param name="state">State of the shutter button</param>
        public void PressShutterButton(CameraCommand state)
        {
            //send command to camera
            SendSDKCommand(
                (cam) =>
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendCommand(cam.Ref,
                        (uint) CameraCommand.PressShutterButton, (int) state)), MainCamera);
        }

        public void CancelTakingPicture(Camera camera)
        {
            //SendSDKCommand(
            //    (cam) =>
            //        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.DownloadCancel(cam.Ref,
            //            (uint)CameraCommand.PressShutterButton, (int)state)), MainCamera);
        }

        public void Recover()
        {
            try
            {
                SendSDKCommand(cam => EdsdkInvokes.SendCommand(cam.Ref, (uint)CameraCommand.PressShutterButton, 1), MainCamera, PriorityValue.Critical);

                SendSDKCommand(cam => EdsdkInvokes.SendCommand(cam.Ref, (uint)CameraCommand.PressShutterButton, 3), MainCamera, PriorityValue.Critical);

                
            }
            finally
            {
                SendSDKCommand(cam => EdsdkInvokes.SendCommand(cam.Ref, (uint)CameraCommand.PressShutterButton, 0), MainCamera, PriorityValue.Critical);
                
            }
        }

        /// <summary>
        /// Takes a photo with the current camera settings
        /// </summary>
        public void TakePhoto(Action<byte[]> callback, Camera camera)
        {
            Action<byte[]> eventHandler = null;
            eventHandler = stream =>
            {
                ItemDownloaded -= eventHandler;
                callback(stream);
            };

            ItemDownloaded += eventHandler;

            //send command to camera
            SendSDKCommand((cam) => WrapCommand(() => _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendCommand(cam.Ref,
                                (uint) CameraCommand.TakePicture, 0))), camera);
        }

        private void QueueItem(Action<Camera> queueAction, Camera camera, PriorityValue priority)
        {
            _actionFactory.Enqueue(queueAction, camera, priority);
        }

        private void DequeueItem()
        {
            _actionFactory.Dequeue();
        }

        /// <summary>
        /// Takes a photo in bulb mode with the current camera settings
        /// </summary>
        /// <param name="BulbTime">The time in milliseconds for how long the shutter will be open</param>
        public void TakePhoto(uint BulbTime)
        {
            //bulbtime has to be at least a second
            if (BulbTime < 1000) { throw new ArgumentException("Bulbtime has to be bigger than 1000ms"); }

            //start thread to not block everything
            new Thread(delegate()
            {
                SendSDKCommand((cam) => 
                {
                    //open the shutter
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendCommand(cam.Ref, (uint)CameraCommand.BulbStart, 0));
                    //wait for the specified time
                    Thread.Sleep((int)BulbTime);
                    //close shutter
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendCommand(cam.Ref, (uint)CameraCommand.BulbEnd, 0));
                }, MainCamera);
            }).Start();
        }

        #endregion

        #region Other

        /// <summary>
        /// Tells the camera that there is enough space on the HDD if SaveTo is set to Host
        /// This method does not use the actual free space!
        /// </summary>
        public void SetCapacity(Camera camera)
        {            
            //create new capacity struct
            Capacity capacity = new Capacity();

            //set big enough values
            capacity.Reset = 1;
            capacity.BytesPerSector = 0x1000;
            capacity.NumberOfFreeClusters = 0x7FFFFFFF;

            //set the values to camera
            SendSDKCommand((cam) =>
            {
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetCapacity(cam.Ref, capacity));
                //DequeueItem();
            }, camera);
        }

        /// <summary>
        /// Tells the camera how much space is available on the host PC
        /// </summary>
        /// <param name="bytesPerSector">Bytes per sector on HD</param>
        /// <param name="numberOfFreeClusters">Number of free clusters on HD</param>
        public void SetCapacity(int bytesPerSector, int numberOfFreeClusters)
        {
            //create new capacity struct
            Capacity capacity = new Capacity();

            //set given values
            capacity.Reset = 1;
            capacity.BytesPerSector = bytesPerSector;
            capacity.NumberOfFreeClusters = numberOfFreeClusters;

            //set the values to camera
            SendSDKCommand(
                (cam) => _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SetCapacity(cam.Ref, capacity)), MainCamera);
        }

        /// <summary>
        /// Moves the focus (only works while in live view)
        /// </summary>
        /// <param name="speed">Speed and direction of focus movement</param>
        public void SetFocus(uint speed)
        {
            if (IsLiveViewOn) SendSDKCommand((cam) => 
            {
                WrapCommand(() => _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendCommand(cam.Ref, (uint) CameraCommand.DriveLensEvf, (int) speed)));
                
                DequeueItem();
            }, MainCamera);
        }

        /// <summary>
        /// Sets the WB of the live view while in live view
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        public void SetManualWBEvf(ushort x, ushort y)
        {
            if (IsLiveViewOn)
            {
                SendSDKCommand((cam) =>
                {
                    //converts the coordinates to a form the camera accepts
                    byte[] xa = BitConverter.GetBytes(x);
                    byte[] ya = BitConverter.GetBytes(y);
                    uint coord = BitConverter.ToUInt32(new byte[] { xa[0], xa[1], ya[0], ya[1] }, 0);
                    //send command to camera
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendCommand(cam.Ref, (uint) CameraCommand.DoClickWBEvf, (int) coord));
                }, MainCamera);
            }
        }

        /// <summary>
        /// Gets all volumes, folders and files existing on the camera
        /// </summary>
        /// <returns>A CameraFileEntry with all informations</returns>
        public CameraFileEntry GetAllEntries()
        {
            //create the main entry which contains all subentries
            CameraFileEntry MainEntry = new CameraFileEntry("Camera", true);

            //get the number of volumes currently installed in the camera
            int VolumeCount;
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetChildCount(MainCamera.Ref, out VolumeCount));
            List<CameraFileEntry> VolumeEntries = new List<CameraFileEntry>();

            //iterate through all of them
            for (int i = 0; i < VolumeCount; i++)
            {
                //get information about volume
                IntPtr ChildPtr;
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetChildAtIndex(MainCamera.Ref, i, out ChildPtr));
                VolumeInformation vinfo;
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetVolumeInformation(ChildPtr, out vinfo));

                //ignore the HDD
                if (vinfo.VolumeLabel != "HDD")
                {
                    //add volume to the list
                    VolumeEntries.Add(new CameraFileEntry("Volume" + i + "(" + vinfo.VolumeLabel + ")", true));
                    //get all child entries on this volume
                    VolumeEntries[i].AddSubEntries(GetChildren(ChildPtr));
                }
                //release the volume
                _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(ChildPtr));
            }
            //add all volumes to the main entry and return it
            MainEntry.AddSubEntries(VolumeEntries.ToArray());
            return MainEntry;
        }

        /// <summary>
        /// Locks or unlocks the cameras UI
        /// </summary>
        /// <param name="lockState">True for locked, false to unlock</param>
        public void UILock(bool lockState)
        {
            SendSDKCommand((cam) =>
            {
                if (lockState == true) _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendStatusCommand(cam.Ref, (uint) CameraState.UILock, 0));
                else _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.SendStatusCommand(cam.Ref, (uint)CameraState.UILock, 0));
            }, MainCamera);
        }

        /// <summary>
        /// Gets the children of a camera folder/volume. Recursive method.
        /// </summary>
        /// <param name="ptr">Pointer to volume or folder</param>
        /// <returns></returns>
        private CameraFileEntry[] GetChildren(IntPtr ptr)
        {
            int ChildCount;
            //get children of first pointer
            _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetChildCount(ptr, out ChildCount));
            if (ChildCount > 0)
            {
                //if it has children, create an array of entries
                CameraFileEntry[] mainEntry = new CameraFileEntry[ChildCount];
                for (int i = 0; i < ChildCount; i++)
                {
                    IntPtr childPtr;
                    //get children of children
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetChildAtIndex(ptr, i, out childPtr));
                    //get the information about this children
                    DirectoryItemInformation childInfo;
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.GetDirectoryItemInformation(childPtr, out childInfo));

                    //create entry from information
                    mainEntry[i] = new CameraFileEntry(childInfo.FileName, GetBool(childInfo.IsFolder));
                    if (!mainEntry[i].IsFolder)
                    {
                        //if it's not a folder, create thumbnail and safe it to the entry
                        IntPtr stream;
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.CreateMemoryStream(0, out stream));
                        _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.DownloadThumbnail(childPtr, stream));
                        mainEntry[i].AddThumb(GetImage(stream, ImageSource.Thumbnail));
                    }
                    else
                    {
                        //if it's a folder, check for children with recursion
                        CameraFileEntry[] retval = GetChildren(childPtr);
                        if (retval != null) mainEntry[i].AddSubEntries(retval);
                    }
                    //release current children
                    _returnValueManager.HandleFunctionReturnValue(EdsdkInvokes.Release(childPtr));
                }
                return mainEntry;
            }
            else return null;
        }

        /// <summary>
        /// Converts an int to a bool
        /// </summary>
        /// <param name="val">Value</param>
        /// <returns>A bool created from the value</returns>
        private bool GetBool(int val)
        {
            if (val == 0) return false;
            else return true;
        }

        #endregion

        #endregion
    }
}
