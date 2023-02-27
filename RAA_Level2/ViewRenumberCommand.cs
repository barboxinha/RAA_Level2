#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RAA_Level2.Classes.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            View activeView = doc.ActiveView;

            if ((activeView == null) || (activeView.ViewType != ViewType.DrawingSheet))
            {
                TaskDialog.Show("Error - Incorrect View Type", "The active view is not a Sheet.");

                return Result.Cancelled;
            }

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

            List<Viewport> renumberViewports = new List<Viewport>();

            // ***** Select viewports individually in order. *****
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

            // Show window with user selections.
            ViewRenumber win2 = new ViewRenumber(doc, renumberViewports)
            {
                Width = 700,
                Height = 500,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            win2.ShowDialog();

            if (win2.DialogResult == false)
            {
                return Result.Cancelled;
            }

            // Start detail viewport renumbering
            int renumberSequenceStart = win2.GetRenumberSequenceStart();

            TransactionGroup transGroup = new TransactionGroup(doc, "View Renumber");
            try
            {
                transGroup.Start();

                RenumberSheetViews(doc, renumberViewports, renumberSequenceStart, "zzz");
                RenumberSheetViews(doc, renumberViewports, renumberSequenceStart);

                transGroup.Assimilate();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error - Could Not Renumber", ex.Message);
                transGroup.RollBack();

                return Result.Failed;
            }
            finally
            {
                transGroup.Dispose();
            }

            ViewRenumberResults winResults = new ViewRenumberResults(renumberViewports) 
            {
                Width = 470,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            winResults.ShowDialog();

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

        private void RenumberSheetViews(Document doc, List<Viewport> detailViews, int sequenceStart, string prefix=null) 
        {
            var renumberSequence = Enumerable.Range(sequenceStart, detailViews.Count);

            Transaction t = new Transaction(doc, "Renumber View");

            t.Start();

            foreach (var detail in detailViews.Zip(renumberSequence, (View, Num) => (View, Num)))
            {
                Parameter paramDetailNumber = detail.View.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                string newDetailNum = Convert.ToString(detail.Num);

                if (prefix != null)
                {
                    newDetailNum = prefix + newDetailNum;
                }

                paramDetailNumber.Set(newDetailNum);
            }

            t.Commit();
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
