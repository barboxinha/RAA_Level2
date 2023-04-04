#region Namespaces
using RAA_Level2.Models;
using RAA_Level2.Utilities.BaseClasses;
using System.Collections.ObjectModel;
#endregion

namespace RAA_Level2.ViewModels
{
    public class SheetMakerViewModel : ViewModelBase
    {
        private readonly SheetMakerModel _model;
        public SheetMakerModel Model => _model;

        private ObservableCollection<NewSheetWrapper> _newSheets;
        public ObservableCollection<NewSheetWrapper> NewSheets
        {
            get { return _newSheets; }
            set { _newSheets = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ElementWrapper> _titleblocks;
        public ObservableCollection<ElementWrapper> Titleblocks 
        {
            get { return _titleblocks; }
            set { _titleblocks = value; RaisePropertyChanged(); } 
        }

        public SheetMakerViewModel(SheetMakerModel model) 
        {
            _model = model;
            NewSheets = new ObservableCollection<NewSheetWrapper>();
            Titleblocks = Model.CollectTitleblocks();

            NewSheetWrapper newSheet = new NewSheetWrapper
            {
                SheetNumber = "A101",
                SheetName = "TEST SHEET NAME",
                IsPlaceholder = true
            };

            NewSheets.Add(newSheet);
        }
    }
}
