 

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

 public class PropertyChangingArgWithNoGetInfoCheckerTest
 {
     [Test]
     public async Task WithGet()
     {
         var checker = new ModuleWeaver();

         var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangingArgWithNoGetInfoCheckerTest>("PropertyWithGet");

         var message = checker.CheckForWarning(
             new()
             {
                 PropertyDefinition = propertyDefinition,
             },
             InvokerTypes.PropertyChangingArg);
         await Assert.That(message).IsNull();
     }

     [Test]
     public async Task NoGet()
     {
         var checker = new ModuleWeaver();

         var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangingArgWithNoGetInfoCheckerTest>("PropertyNoGet");

         var message = checker.CheckForWarning(
             new()
             {
                 PropertyDefinition = propertyDefinition,
             },
             InvokerTypes.PropertyChangingArg);
         await Assert.That(message).IsNotNull();
     }

     string property;

     internal string PropertyNoGet
     {
         set => property = value;
     }

     public string PropertyWithGet
     {
         set => property = value;
         get => property;
     }
 }