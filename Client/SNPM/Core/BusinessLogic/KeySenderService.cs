using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
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
        private static Keys ComboLookupKey = Keys.F2;

        private readonly IHotkeyService hotkeyService;
        private readonly IRecordBlService recordBlService;
        private readonly IDialogService dialogService;
        private readonly IChoiceViewModel choiceViewModel;

        private string previousWindowTitle;
        private IRecord? chosenRecord;

        public KeySenderService(
            IHotkeyService hotkeyService,
            IRecordBlService recordBlService,
            IDialogService dialogService,
            IChoiceViewModel choiceViewModel)
        {
            this.hotkeyService = hotkeyService;
            this.recordBlService = recordBlService;
            this.dialogService = dialogService;
            this.choiceViewModel = choiceViewModel;
            this.hotkeyService.HotKeyPressed += async (s, e) => await OnHotKeyPressed(s, e);

            previousWindowTitle = string.Empty;
        }

        public void Initialize()
        {
            hotkeyService.RegisterHotKey(AccordKey, QuickLookupKey);
            hotkeyService.RegisterHotKey(AccordKey, ComboLookupKey);
        }

        private async Task OnHotKeyPressed(object? sender, KeyPressedEventArgs e)
        {
            hotkeyService.IsHotkeyEnabled = false;

            if (e.Key == QuickLookupKey)
            {
                await QuickLookup(false);
            }
            else if (e.Key == ComboLookupKey)
            {
                await QuickLookup(true);
            }

            hotkeyService.IsHotkeyEnabled = true;
        }

        private async Task QuickLookup(bool usernameCombo)
        {
            var windowTitle = GetActiveWindowTitle();

            if (windowTitle == previousWindowTitle && chosenRecord != null)
            {
                var strToSend = usernameCombo ? $"{PreparePassword(chosenRecord.Username)}{{TAB}}{PreparePassword(chosenRecord.Password)}" : PreparePassword(chosenRecord.Password);

                SendKeys.SendWait(strToSend);

                previousWindowTitle = string.Empty;
                chosenRecord = null;
                return;
            }
            else
            {
                previousWindowTitle = string.Empty;
            }

            var allRecords = await recordBlService.GetRecordsMatchingTitle(windowTitle);

            if (allRecords.Count() == 0)
            {
                await dialogService.CreateDialogWindow("No password found matching the title", "Please ensure the regular expression is correct", "Ok");
            }
            else if (allRecords.Count() == 1)
            {
                var record = allRecords.First();

                var strToSend = usernameCombo ?
                    $"{PreparePassword(record.Username)}{{TAB}}{PreparePassword(record.Password)}" :
                    PreparePassword(record.Password);


                SendKeys.SendWait(strToSend);
            }
            else
            {
                previousWindowTitle = windowTitle;

                choiceViewModel.Initialize(RecordsToChoices(allRecords), OnChoiceMade);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern bool SetKeyboardState(byte[] lpKeyState);

        private void OnChoiceMade(object chosen)
        {
            if (chosen is IRecord record)
            {
                chosenRecord = record;
            }
        }

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

        private IEnumerable<IChoiceItem> RecordsToChoices(IEnumerable<IRecord> records)
        {
            var choices = new List<IChoiceItem>();

            foreach (var record in records)
            {
                var choice = new ChoiceItem(record, record.EntryId, record.Name);
                choices.Add(choice);
            }

            return choices;
        }
    }
}
