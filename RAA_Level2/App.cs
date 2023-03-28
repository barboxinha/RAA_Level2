#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows.Markup;

#endregion

namespace RAA_Level2
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            // 1. Create ribbon tab
            try
            {
                app.CreateRibbonTab("Revit Add-in Academy");
            }
            catch (Exception)
            {
                Debug.Print("Tab already exists.");
            }

            // 2. Create ribbon panel 
            RibbonPanel panel = Utils.CreateRibbonPanel(app, "Revit Add-in Academy", "Revit Tools");

            // 3. Create button data instances
            ButtonDataClass projectSetupButtonData = new ButtonDataClass("ProjectSetup", "Project\rSetup", ProjectSetupCommand.GetMethod(), Properties.Resources.Blue_32, Properties.Resources.Blue_16, "Setup initial Levels and Views from a CSV file.");
            ButtonDataClass viewRenumberButtonData = new ButtonDataClass("ViewRenumber", "View\rRenumber", ViewRenumberCommand.GetMethod(), Properties.Resources.Yellow_32, Properties.Resources.Yellow_16, "Renumber views in a sheet.");
            ButtonDataClass sheetMakerButtonData = new ButtonDataClass("SheetMaker", "Sheet Maker", SheetMakerCommand.GetMethod(), Properties.Resources.Red_32, Properties.Resources.Red_16, "Create new Sheets within the project.");

            // 4. Create buttons
            PushButton projectSetupButton = panel.AddItem(projectSetupButtonData.Data) as PushButton;
            PushButton viewRenumberButton = panel.AddItem(viewRenumberButtonData.Data) as PushButton;
            PushButton sheetMakerButton = panel.AddItem(sheetMakerButtonData.Data) as PushButton;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
