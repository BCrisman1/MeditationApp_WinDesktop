using EZMedit8.Enums;
using EZMedit8.Models;
using EZMedit8.Models.Utilities;
using Microsoft.Win32;
using System;
using System.Windows.Input;


namespace EZMedit8.ViewModels
{
    public class ButtonPanelVM : Bindable
    {
        #region PROPERTIES: ICommand
        public ICommand StartMeditationCommand { get; set; }
        public bool StartMeditationCanExecute { get => _startMeditationCanExecute; set => SetProperty(ref _startMeditationCanExecute, value); }

        public ICommand LoadPresetCommand { get; set; }
        public ICommand LoadDefaultPresetCommand { get; set; }
        public ICommand ResetDefaultPresetCommand { get; set; }
        public bool LoadPresetCanExecute { get => _loadPresetCanExecute; set => SetProperty(ref _loadPresetCanExecute, value); }

        public ICommand SavePresetCommand { get; set; }
        public ICommand SaveDefaultPresetCommand { get; set; }
        public bool SavePresetCanExecute { get => _savePresetCanExecute; set => SetProperty(ref _savePresetCanExecute, value); }
        #endregion

        #region PROPERTIES: Command Booleans
        private static bool _startMeditationCanExecute = true;
        private static bool _loadPresetCanExecute = true;
        private static bool _savePresetCanExecute = true;
        #endregion

        #region PROPERTIES: Independent
        private static bool _inited;
        public bool Inited { get => _inited; set => SetProperty(ref _inited, value); }
        #endregion

        #region METHODS: Constructor(s)
        public ButtonPanelVM()
        {
            Init();
        }
        #endregion

        #region METHODS: Init
        private void Init()
        {
            CommandsInit();

            if (Inited) { return; }
            else { Inited = true; }

            LoadDefaultPresetExecute(null);
            System.Diagnostics.Trace.WriteLine("ButtonPanelVM() initalized!");
        }

        private void CommandsInit()
        {
            StartMeditationCommand = new DelegateCommand(StartMeditationExecute);
            LoadPresetCommand = new DelegateCommand(LoadPresetExecute);
            LoadDefaultPresetCommand = new DelegateCommand(LoadDefaultPresetExecute);
            ResetDefaultPresetCommand = new DelegateCommand(ResetDefaultPresetExecute);
            SavePresetCommand = new DelegateCommand(SavePresetExecute);
            SaveDefaultPresetCommand = new DelegateCommand(SaveDefaultPresetExecute);
        }
        #endregion

        #region METHODS: Commands
        public void StartMeditationExecute(object _)
        {
            System.Diagnostics.Debug.WriteLine("ButtonPanelVM.StartMeditationExecute() called!");
            if (!StartMeditationCanExecute) { return; }
            StartMeditationCanExecute = false;

            Statics.UserControlChangedHandler?.Invoke(this, new UserControlChangedEventArgs() { Control = AppUserControls.SessionControl});

            StartMeditationCanExecute = true;
        }

        public void LoadPresetExecute(object _)
        {
            if (!LoadPresetCanExecute) { return; }
            LoadPresetCanExecute = false;

            var ofd = new OpenFileDialog();
            ofd.Filter = Statics.FILE_DIALOG_FILTER_EZM;
            ofd.InitialDirectory = Statics.GetFolder(FolderType.Presets);

            if ((bool)ofd.ShowDialog())
            {
                var sessionData = ofd.FileName.LoadPreset();
                //if (sessionData != null) { SessionData = sessionData; }
            }

            LoadPresetCanExecute = true;
        }

        public void LoadDefaultPresetExecute(object _)
        {
            if (!LoadPresetCanExecute) { return; }
            LoadPresetCanExecute = false;

            Statics.LoadPreset();

            LoadPresetCanExecute = true;
        }

        public void ResetDefaultPresetExecute(object _)
        {
            if (!LoadPresetCanExecute) { return; }
            LoadPresetCanExecute = false;

            SessionData.Instance.ResetData();
            Statics.GetTemplatePresetFilename().LoadPreset();

            LoadPresetCanExecute = true;
        }

        public void SavePresetExecute(object _)
        {
            if (!SavePresetCanExecute) { return; }
            SavePresetCanExecute = false;

            var sfd = new SaveFileDialog();
            sfd.Filter = Statics.FILE_DIALOG_FILTER_EZM;
            sfd.DefaultExt = Statics.GetPresetExtension();
            sfd.InitialDirectory = Statics.GetFolder(FolderType.Presets);

            if ((bool)sfd.ShowDialog()) { SessionData.Instance.SavePreset(sfd.FileName); }

            SavePresetCanExecute = true;
        }

        public void SaveDefaultPresetExecute(object _)
        {
            if (!SavePresetCanExecute) { return; }
            SavePresetCanExecute = false;

            SessionData.Instance.SavePreset();

            SavePresetCanExecute = true;
        }
        #endregion
    }
}