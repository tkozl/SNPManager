using SNPM.MVVM.Models.Interfaces;

namespace SNPM.MVVM.Models.UiModels
{
    internal class ChoiceItem : IChoiceItem
    {
        public object Item { get; }
        public int Id { get; }
        public string DisplayName { get; set; }

        public ChoiceItem(object item, int id, string displayName)
        {
            Item = item;
            Id = id;
            DisplayName = displayName;
        }
    }
}
