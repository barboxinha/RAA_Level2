#region Namespaces
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RAA_Level2.Classes;
using RAA_Level2.Utilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace RAA_Level2.Models
{
    public class SheetMakerModel : ModelBase
    {
        public SheetMakerModel(UIApplication uiApp) : base(uiApp) 
        { 
        }

        public void SetNewSheetParameters(ViewSheet sheet, NewSheetWrapper newSheet)
        {
            string newSheetNum = newSheet.SheetNumber;
            string newSheetName = newSheet.SheetName;

            Parameter sheetNameParam = sheet.get_Parameter(BuiltInParameter.SHEET_NAME);

            sheet.SheetNumber = newSheetNum;
            sheetNameParam.Set(newSheetName);
        }

        public void CreateNewSheets(IEnumerable<NewSheetWrapper> newSheets)
        {
            Category viewSheetCategory = Category.GetCategory(Doc, BuiltInCategory.OST_Sheets);

            using (Transaction t = new Transaction(Doc))
            {
                t.Start("Create New Sheets");

                foreach (NewSheetWrapper newSheet in newSheets)
                {
                    ViewSheet sheet = null;

                    sheet = newSheet.IsPlaceholder ? 
                            ViewSheet.CreatePlaceholder(Doc) : 
                            ViewSheet.Create(Doc, newSheet.TitleblockId);

                    // Set parameter values after sheet creation
                    if (sheet != null)
                    {
                        SetNewSheetParameters(sheet, newSheet);
                    }
                }

                t.Commit();
            }
        }
    }
}
