using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.Core.Options;
using SNPM.MVVM.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Action CloseAction { get; set; }

        public string Title;

        public ObservableCollection<IOption> Options { get; set; }

        public event PreferenceHandler PreferenceChanged;

        public PreferencesViewModel()
        {
            Title = Properties.Resources.UserPreferencesTitle;

            CloseAction = new Action(() => { });
            Options = new()
            {
                new TextBoxOption(ChangeableOption.TextSize),
                new CheckBoxOption(ChangeableOption.DarkMode)
            };

            foreach (var option in Options)
            {
                option.PropertyChanged += OnChildPropertyChanged;
            }
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
