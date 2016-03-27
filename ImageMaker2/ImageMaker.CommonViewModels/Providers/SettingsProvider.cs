using System;
using System.Monads;
using ImageMaker.Common.Dto;
using ImageMaker.Common.Extensions;
using ImageMaker.Common.Helpers;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;
using ImageMaker.SDKData;
using NLog;

namespace ImageMaker.CommonViewModels.Providers
{
    public class SettingsProvider
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashBuilder _hashBuilder;
        private readonly Lazy<User> _user;

        public SettingsProvider(IUserRepository userRepository, IHashBuilder hashBuilder)
        {
            _userRepository = userRepository;
            _hashBuilder = hashBuilder;
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

        public virtual ThemeSettingsDto GetThemeSettings()
        {
            return _user.Value.ThemeSettings.Deserialize<ThemeSettingsDto>();
        }

        public virtual bool ValidatePassword(string password)
        {
            return _hashBuilder.ValidatePassword(password, _user.Value.Password);
        }

        public virtual bool ChangePassword(string password, string newpassword)
        {
            if (!_hashBuilder.ValidatePassword(password, _user.Value.Password))
            {
                return false;
            }
            try
            {
                newpassword = _hashBuilder.HashPassword(newpassword);
                _user.Value.Password = newpassword;
                _userRepository.UpdateUser(_user.Value);
                _userRepository.Commit();
                return true;
            }
            catch(Exception ex) {
                LogManager.GetCurrentClassLogger().Error(ex);
                return false;
            }
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

        public virtual void SaveThemeSettings(ThemeSettingsDto settings)
        {
            _user.Value.ThemeSettings = settings.With(x=>x.Serialize());
            _userRepository.UpdateUser(_user.Value);
            _userRepository.Commit();
        }
    }
}
