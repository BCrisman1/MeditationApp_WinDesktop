using EZMedit8.Enums;
using System.Windows.Controls;

namespace System
{
    public class UserControlChangedEventArgs : EventArgs
    {
        public AppUserControls Control { get; set; }
        public bool Handled { get; set; }
    }
}