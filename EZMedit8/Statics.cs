using EZMedit8.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;


namespace EZMedit8
{
    public static class Statics
    {
        #region CONSTANTS: Strings (Private)
        private static readonly string PROGRAMDATA_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EZMedit8");
        private static readonly string PRESETS_FOLDER = Path.Combine(PROGRAMDATA_FOLDER, "Presets");
        private static readonly string AUDIO_FOLDER = Path.Combine(PROGRAMDATA_FOLDER, "Audio");
        private static readonly string IMAGES_FOLDER = Path.Combine(PROGRAMDATA_FOLDER, "Images");
        private static readonly string LOGS_FOLDER = Path.Combine(PROGRAMDATA_FOLDER, "Logs");
        private static readonly string WORKING_FOLDER = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        private static readonly string SETTINGS_FILENAME = "settings.json";
        private static readonly string DEFAULT_PRESET_EXTENSION = ".ezm";
        private static readonly string DEFAULT_PRESET_FILENAME = $"default_preset{DEFAULT_PRESET_EXTENSION}";
        private static readonly string TEMPLATE_PRESET_FILENAME = $"template_preset{DEFAULT_PRESET_EXTENSION}";
        #endregion

        #region CONSTANTS: Strings (Public)
        public static readonly string FILE_DIALOG_FILTER_IMAGES = "Image Files(*.BMP;*.GIF;*.JPG;*.PNG)|*.BMP;*.GIF;*.JPG;*.PNG|All files (*.*)|*.*";
        public static readonly string FILE_DIALOG_FILTER_AUDIO = "Audio Files(*.AIFF;*.MP3;*.WAV)|*.AIFF;*.MP3;*.WAV|All files (*.*)|*.*";
        public static readonly string FILE_DIALOG_FILTER_EZM = "EZMedit8 Session Data Preset(*.EZM)|*.EZM";
        #endregion

        #region CONSTANTS: Doubles
        public static double NORMAL_SCREEN_SIZE { get; } = 500;
        public static double WidthRatio { get; set; }
        public static double HeightRatio { get; set; }
        #endregion

        #region PROPERTIES: Booleans (Public)
        public static bool WindowStateMaximized { get; set; }
        #endregion

        #region PROPERTIES: EventHandler<T> Objects (Public)
        public static EventHandler<UserControlChangedEventArgs> UserControlChangedHandler { get; set; }

        public static EventHandler<bool> ToggleWindowStateMaximized { get; set; }
        #endregion

        #region PROPERTIES: System Information
        public static Rect ScreenDimensions { get; set; }
        #endregion

        #region FIELDS: Settings
        private static Settings Settings = new();
        #endregion

        #region METHODS: Settings
        public static bool LoadSettings()
        {
            Settings.DefaultPresetPath = Path.Combine(PRESETS_FOLDER, DEFAULT_PRESET_FILENAME);
            //return Path.Combine(WORKING_FOLDER, SETTINGS_FILENAME).LoadSettings();
            return true;
        }

        public static bool LoadSettings(this string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) { return false; }

            var fileContents = File.ReadAllText(filename);
            var settings = JsonConvert.DeserializeObject<Settings>(fileContents);
            if (settings is not null) { Settings = settings; }

            return settings is not null;
        }

        public static bool SaveSettings()
        {
            return Path.Combine(WORKING_FOLDER, SETTINGS_FILENAME).SaveSettings();
        }

        public static bool SaveSettings(this string filename)
        {
            if (string.IsNullOrEmpty(filename)) { return false; }
            var settings = JsonConvert.SerializeObject(Settings);

            try { File.WriteAllText(filename, settings); }
            catch (Exception) { return false; }

            return File.Exists(filename);
        }
        #endregion

        #region METHODS: Presets
        public static SessionData LoadPreset()
        {
            return Settings.DefaultPresetPath.LoadPreset();
        }

        public static SessionData LoadPreset(this string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) { return null; }

            var fileContents = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<SessionData>(fileContents);
        }

        public static void SavePreset(this SessionData sessionData)
        {
            sessionData.SavePreset(Settings.DefaultPresetPath);
        }

        public static void SavePreset(this SessionData sessionData, string filename)
        {
            if (string.IsNullOrEmpty(filename)) { return; }

            var preset = JsonConvert.SerializeObject(sessionData, Formatting.Indented);

            try { File.WriteAllText(filename, preset); }
            catch (Exception) { /*TODO: Display Error Msg?*/ }
        }
        #endregion

        #region METHODS: Accessors
        public static string GetFolder(FolderType folderType)
        {
            var folderPath = folderType switch
            {
                FolderType.AppCommonData => PROGRAMDATA_FOLDER,
                FolderType.Audio         => AUDIO_FOLDER,
                FolderType.Images        => IMAGES_FOLDER,
                FolderType.Install       => WORKING_FOLDER,
                FolderType.Logs          => LOGS_FOLDER,
                FolderType.Presets       => PRESETS_FOLDER,
                _                        => ""
            };

            return folderPath;
        }

        public static string GetPresetExtension()
        {
            return DEFAULT_PRESET_EXTENSION;
        }

        public static string GetTemplatePresetFilename()
        {
            return Path.Combine(WORKING_FOLDER, TEMPLATE_PRESET_FILENAME);
        }
        #endregion
    }

    public enum FolderType
    {
        AppCommonData,
        Audio,
        Images,
        Install,
        Logs,
        Presets        
    }
    
    public class Settings
    {
        public string DefaultPresetPath { get; set; }
    }
}