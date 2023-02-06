#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace RAA_Level2
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Instantiate window
            ProjectSetup win = new ProjectSetup()
            {
                Width = 600,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            if (win.ShowDialog() == false)
            {
                return Result.Cancelled;
            }

            // Begin Revit transaction
            List<Level> newLevels = new List<Level>();
            List<ViewPlan> newFloorPlans = new List<ViewPlan>();
            List<ViewPlan> newRCPs = new List<ViewPlan>();

            Transaction trans = new Transaction(doc, "Create Levels + Views");
            try
            {
                trans.Start();

                // Get existing document Levels and elevations
                var existingLevels = new FilteredElementCollector(doc)
                                        .OfClass(typeof(Level))
                                        .ToElements()
                                        .Cast<Level>();

                List<double> existingElevations = existingLevels.Select(x => x.Elevation).ToList();

                foreach (string[] arr in win.FileData)
                {
                    string levelName = arr[0];
                    string elevation = win.Units == ProjectUnits.Imperial ? arr[1] : arr[2];

                    bool isNumber = double.TryParse(elevation, out double levelElevation);

                    if (isNumber)
                    {
                        if (win.Units == ProjectUnits.Metric)
                        {
                            levelElevation = ConvertMetricToImperial(levelElevation);
                        }

                        // Check Level with same elevation does not exist
                        if (!existingElevations.Contains(levelElevation))
                        {
                            Level level = Level.Create(doc, levelElevation);
                            level.Name = levelName;
                            newLevels.Add(level);

                            if (win.CreateFloorPlans)
                            {
                                ViewFamilyType planFamilyType = GetViewFamilyTypeByName(doc, "Floor Plan", ViewFamily.FloorPlan);
                                ViewPlan plan = ViewPlan.Create(doc, planFamilyType.Id, level.Id);
                                newFloorPlans.Add(plan);
                            }

                            if (win.CreateRCPs)
                            {
                                ViewFamilyType rcpFamilyType = GetViewFamilyTypeByName(doc, "Ceiling Plan", ViewFamily.CeilingPlan);
                                ViewPlan rcp = ViewPlan.Create(doc, rcpFamilyType.Id, level.Id);
                                newRCPs.Add(rcp);
                            }
                        }
                    }
                    else
                    {
                        TaskDialog.Show("Error", $"The item in the Elevation column is not a number.\n{levelName} will be skipped.");
                        continue;
                    }
                }

                trans.Commit();

                if (trans.GetStatus() == TransactionStatus.Committed)
                {
                    if (newLevels.Count > 0)
                    {
                        string msg = $"Levels created: {newLevels.Count}";

                        if (newFloorPlans.Count > 0)
                        {
                            string plansMsg = $"\n\nFloor Plan views created: {newFloorPlans.Count}";
                            msg += plansMsg;
                        }

                        if (newRCPs.Count > 0)
                        {
                            string rcpsMsg = $"\n\nReflected Ceiling Plan views created: {newRCPs.Count}";
                            msg += rcpsMsg;
                        }

                        TaskDialog.Show("Success - Created Levels + Views", msg);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Failed to Create Levels / Views", ex.Message.ToString());
                trans.RollBack();
            }
            finally
            {
                trans.Dispose();
            }

            return Result.Succeeded;
        }

        private double ConvertMetricToImperial(double metricNumber)
        {
            return metricNumber * 3.28084;
        }

        private ViewFamilyType GetViewFamilyTypeByName(Document doc, string typeName, ViewFamily viewFamily)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));

            foreach (ViewFamilyType vft in collector)
            {
                if (((vft.Name == typeName) || (vft.FamilyName == typeName)) && (vft.ViewFamily == viewFamily))
                {
                    return vft;
                }
            }

            return null;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}

