#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RAA_Level2.Classes.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

#endregion

namespace RAA_Level2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ViewRenumberCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            ViewRenumber win = new ViewRenumber(doc, null) 
            {
                Width = 470,
                Height= 500,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost= true,
            };

            win.ShowDialog();

            if (win.DialogResult == false)
            {
                return Result.Cancelled;
            }

            List<Viewport> renumberViewports = null;

            Autodesk.Revit.DB.View activeView = doc.ActiveView;

            if ((activeView != null) && (activeView.ViewType == ViewType.DrawingSheet))
            {
                do
                {
                    try
                    {
                        renumberViewports = SelectSheetViewports(uidoc, doc);
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        TaskDialog.Show("Incomplete Selection", "Click 'Finish' to complete viewport selection.");
                    }
                    
                } while (renumberViewports == null);
            }
            else
            {
                TaskDialog.Show("Error - Incorrect View Type", "The active view is not a Sheet.");
                return Result.Cancelled;
            }

            // Show window with user selections.
            ViewRenumber win2 = new ViewRenumber(doc, renumberViewports);

            win2.ShowDialog();
            win2.btnSelect.IsEnabled = false;

            if (win.DialogResult == false)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        public List<Viewport> SelectSheetViewports(UIDocument uidoc, Document doc) 
        {
            List<Viewport> selectedViewports = new List<Viewport>();

            ISelectionFilter selFilter = new ViewportSelectionFilter();
            List<Reference> pickedRefs = uidoc.Selection.PickObjects(ObjectType.Element, selFilter, "Pick viewports in order.") as List<Reference>;

            foreach (Reference reference in pickedRefs)
            {
                Viewport viewport = doc.GetElement(reference) as Viewport;
                selectedViewports.Add(viewport);
            }

            return selectedViewports;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
