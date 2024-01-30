using System.Linq;
using System.Xml.Linq;

public static class XmlExtensions
{
    public static void StripNamespace(this XDocument document)
    {
        if (document.Root == null)
        {
            return;
        }
        foreach (var element in document.Root.DescendantsAndSelf())
        {
            element.Name = element.Name.LocalName;
            element.ReplaceAttributes(GetAttributes(element).ToList());
        }
    }

    static IEnumerable<XAttribute> GetAttributes(XElement xElement)
    {
        return xElement.Attributes()
            .Where(_ => !_.IsNamespaceDeclaration)
            .Select(_ => new XAttribute(_.Name.LocalName, _.Value));
    }
}