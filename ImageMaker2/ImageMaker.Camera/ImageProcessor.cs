using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EDSDKLib;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;

namespace ImageMaker.Camera
{
    public class ImageProcessor : IDisposable
    {
        public event EventHandler<byte[]> StreamChanged;
        public event EventHandler AddCamera;
        public event EventHandler RemoveCamera;
        protected SDKHandler CameraHandler;
        public bool CameraExist => Cameras.Any();

        public event EventHandler<CameraEventBase> CameraErrorEvent;
        List<EDSDKLib.Camera> CamList;
        bool IsInit = false;
        int BulbTime = 30;

        public ImageProcessor()
        {
            Cameras = new ObservableCollection<EDSDKLib.Camera>();

            CameraHandler = new SDKHandler();
        }

        public void Terminate()
        {
            Debug.WriteLine("disposing...");
            CameraHandler.ErrorEvent -= CameraHandlerOnErrorEvent;
            CameraHandler.CameraAdded -= SDK_CameraAdded;
            CameraHandler.LiveViewUpdated -= SDK_LiveViewUpdated;
            CameraHandler.ProgressChanged -= SDK_ProgressChanged;

            CameraHandler.CameraHasShutdown -= SDK_CameraHasShutdown;

            IsInit = false;
        }

        public void Initialize()
        {
            CameraHandler.Initialize();

            CameraHandler.ErrorEvent += CameraHandlerOnErrorEvent;
            CameraHandler.CameraAdded += SDK_CameraAdded;
            CameraHandler.LiveViewUpdated += SDK_LiveViewUpdated;
            CameraHandler.ProgressChanged += SDK_ProgressChanged;

            CameraHandler.CameraHasShutdown += SDK_CameraHasShutdown;
            RefreshCamera();
            IsInit = true;
        }

        public void SubscribeToCameraAddEvent()
        {
            CameraHandler.CameraAdded += SDK_CameraAdded;
            CameraHandler.Initialize();
        }

        private void CameraHandlerOnErrorEvent(object sender, ErrorEvent errorInfo)
        {
            OnErrorEvent(errorInfo);
        }

        protected virtual void OnStreamChanged(byte[] imageBuffer)
        {
            EventHandler<byte[]> handler = StreamChanged;
            if (handler != null) handler(this, imageBuffer);
        }

        protected virtual void OnErrorEvent(ErrorEvent error)
        {
            RaiseCameraEvent(error);
        }

        protected virtual void OnShutdownEvent()
        {
            RaiseCameraEvent(new ShutdownEvent());
        }


        protected virtual void RaiseCameraEvent(CameraEventBase eventBase)
        {
            EventHandler<CameraEventBase> handler = CameraErrorEvent;
            if (handler != null) handler(this, eventBase);
        }
        public ObservableCollection<EDSDKLib.Camera> Cameras { get; private set; }

        public void Dispose()
        {
            if (!IsInit)
                return;

            Terminate();


            CameraHandler.Dispose();
        }

        #region SDK Events

        private void SDK_ProgressChanged(int progress)
        {
            //if (progress == 100) progress = 0;
            //MainProgressBar.Value = progress;
        }

        private void SDK_LiveViewUpdated(object sender, byte[] imgBuf)
        {
            if (CameraHandler.IsLiveViewOn)
            {
                OnStreamChanged(imgBuf);
            }
        }

        private void SDK_CameraAdded()
        {
            RefreshCamera();
            AddCamera?.Invoke(this, new EventArgs());
        }

        private void SDK_CameraHasShutdown(object sender, EventArgs e)
        {
            Terminate();
            CameraHandler.ShutDown();
            //RefreshCamera();//TODO если что-то сломается, то убрать

            OnShutdownEvent();
            RemoveCamera?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Session

        public bool DoOpenSession()
        {
            if (!CameraHandler.CameraSessionOpen)
                return OpenSession();

            CloseSession();
            return true;
        }

        public EDSDKLib.Camera SelectedCamera { get; private set; }

        public void DoRefresh()
        {
            RefreshCamera();
        }

        public void DoTakePicture(Action<byte[]> callback)
        {
            CameraHandler.StopLiveView();

            CameraHandler.TakePhoto(callback, SelectedCamera); //todo test comment
        }

        public Task<byte[]> DoTakePictureAsync()
        {
            CameraHandler.StopLiveView();

            var source = new TaskCompletionSource<byte[]>();

            CameraHandler.TakePhoto(result => source.TrySetResult(result), SelectedCamera); //todo test comment

            return source.Task;
        }

        public byte[] DoTakePicture()
        {
            CameraHandler.StopLiveView();

            var res = new byte[] {};

            CameraHandler.TakePhoto(result => { res= result; }, SelectedCamera); //todo test comment

            return res;
        }

        #endregion

        #region Settings

        #endregion

        public void StartLiveView()
        {
            if (!CameraHandler.IsLiveViewOn)
            {
                CameraHandler.StartLiveView(SelectedCamera);
            }
            else
            {
                CameraHandler.StopLiveView();
                OnStreamChanged(new byte[] {});
            }
        }


        public void SetFocus(uint focus)
        {
            CameraHandler.SetFocus(focus);
        }

        public IEnumerable<UInt32> GetSettingList(PropertyId propertuId)
        {
            return CameraHandler.GetSettingsList(propertuId);
        }

        public void SetSetting(uint settingId, uint settingValue)
        {
            CameraHandler.SetSetting(settingId, settingValue, SelectedCamera);
        }

        #region Subroutines

        public void CloseSession()
        {
            CameraHandler.CloseSession();
        }

        private void RefreshCamera()
        {
            CloseSession();
            Cameras.Clear();
            foreach (var camera in CameraHandler.GetCameraList())
            {
                Cameras.Add(camera);
            }

            SelectedCamera = Cameras.FirstOrDefault();
        }

        private bool OpenSession()
        {
            if (SelectedCamera != null)
            {
                CameraHandler.OpenSession(SelectedCamera);
                return true;
            }

            return false;
        }

        #endregion
    }
}
