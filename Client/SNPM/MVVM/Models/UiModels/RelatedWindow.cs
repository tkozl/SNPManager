using SNPM.Core;
using SNPM.MVVM.Models.UiModels.Interfaces;

namespace SNPM.MVVM.Models.UiModels
{
    public class RelatedWindow : ObservableObject, IRelatedWindow
    {
        private string windowName;

        public string WindowName
        {
            get => windowName;
            set
            {
                OnPropertyChanged(nameof(WindowName));
                windowName = value;
            }
        }

        public RelatedWindow(string windowName)
        {
            this.windowName = windowName;
        }

        public RelatedWindow() : this(string.Empty) { }
    }
}
