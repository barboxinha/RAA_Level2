using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAA_Level2.Classes
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
