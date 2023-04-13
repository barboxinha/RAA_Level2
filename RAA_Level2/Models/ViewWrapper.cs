using Autodesk.Revit.DB;

namespace RAA_Level2.Models
{
    public class ViewWrapper
    {
        public ElementId Id { get; private set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public ViewType ViewType { get; set; }

        public ViewWrapper(View view)
        {
            Id = view.Id;
            Name = view.Name;
            Title = view.Title;
            ViewType = view.ViewType;
        }
    }
}
