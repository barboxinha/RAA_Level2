#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RAA_Level2.Utilities.BaseClasses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public IList<ViewSheet> CreateNewSheets(IEnumerable<NewSheetWrapper> newSheets)
        {
            IList<ViewSheet> createdSheets = new List<ViewSheet>();

            using (Transaction t = new Transaction(Doc))
            {
                t.Start("Create New Sheets");

                foreach (NewSheetWrapper newSheet in newSheets)
                {
                    ViewSheet sheet = null;

                    sheet = newSheet.IsPlaceholder ? 
                            ViewSheet.CreatePlaceholder(Doc) : 
                            ViewSheet.Create(Doc, newSheet.TitleblockId);

                    // Set parameter values and place views after sheet creation
                    if (sheet != null)
                    {
                        SetNewSheetParameters(sheet, newSheet);
                        AddViewToSheet(newSheet.ViewOnSheet, sheet);
                    }

                    createdSheets.Add(sheet);
                }

                t.Commit();
            }

            return createdSheets;
        }
    }
}
