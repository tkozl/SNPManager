using QRCoder;
using QRCoder.Xaml;
using SNPM.Core;
using SNPM.Core.Options;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SNPM.MVVM.ViewModels
{
    public enum ChangeableOption
    {
        [Display(Name = "Text Size")]
        TextSize,
        [Display(Name = "Dark Mode")]
        DarkMode
    }

    public delegate void PreferenceHandler(string PropertyName, object NewValue);

    public class PreferencesViewModel : ObservableObject, IPreferencesViewModel
    {
        private readonly QRCodeGenerator qrGenerator;

        public Action CloseAction { get; set; }

        public bool Is2FaActive { get; set; }

        public string Label2Fa => Is2FaActive ? "Active" : "Inactive";

        public string Title;
        private readonly IProxyService proxyService;

        public ObservableCollection<IOption> Options { get; set; }

        public ICommand Toggle2Fa { get; }

        public event PreferenceHandler PreferenceChanged;

        public DrawingImage Image { get; set; }

        public PreferencesViewModel(IProxyService proxyService)
        {
            Title = Properties.Resources.UserPreferencesTitle;

            CloseAction = new Action(() => { });

            this.proxyService = proxyService;

            var activity = proxyService.GetAccountActivity();
            Is2FaActive = activity?.Active2fa ?? false;

            qrGenerator = new QRCodeGenerator();

            Toggle2Fa = new RelayCommand(ToggleCommand);

            InitOptions();
        }

        private void InitOptions()
        {
            Options = new()
            {
                new TextBoxOption(ChangeableOption.TextSize),
            };

            foreach (var option in Options)
            {
                option.PropertyChanged += OnChildPropertyChanged;
            }
        }

        private async void ToggleCommand(object param)
        {
            var res = await proxyService.Toggle2Fa();

            var qrCodeData = qrGenerator.CreateQrCode(res, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new XamlQRCode(qrCodeData);
            Image = qrCode.GetGraphic(20);

            OnPropertyChanged(nameof(Image));
        }

        private void OnChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender != null && e.PropertyName != null && sender is IOption option && option.Value != null)
            {
                PreferenceChanged.Invoke(e.PropertyName, option.Value);
            }
        }
    }
}
