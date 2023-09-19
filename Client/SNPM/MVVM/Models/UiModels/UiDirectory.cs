using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SNPM.MVVM.Models.UiModels
{
    internal class UiDirectory : IUiDirectory
    {
        private string name;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name
        {
            get => name;
            set 
            {
                OldName = name;
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public ObservableCollection<IUiDirectory> Children { get; }

        public string OldName { get; set; }

        public UiDirectory(int Id, int ParentId, string name, PropertyChangedEventHandler propertyChangedEventHandler)
        {
            this.Id = Id;
            this.ParentId = ParentId;
            this.name = name;
            OldName = name;

            PropertyChanged += propertyChangedEventHandler;

            Children = new ObservableCollection<IUiDirectory>();
        }
    }
}
