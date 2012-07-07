using System.Linq;


public class ReferenceCleaner
{
    ModuleWeaver moduleWeaver;

    public ReferenceCleaner(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
    }

    public void Execute()
    {
        var referenceToRemove = moduleWeaver.ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "PropertyChanging");
        if (referenceToRemove == null)
        {
            moduleWeaver.LogInfo("\tNo reference to 'PropertyChanging' found. References not modified.");
            return;
        }

        moduleWeaver.ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        moduleWeaver.LogInfo("\tRemoving reference to 'PropertyChanging'.");
    }
}
