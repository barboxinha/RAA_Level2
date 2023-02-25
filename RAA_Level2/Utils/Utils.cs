using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAA_Level2
{
    internal static class Utils
    {
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        internal static ForgeTypeId GetDocumentUnits(Document doc) 
        {
            Units units = doc.GetUnits();
            FormatOptions formatOps = units.GetFormatOptions(SpecTypeId.Length);
            ForgeTypeId unitTypeId = formatOps.GetUnitTypeId();

            return unitTypeId;
        }

        internal static object GetBuiltinParameterValue(Element element, BuiltInParameter parameterId) 
        {
            object parameterValue;

            Parameter builtinParameter = element.get_Parameter(parameterId);
            StorageType storageType = builtinParameter.StorageType;

            switch (storageType)
            {
                case StorageType.String:
                    parameterValue = builtinParameter.AsString();
                    break;

                case StorageType.Integer:
                    parameterValue = builtinParameter.AsInteger();
                    break;

                case StorageType.Double:
                    parameterValue = builtinParameter.AsDouble();
                    break;

                case StorageType.ElementId:
                    parameterValue = builtinParameter.AsElementId();
                    break;

                default:
                    parameterValue = null;
                    break;
            }

            return parameterValue;
        }
    }
}
