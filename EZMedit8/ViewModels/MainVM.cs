using EZMedit8.Enums;
using EZMedit8.Models.Utilities;
using EZMedit8.Views;
using EZMedit8.Views.UserControls;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;


namespace EZMedit8.ViewModels
{
    public class MainVM : Bindable
    {
        #region PROPERTIES: Bindable
        private object _activeControl;
        public object ActiveControl { get => _activeControl; set => SetProperty(ref _activeControl, value); }

        private string _appName;
        public string AppName { get => _appName; set => SetProperty(ref _appName, value); }

        private string _appVersion;
        public string AppVersion { get => _appVersion; set => SetProperty(ref _appVersion, value); }
        #endregion

        #region PROPERTIES: ICommands
        public ICommand AppShutDownCommand { get; set; }
        #endregion

        #region PROPERTIES: Booleans
        public bool AppShutDownCanExecute { get => _appShutDownCanExecute; set => SetProperty(ref _appShutDownCanExecute, value); }
        #endregion

        #region PROPERTIES: System Information
        private static Point _normalScreenLocation;
        public Point NormalScreenLocation { get => _normalScreenLocation; set => SetProperty(ref _normalScreenLocation, value); }
        #endregion

        #region FIELDS: Booleans
        private static bool _appShutDownCanExecute = true;
        #endregion

        #region METHODS: Constructor(s)
        public MainVM()
        {
            AppShutDownCommand = new DelegateCommand(AppShutDownExecute);
            MainNormalize(Application.Current.MainWindow);
            Statics.UserControlChangedHandler += HandleActiveControl;
            Statics.ToggleWindowStateMaximized += HandleToggleWindowMaximized;
            Init();
        }
        #endregion

        #region METHODS: Init
        private void Init()
        {
            CreateFolders();
            CopyAssets();
            GetVersion();
        }

        public void CreateFolders()
        {
            _ = Directory.CreateDirectory(Statics.GetFolder(FolderType.AppCommonData));
            _ = Directory.CreateDirectory(Statics.GetFolder(FolderType.Images));
            _ = Directory.CreateDirectory(Statics.GetFolder(FolderType.Presets));
        }

        private void CopyAssets()
        {
            var assetsFolder = Path.Combine(Statics.GetFolder(FolderType.Install), "Assets");
            var assets = Directory.GetFiles(assetsFolder, "*.*", new EnumerationOptions() { RecurseSubdirectories = true });            

            foreach (var sourceFilePath in assets) 
            {
                var searchPattern = "\\Assets\\";
                if (!sourceFilePath.Contains(searchPattern)) { continue; }

                int indexStart = sourceFilePath.IndexOf(searchPattern) + searchPattern.Length;
                int indexEnd = sourceFilePath.Length - indexStart;
                var relativeFilePath = sourceFilePath.Substring(indexStart, indexEnd);

                var subfolders = Path.GetDirectoryName(relativeFilePath).Split("\\").ToList();
                var programDataFolderPath = Statics.GetFolder(FolderType.AppCommonData);

                foreach (var subfolder in subfolders)
                {
                    programDataFolderPath = Path.Combine(programDataFolderPath, subfolder);
                    Directory.CreateDirectory(programDataFolderPath);
                }

                var destionationFilePath = Path.Combine(Statics.GetFolder(FolderType.AppCommonData), relativeFilePath);

                if (File.Exists(destionationFilePath)) { continue; }

                File.Copy(sourceFilePath, destionationFilePath);
            }
        }

        private void GetVersion()
        {
            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            var assemblyName = assemblyInfo.Name;
            var assemblyVersion = assemblyInfo.Version.ToString();

            AppName = assemblyName;
            AppVersion = $"v{assemblyVersion}";
        }
        #endregion

        #region METHODS: Commands
        public void AppShutDownExecute(object _)
        {
            if (!AppShutDownCanExecute) { return; }
            AppShutDownCanExecute = false;

            Application.Current.Shutdown(0);

            AppShutDownCanExecute = true;
        }
        #endregion

        #region METHODS: Event Handlers
        private void HandleActiveControl(object sender, UserControlChangedEventArgs e)
        {
            if (e.Handled) { return; }
            e.Handled = true;
            ActiveControl = e.Control switch
            {
                AppUserControls.SessionControl => new SessionControl(),
                _ => new SetupControl()
            };
        }

        private void HandleToggleWindowMaximized(object sender, bool maximizing)
        {
            if (sender is not MainWindow) { return; }
            var main = sender as MainWindow;
            if (maximizing) { MainMaximize(main); }
            else { MainNormalize(main); }
        }

        private void MainMaximize(Window main)
        {            
            NormalScreenLocation = new Point(main.Left, main.Top);
            main.SizeToContent = SizeToContent.Manual;
            main.WindowState = WindowState.Maximized;
        }

        private void MainNormalize(Window main)
        {
            if (NormalScreenLocation.X == 0 && NormalScreenLocation.Y == 0) { return; }
            main.SizeToContent = SizeToContent.WidthAndHeight;
            main.WindowState = WindowState.Normal;
        }
        #endregion
    }
}