using EZMedit8.Models.Utilities;


namespace EZMedit8.Models
{
    public class OtherData : Bindable
    {
        private string _filename;
        public string Filename { get => _filename; set => SetProperty(ref _filename, value); }
    }
}