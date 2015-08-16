using System;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.CommonViewModels.Providers
{
    public class SettingsProvider
    {
        private readonly IUserRepository _userRepository;
        private readonly Lazy<User> _user;

        public SettingsProvider(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _user = new Lazy<User>(() => _userRepository.GetAdmin());
        }

        public virtual AppSettingsDto GetAppSettings()
        {
            return _user.Value.AppSettings.Deserialize<AppSettingsDto>();
        }

        public virtual CameraSettingsDto GetCameraSettings()
        {
            return _user.Value.CameraSettings.Deserialize<CameraSettingsDto>();
        }

        public virtual void SaveCameraSettings(CameraSettingsDto settings)
        {
            _user.Value.CameraSettings = settings.Serialize();
            _userRepository.UpdateUser(_user.Value);
            _userRepository.Commit();
        }

        public virtual void SaveAppSettings(AppSettingsDto settings)
        {
            _user.Value.AppSettings = settings.Serialize();
            _userRepository.UpdateUser(_user.Value);
            _userRepository.Commit();
        }
    }
}
