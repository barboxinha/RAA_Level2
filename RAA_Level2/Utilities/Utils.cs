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

        internal static IList<FamilySymbol> GetAllTitleblocks(Document doc)
        {
            IList<FamilySymbol> titleblocks = new FilteredElementCollector(doc)
                                                 .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                 .WhereElementIsElementType()
                                                 .Cast<FamilySymbol>()
                                                 .ToList();
            
            return titleblocks;
        }

        /// <summary>
        /// Calculates the center point of a given View.
        /// </summary>
        /// <param name="view">View to get the center point from.</param>
        /// <returns>The center point of the given View.</returns>
        internal static XYZ GetViewCenter(View view)
        {
            if (view != null)
            {
                BoundingBoxUV outline = view.Outline;
                UV center = new UV();
                center = (outline.Min - outline.Max) * 0.5;
                XYZ viewCenter = new XYZ(center.U, center.V, 0);

                return viewCenter;
            }

            return null;
        }
    }
}
