using System.Linq;

public partial class ModuleWeaver
{

    public void CleanReferences()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "PropertyChanging");
        if (referenceToRemove == null)
        {
            LogInfo("\tNo reference to 'PropertyChanging' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'PropertyChanging'.");
    }
}
