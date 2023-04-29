#region Namespaces
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RAA_Level2.Classes;
using RAA_Level2.Models;
using RAA_Level2.Utilities.BaseClasses;
#endregion

namespace RAA_Level2.ViewModels
{
    public class SheetMakerViewModel : ViewModelBase
    {
        private readonly SheetMakerModel _model;
        private ObservableCollection<ElementWrapper> _titleblocks;
        private ObservableCollection<NewSheetWrapper> _newSheets;

        public ObservableCollection<NewSheetWrapper> NewSheets
        {
            get { return _newSheets; }
            set { _newSheets = value; RaisePropertyChanged(); }
        }
        
        public ObservableCollection<ElementWrapper> Titleblocks 
        {
            get { return _titleblocks; }
            set { _titleblocks = value; RaisePropertyChanged(); } 
        }

        public SheetMakerViewModel(SheetMakerModel model) 
        {
            _model = model;
            NewSheets = new ObservableCollection<NewSheetWrapper>();
            Titleblocks = _model.CollectTitleblocks();

            NewSheetWrapper initSheet = new NewSheetWrapper
            {
                SheetNumber = "A101",
                SheetName = "NEW SHEET",
                IsPlaceholder = false
            };

            NewSheets.Add(initSheet);

            // ***** Register Commands *****
            AddSheetCommand = new DelegateCommand(OnAddSheetRow);
            RemoveSheetCommand = new DelegateCommand(OnRemoveSheetRow);
        }

        #region Commands
        public DelegateCommand AddSheetCommand { get; }
        public DelegateCommand RemoveSheetCommand { get; }
        #endregion

        private void OnAddSheetRow(object parameter)
        {
            int sheetCount = NewSheets.Count;
            sheetCount++;
            string sheetNum = Convert.ToString(sheetCount);
            string prefix = sheetCount < 10 ? "A10" : "A1";
            NewSheets.Add(new NewSheetWrapper { SheetNumber = prefix + sheetNum, SheetName = "NEW SHEET"});
        }

        private void OnRemoveSheetRow(object parameter) 
        {
            try
            {
                System.Collections.IList items = (System.Collections.IList)parameter;
                List<NewSheetWrapper> selectedSheets = items.Cast<NewSheetWrapper>().ToList();

                if (selectedSheets.Any())
                {
                    foreach (NewSheetWrapper sheet in selectedSheets)
                    {
                        NewSheets.Remove(sheet);
                    }
                }
                else
                {
                    int lastSheetRowIndex = NewSheets.Count - 1;

                    if (lastSheetRowIndex >= 0)
                    {
                        NewSheets.RemoveAt(lastSheetRowIndex);
                    }
                }
            }
            catch (InvalidCastException)
            {
                
            }
        }
    }
}
