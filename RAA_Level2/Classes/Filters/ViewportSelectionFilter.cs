using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RAA_Level2.Classes.Filters
{
    public class ViewportSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Name == "Viewports")
            {
                return true; 
            }

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
