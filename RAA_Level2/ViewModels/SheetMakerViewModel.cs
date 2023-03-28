#region Namespaces
using RAA_Level2.Classes;
using RAA_Level2.Models;
using RAA_Level2.Utilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public SheetMakerViewModel(SheetMakerModel model) 
        {
            _model = model;
            NewSheets = new ObservableCollection<NewSheetWrapper>();
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
