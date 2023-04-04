using Autodesk.Revit.DB;

namespace RAA_Level2.Models
{
    public class NewSheetWrapper
    {
        public string SheetNumber { get; set; }
        public string SheetName { get; set; }
        public bool IsPlaceholder { get; set; }
        public ElementId TitleblockId { get; set; }
        public View ViewOnSheet { get; set; }
    }
}
