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

            List<Viewport> renumberViewports;

            Autodesk.Revit.DB.View activeView = doc.ActiveView;

            if ((activeView != null) && (activeView.ViewType == ViewType.DrawingSheet))
            {
                // ***** Select viewports individually in order. *****
                renumberViewports = new List<Viewport>();
                bool SELECTION_IN_PROCESS = true;

                while (SELECTION_IN_PROCESS)
                {
                    try
                    {
                        Viewport selectedViewport = SelectSheetViewport(uidoc, doc);

                        if (selectedViewport != null)
                        {
                            renumberViewports.Add(selectedViewport);
                        }
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        SELECTION_IN_PROCESS = false;
                    }
                }

                // ***** Selection Order is NOT being maintained with this multi-select approach. *****
                //do
                //{
                //    try
                //    {
                //        renumberViewports = SelectSheetViewports(uidoc, doc);
                //    }
                //    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                //    {
                //        TaskDialog.Show("Incomplete Selection", "Click 'Finish' to complete viewport selection.");
                //    }
                    
                //} while (renumberViewports == null);
            }
            else
            {
                TaskDialog.Show("Error - Incorrect View Type", "The active view is not a Sheet.");
                return Result.Cancelled;
            }

            // Show window with user selections.
            ViewRenumber win2 = new ViewRenumber(doc, renumberViewports)
            {
                Width = 470,
                Height = 500,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            win2.ShowDialog();

            if (win2.DialogResult == false)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        private Viewport SelectSheetViewport(UIDocument uidoc, Document doc)
        {
            Viewport selectedViewport;

            ISelectionFilter selFilter = new ViewportSelectionFilter();
            Reference pickedRef = uidoc.Selection.PickObject(ObjectType.Element, selFilter, "Pick viewports in order. Hit 'ESC' when finished...");
            selectedViewport = doc.GetElement(pickedRef) as Viewport;

            return selectedViewport;
        }

        private List<Viewport> SelectSheetViewports(UIDocument uidoc, Document doc) 
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
