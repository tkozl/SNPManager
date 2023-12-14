using System;
using System.Windows.Forms;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IHotkeyService : IDisposable
    {
        bool IsHotkeyEnabled { get; set; }

        event EventHandler<KeyPressedEventArgs> HotKeyPressed;

        void RegisterHotKey(ModifierKeys modifier, Keys key);
    }
}