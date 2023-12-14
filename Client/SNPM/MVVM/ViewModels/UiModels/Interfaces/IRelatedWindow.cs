using System.ComponentModel;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IRelatedWindow : INotifyPropertyChanged
    {
        string WindowName { get; set; }
    }
}
