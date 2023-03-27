using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RAA_Level2.Utilities.BaseClasses
{
    /// <summary>
    /// Base class for Models containing handles for Revit UiApp and Doc.
    /// </summary>
    public abstract class ModelBase
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }

        protected ModelBase(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = UiApp.ActiveUIDocument.Document;
        }
    }
}
