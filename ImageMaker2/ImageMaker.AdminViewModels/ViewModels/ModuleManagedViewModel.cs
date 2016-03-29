using System.Windows.Input;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Dto;
using ImageMaker.Common.Enums;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class ModuleManagedViewModel : BaseViewModel
    {

        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IMappingEngine _mappingEngine;
        private ICommand _instaBoxCheckCommand;
        private ICommand _selfyBoxCheckCommand;
        private ICommand _instaPrinterCheckCommand;
        private ICommand _goBackCommand;
        private ICommand _saveSettings;
        private bool _instaBoxSuccess;
        public bool InstaBoxSuccess
        {
            get { return _instaBoxSuccess; }
            set { Set(() => InstaBoxSuccess, ref _instaBoxSuccess, value); }
        }

        private bool _instaPrinterSuccess;
        public bool InstaPrinterSuccess
        {
            get { return _instaPrinterSuccess; }
            set { Set(() => InstaPrinterSuccess, ref _instaPrinterSuccess, value); }
        }

        private bool _selfyBoxSuccess;
        public bool SelfyBoxSuccess
        {
            get { return _selfyBoxSuccess; }
            set { Set(() => SelfyBoxSuccess, ref _selfyBoxSuccess, value); }
        }


        public ModuleManagedViewModel(SettingsProvider settingsProvider, IViewModelNavigator navigator,
            IMappingEngine mappingEngine)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _mappingEngine = mappingEngine;
        }

        public override void Initialize()
        {
            InstaBoxSuccess = false;
            InstaPrinterSuccess = false;
            SelfyBoxSuccess = false;

            ModuleSettingDto settings = _settingsProvider.GetAvailableModules();
            if (settings != null)
                foreach (var module in settings.AvailableModules)
                {
                    switch (module)
                    {
                        case AppModules.InstaBox:
                            InstaBoxSuccess = true;
                            break;
                        case AppModules.InstaPrinter:
                            InstaPrinterSuccess = true;
                            break;
                        case AppModules.SelfyBox:
                            SelfyBoxSuccess = true;
                            break;
                    }
                }
        }

        private void Save()
        {
            _settingsProvider.SaveModuleSettings(_mappingEngine.Map<ModuleSettingDto>(this));
        }
        private void GoBack()
        {
            Dispose();
            _navigator.NavigateBack(this);
        }


        public ICommand InstaBoxCheckCommand => _instaBoxCheckCommand ??
            (_instaBoxCheckCommand = new RelayCommand(() =>
            {
                InstaBoxSuccess = !InstaBoxSuccess;
                Save();
            }));

        public ICommand InstaPrinterCheckCommand => _instaPrinterCheckCommand ??
            (_instaPrinterCheckCommand = new RelayCommand(() =>
            {
                InstaPrinterSuccess = !InstaPrinterSuccess;
                Save();
            }));
        public ICommand SelfyBoxCheckCommand => _selfyBoxCheckCommand ??
            (_selfyBoxCheckCommand = new RelayCommand(() =>
            {
                SelfyBoxSuccess = !SelfyBoxSuccess;
                Save();
            }));

        //public ICommand SaveSettings => _saveSettings ?? (_saveSettings = new RelayCommand(Save));

        public ICommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));
    }
}
