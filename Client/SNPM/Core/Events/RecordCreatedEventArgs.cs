using SNPM.MVVM.Models.UiModels.Interfaces;
using System;

namespace SNPM.Core.Events
{
    public class RecordCreatedEventArgs : EventArgs
    {
        public IUiRecord Record { get; }

        public RecordCreatedEventArgs(IUiRecord record) : base()
        {
            Record = record;
        }
    }
}
