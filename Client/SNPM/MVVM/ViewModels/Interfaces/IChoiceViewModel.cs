using SNPM.MVVM.Models.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    public interface IChoiceViewModel : IViewModel
    {
        public delegate void ChoiceCallback(object chosen);

        ObservableCollection<IChoiceItem> Choices { get; set; }

        IChoiceItem SelectedItem { get; set; }
        
        ICommand MakeChoiceCommand { get; set; }

        void Initialize(IEnumerable<IChoiceItem> choiceItems, ChoiceCallback onChoiceMade);
    }
}
