using System.Linq;
using System.Xml.Linq;

public class VersionReader
{
    public Version FrameworkVersionAsNumber;
    public string FrameworkVersionAsString;
    public string TargetFrameworkProfile;
    public string TargetFSharpCoreVersion;
    public bool IsFSharp;

    public VersionReader(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();
        GetFrameworkVersion(xDocument);
        GetTargetFrameworkProfile(xDocument);
        GetTargetFSharpCoreVersion(xDocument);
    }

    void GetFrameworkVersion(XDocument xDocument)
    {
        FrameworkVersionAsString = xDocument.Descendants("TargetFrameworkVersion")
            .Select(_ => _.Value)
            .First();
        FrameworkVersionAsNumber = Version.Parse(FrameworkVersionAsString.Remove(0, 1));
    }


    void GetTargetFrameworkProfile(XDocument xDocument)
    {
        TargetFrameworkProfile = xDocument.Descendants("TargetFrameworkProfile")
            .Select(_ => _.Value)
            .FirstOrDefault();
    }

    void GetTargetFSharpCoreVersion(XDocument xDocument)
    {
        TargetFSharpCoreVersion = xDocument.Descendants("TargetFSharpCoreVersion")
            .Select(_ => _.Value)
            .FirstOrDefault();
        if (TargetFSharpCoreVersion != null)
        {
            IsFSharp = true;
        }

    }

}