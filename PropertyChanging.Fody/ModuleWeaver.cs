using Fody;

public partial class ModuleWeaver: BaseModuleWeaver
{
    public override void Execute()
    {
        ResolveOnPropertyNameChangingConfig();
        ResolveEventInvokerName();
        FindCoreReferences();
        FindInterceptor();
        BuildTypeNodes();
        CleanDoNotNotifyTypes();
        CleanCodeGenedTypes();
        FindMethodsForNodes();
        FindAllProperties();
        FindMappings();
        DetectIlGeneratedByDependency();
        ProcessDependsOnAttributes();
        WalkPropertyData();
        CheckForWarnings();
        ProcessOnChangingMethods();
        CheckForStackOverflow();
        FindComparisonMethods();
        ProcessTypes();
        CleanAttributes();
    }
    public override bool ShouldCleanReference => true;
}