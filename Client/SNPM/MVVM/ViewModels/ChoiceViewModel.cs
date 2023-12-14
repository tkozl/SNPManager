using SNPM.Core;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static SNPM.MVVM.ViewModels.Interfaces.IChoiceViewModel;

namespace SNPM.MVVM.ViewModels
{
    public class ChoiceViewModel : IChoiceViewModel
    {
        private ChoiceCallback? OnMadeChoiceCallback;

        public ChoiceViewModel()
        {
            MakeChoiceCommand = new RelayCommand(MakeChoice);

            Choices = new ObservableCollection<IChoiceItem>();

            View = new ChoiceView
            {
                DataContext = this,
            };
        }

        public ObservableCollection<IChoiceItem> Choices { get; set; }

        public Window View { get; set; }

        public ICommand MakeChoiceCommand { get; set; }

        public IChoiceItem SelectedItem { get; set; }

        public void HideView()
        {
            View.Hide();
        }

        public void Initialize(IEnumerable<IChoiceItem> choiceItems, ChoiceCallback onChoiceMade)
        {
            Choices.Clear();
            foreach (var item in choiceItems)
            {
                Choices.Add(item);
            }

            OnMadeChoiceCallback = onChoiceMade;

            ShowView();
        }

        public void ShowView()
        {
            View.Show();
        }

        private void MakeChoice(object obj)
        {
            if (SelectedItem != null)
            {
                OnMadeChoiceCallback?.Invoke(SelectedItem.Item);
                OnMadeChoiceCallback = null;

                HideView();
            }

        }
    }
}
