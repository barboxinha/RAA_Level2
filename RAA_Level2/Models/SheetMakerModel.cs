#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RAA_Level2.Classes;
using RAA_Level2.Utilities.BaseClasses;
using System.Collections.Generic;
#endregion

namespace RAA_Level2.Models
{
    public class SheetMakerModel : ModelBase
    {
        public SheetMakerModel(UIApplication uiApp) : base(uiApp) 
        { 
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
                //TODO - Calculate sheet center.
                XYZ sheetCenter = new XYZ(0, 0, 0);

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
