using System.Linq;

public partial class ModuleWeaver
{
    public bool InjectOnPropertyNameChanging =true;


    public void ResolveOnPropertyNameChangingConfig()
    {
        if (Config != null)
        {
            var value = Config.Attributes("InjectOnPropertyNameChanging").FirstOrDefault();
            if (value != null)
            {
                InjectOnPropertyNameChanging = bool.Parse((string) value);
            }
        }
    }
}