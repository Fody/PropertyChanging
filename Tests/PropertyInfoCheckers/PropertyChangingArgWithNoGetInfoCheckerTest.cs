 using Xunit;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

 public class PropertyChangingArgWithNoGetInfoCheckerTest
 {
     [Fact]
     public void WithGet()
     {
         var checker = new ModuleWeaver();

         var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangingArgWithNoGetInfoCheckerTest>("PropertyWithGet");

         var message = checker.CheckForWarning(
             new()
             {
                 PropertyDefinition = propertyDefinition,
             },
             InvokerTypes.PropertyChangingArg);
         Assert.Null(message);
     }

     [Fact]
     public void NoGet()
     {
         var checker = new ModuleWeaver();

         var propertyDefinition = DefinitionFinder.FindProperty<PropertyChangingArgWithNoGetInfoCheckerTest>("PropertyNoGet");

         var message = checker.CheckForWarning(
             new()
             {
                 PropertyDefinition = propertyDefinition,
             },
             InvokerTypes.PropertyChangingArg);
         Assert.NotNull(message);
     }

     string property;

     public string PropertyNoGet
     {
         set => property = value;
     }

     public string PropertyWithGet
     {
         set => property = value;
         get => property;
     }
 }