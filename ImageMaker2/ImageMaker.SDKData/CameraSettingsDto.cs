using ImageMaker.SDKData.Enums;

namespace ImageMaker.SDKData
{
    public class CameraSettingsDto
    {
        public AEMode SelectedAeMode { get; set; }

        public ExposureCompensation SelectedCompensation { get; set; }

        public WhiteBalance SelectedWhiteBalance { get; set; }

        public CameraISOSensitivity SelectedIsoSensitivity { get; set; }

        public ShutterSpeed SelectedShutterSpeed { get; set; }

        public ApertureValue SelectedAvValue { get; set; }


        public AEMode SelectedPhotoAeMode { get; set; }

        public ExposureCompensation SelectedPhotoCompensation { get; set; }

        public WhiteBalance SelectedPhotoWhiteBalance { get; set; }

        public CameraISOSensitivity SelectedPhotoIsoSensitivity { get; set; }

        public ShutterSpeed SelectedPhotoShutterSpeed { get; set; }

        public ApertureValue SelectedPhotoAvValue { get; set; }
    }
}
