#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RAA_Level2.Models;
using RAA_Level2.ViewModels;
using System;
using System.Reflection;
#endregion

namespace RAA_Level2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SheetMakerCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                SheetMakerModel model = new SheetMakerModel(uiapp);
                SheetMakerViewModel viewModel = new SheetMakerViewModel(model);

                SheetMakerView view = new SheetMakerView
                {
                    Width = 600,
                    Height = 480,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                    Topmost = true,
                    DataContext = viewModel
                };

                view.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}