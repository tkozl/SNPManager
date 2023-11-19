using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    public interface IRecordFormViewModel : IViewModel
    {
        event EventHandler RecordCreatedEvent;

        ICommand CancelCommand { get; }

        ICommand ConfirmCommand { get; }

        ICommand AddRelatedWindowCommand { get; }

        IUiRecord CreatedRecord { get; }

        RecordFormView View { get; }

        ObservableCollection<IUiDirectory> Directories { get; }

        IUiDirectory SelectedDirectory { get; }
        ICommand AddParameterCommand { get; }

        void OpenCreateDialog(int id);

        void OpenCreateDialog(IUiRecord uiRecord);
    }
}
