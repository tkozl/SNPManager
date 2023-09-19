using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    class DirectoryViewModel : IDirectoryViewModel
    {
        private readonly IProxyService proxyService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public IUiDirectory DirectoryTree { get; set; }

        public ObservableCollection<IUiDirectory> RootNodes { get; set; }

        public ICommand NewDirectoryCommand { get; }

        public ICommand RenameDirectoryCommand { get; }

        public ICommand DeleteDirectoryCommand { get; }

        public ICommand SelectedTreeViewItemClickedCommand { get; }
        
        public IUiDirectory? SelectedNode { get; set; }

        public DirectoryViewModel(IProxyService proxyService)
        {
            DirectoryTree = new UiDirectory(1, 0, "Root", OnNodePropertyChanged);
            RootNodes = new ObservableCollection<IUiDirectory>();

            NewDirectoryCommand = new RelayCommand(CreateNewDirectory, CanCreateNewDirectory);
            RenameDirectoryCommand = new RelayCommand(RenameDirectory, CanRenameDirectory);
            DeleteDirectoryCommand = new RelayCommand(DeleteDirectory, CanDeleteDirectory);

            SelectedNode = null;
            this.proxyService = proxyService;
        }

        public void ShowView()
        {
        }

        public void HideView()
        {
        }

        public async Task RebuildDirectoryTree()
        {
            var directories = await proxyService.GetDirectories(1);

            DirectoryTree = BuildTree(directories, new UiDirectory(1, 0, "Root", OnNodePropertyChanged));

            RootNodes.Clear();
            RootNodes.Add(DirectoryTree);
        }

        private IUiDirectory BuildTree(IEnumerable<IDirectory> directories, IDirectory current)
        {
            IUiDirectory treeNode = new UiDirectory(current.Id, current.ParentId, current.Name, OnNodePropertyChanged);
            foreach (var childNode in directories.Where(x => x.ParentId == current.Id))
            {
                treeNode.Children.Add(BuildTree(directories, childNode));
            }

            return treeNode;
        }

        private async void CreateNewDirectory(object sender)
        {
            if (SelectedNode == null)
            {
                return;
            }

            var parentId = SelectedNode?.Id ?? 0;
            var newId = await proxyService.CreateDirectory(parentId, "New Directory");

            IUiDirectory treeNode = new UiDirectory(newId, parentId, "New Directory", OnNodePropertyChanged);
            if (SelectedNode != null)
            {
                SelectedNode.Children.Add(treeNode);
            }
            else
            {
                RootNodes.Add(treeNode);
            }
        }

        private bool CanCreateNewDirectory(object _) => true;

        private async void RenameDirectory(object sender)
        {
            if (sender != null && sender is IUiDirectory directory && directory.OldName != directory.Name && directory.Name != string.Empty)
            {
                await proxyService.MoveDirectory(directory.Id, directory.Name, directory.ParentId);
            }
        }

        private bool CanRenameDirectory(object _) => true;

        private async void DeleteDirectory(object sender)
        {
            if (SelectedNode != null) // TODO: Check if directory can be deleted
            {
                await proxyService.DeleteDirectory(SelectedNode.Id);

                RebuildDirectoryTree();
            }
        }

        private bool CanDeleteDirectory(object _) => true;

        private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && sender is IUiDirectory uiDirectory && uiDirectory.OldName != uiDirectory.Name)
            {
                RenameDirectory(sender);
            }
        }
    }
}
