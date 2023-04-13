using Autodesk.Revit.DB;

namespace RAA_Level2.Models
{
    public class ElementWrapper
    {
        public ElementId Id { get; private set; }
        public string FamilyName { get; set; }
        public string FamilyType { get; set; }
        public string FamilyNameAndType { get; private set; }

        public ElementWrapper(Element element)
        {
            var doc = element.Document;

            if (!(doc.GetElement(element.GetTypeId()) is FamilySymbol type))
            {
                return;
            }

            Id = element.Id;
            FamilyName = type.FamilyName;
            FamilyType = type.Name;
            FamilyNameAndType = $"{FamilyName} - {FamilyType}";
        }

        public ElementWrapper(ElementType elemType)
        {
            if (!(elemType is FamilySymbol type))
            {
                return;
            }

            Id = elemType.Id;
            FamilyName = type.FamilyName;
            FamilyType = type.Name;
            FamilyNameAndType = $"{FamilyName} - {FamilyType}";
        }
    }
}
