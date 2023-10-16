using SNPM.Core.BusinessLogic.Interfaces;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNPM.Core.BusinessLogic
{
    internal class KeySenderService : IKeySenderService
    {
        private static readonly char[] SpecialKeys = { '+', '^', '%', '~' , '(', ')', '{', '}', '[', ']'};

        private static ModifierKeys AccordKey = ModifierKeys.None;

        private static Keys QuickLookupKey = Keys.F4;
        private static Keys FullLookupKey = Keys.F3;

        private readonly IHotkeyService hotkeyService;
        private readonly IRecordBlService recordBlService;
        private readonly IDialogService dialogService;

        public KeySenderService(IHotkeyService hotkeyService, IRecordBlService recordBlService, IDialogService dialogService)
        {
            this.hotkeyService = hotkeyService;
            this.recordBlService = recordBlService;
            this.dialogService = dialogService;
            this.hotkeyService.HotKeyPressed += async (s, e) => await OnHotKeyPressed(s, e);
        }

        public void Initialize()
        {
            hotkeyService.RegisterHotKey(AccordKey, QuickLookupKey);
            hotkeyService.RegisterHotKey(AccordKey, FullLookupKey);
        }

        private async Task OnHotKeyPressed(object? sender, KeyPressedEventArgs e)
        {
            hotkeyService.IsHotkeyEnabled = false;

            if (e.Key == QuickLookupKey)
            {
                await QuickLookup();
            }
            else if (e.Key == FullLookupKey)
            {
                FullLookup();
            }

            hotkeyService.IsHotkeyEnabled = true;
        }

        private void FullLookup()
        {
            
        }

        private async Task QuickLookup()
        {
            var windowTitle = GetActiveWindowTitle();
            var allPasswords = await recordBlService.GetCompatibleRecordPasswords(windowTitle);

            if (allPasswords.Count() == 0)
            {
                await dialogService.CreateDialogWindow("No password found", "123", "zxc");
            }
            else if (allPasswords.Count() == 1)
            {
                var password = allPasswords.First();

                SendKeys.SendWait(PreparePassword(password));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern bool SetKeyboardState(byte[] lpKeyState);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return string.Empty;
        }

        private string PreparePassword(string password)
        {
            var endStr = new StringBuilder();

            foreach (var c in password)
            {
                if (SpecialKeys.Contains(c))
                {
                    endStr.Append($"{{{c}}}");
                }
                else
                {
                    endStr.Append(c);
                }
            }

            endStr = endStr.Replace("\n", "");

            return endStr.ToString();
        }

    }
}
