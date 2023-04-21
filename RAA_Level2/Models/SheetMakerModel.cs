#region Namespaces
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RAA_Level2.Utilities.BaseClasses;
#endregion

namespace RAA_Level2.Models
{
    public class SheetMakerModel : ModelBase
    {
        public SheetMakerModel(UIApplication uiApp) : base(uiApp) 
        { 
        }

        public ObservableCollection<ElementWrapper> CollectTitleblocks()
        {
            List<FamilySymbol> titleblocks = Utils.GetAllTitleblocks(Doc) as List<FamilySymbol>;
            var sortedTitleblocks = titleblocks.Select(x => new ElementWrapper(x)).OrderBy(x => x.FamilyName);

            return new ObservableCollection<ElementWrapper>(sortedTitleblocks);
        }

        public ObservableCollection<ViewWrapper> CollectAllAvailableViews()
        {
            List<View> availableViews = Utils.GetAllAvailableViews(Doc);
            var sortedViews = availableViews.Select(x => new ViewWrapper(x)).OrderBy(x => x.ViewType);

            return new ObservableCollection<ViewWrapper>(sortedViews);
        }

        private void SetNewSheetParameters(ViewSheet sheet, NewSheetWrapper newSheet)
        {
            string newSheetNum = newSheet.SheetNumber;
            string newSheetName = newSheet.SheetName;

            Parameter sheetNameParam = sheet.get_Parameter(BuiltInParameter.SHEET_NAME);

            sheet.SheetNumber = newSheetNum;
            sheetNameParam.Set(newSheetName);
        }

        private void AddViewToSheet(View view, ViewSheet placementSheet)
        {
            if (view is null)
            {
                return;
            }

            if (Viewport.CanAddViewToSheet(Doc, placementSheet.Id, view.Id))
            {
                XYZ sheetCenter = Utils.GetViewCenter(placementSheet);

                if (view.ViewType != ViewType.Schedule)
                {
                    Viewport placedView = Viewport.Create(Doc, placementSheet.Id, view.Id, sheetCenter);
                }
                else
                {
                    ScheduleSheetInstance placedSchedule = ScheduleSheetInstance.Create(Doc, placementSheet.Id, view.Id, sheetCenter);
                }
            }
        }

        public void CreateNewSheets(IEnumerable<NewSheetWrapper> newSheets)
        {
            List<ViewSheet> createdSheets = new List<ViewSheet>();

            using (Transaction trans = new Transaction(Doc))
            {
                if (trans.Start("Create New Sheets") == TransactionStatus.Started)
                {
                    foreach (NewSheetWrapper newSheet in newSheets)
                    {
                        ViewSheet sheet = null;
                        sheet = newSheet.IsPlaceholder ?
                                ViewSheet.CreatePlaceholder(Doc) :
                                ViewSheet.Create(Doc, newSheet.TitleblockId);

                        // ***** Set parameter values and place views after sheet creation *****
                        if (sheet != null)
                        {
                            SetNewSheetParameters(sheet, newSheet);
                            AddViewToSheet(newSheet.ViewOnSheet, sheet);
                        }

                        createdSheets.Add(sheet);
                    }

                    // ***** Report newly created sheets to user *****
                    string taskMessage = "The following sheets were created successfully:\r\n";

                    foreach (ViewSheet s in createdSheets)
                    {
                        taskMessage += $"{s.SheetNumber} // {s.LookupParameter("Sheet Name").AsString()}";
                    }

                    TaskDialog taskDialog = new TaskDialog("Revit Add-In Academy - SheetMaker");
                    taskDialog.MainInstruction = taskMessage;
                    taskDialog.MainContent = "Click [OK] to Accept the changes. [Cancel] to abort.";
                    TaskDialogCommonButtons taskButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                    taskDialog.CommonButtons = taskButtons;


                    // ***** Give the user the ability to Accept or Cancel the Transaction *****
                    if (taskDialog.Show() == TaskDialogResult.Ok)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.RollBack();
                    }
                }
            }
        }
    }
}
