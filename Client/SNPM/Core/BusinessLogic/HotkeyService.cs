using SNPM.Core.BusinessLogic.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SNPM.Core.BusinessLogic
{
    internal class HotkeyService : IHotkeyService
    {
        private Window window = new Window();
        private int currentId;
        private bool isHotkeyEnabled;

        public HotkeyService()
        {
            currentId = 0;

            window.KeyPressed += delegate (object? sender, KeyPressedEventArgs args)
            {
                if (HotKeyPressed != null)
                {
                    HotKeyPressed(this, args);
                }
            };

            IsHotkeyEnabled = true;
        }

        public bool IsHotkeyEnabled { get => isHotkeyEnabled; set => isHotkeyEnabled = value; }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event EventHandler<KeyPressedEventArgs> HotKeyPressed;

        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            currentId += 1;

            if (!RegisterHotKey(window.Handle, currentId, (uint)modifier, (uint)key))
            {
                throw new InvalidOperationException("Key could not be registered.");
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < currentId; i++)
            {
                UnregisterHotKey(window.Handle, currentId);
            }

            window.Dispose();
        }

        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    if (KeyPressed != null)
                    {
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                    }
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            public void Dispose()
            {
                this.DestroyHandle();
            }
        }
    }

    public class KeyPressedEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier
        {
            get { return _modifier; }
        }

        public Keys Key
        {
            get { return _key; }
        }
    }

    [Flags]
    public enum ModifierKeys
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 0x4000
    }
}
