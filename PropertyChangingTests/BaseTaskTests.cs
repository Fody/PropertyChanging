using System;
using System.Collections.Generic;
using System.ComponentModel;
#if (DEBUG)
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
#endif
using System.Reflection;
using NUnit.Framework;

public abstract class BaseTaskTests
{
    string projectPath;
    Assembly assembly;

    protected BaseTaskTests(string projectPath)
    {

#if (RELEASE)
            projectPath = projectPath.Replace("Debug", "Release");
#endif
        this.projectPath = projectPath;
    }

    [TestFixtureSetUp]
    public virtual void Setup()
    {
        var weaverHelper = new WeaverHelper(projectPath);
        assembly = weaverHelper.Assembly;
    }



#if (DEBUG)
    [Test]
    [Ignore]
    public virtual void EnsureOnly1RefToMscorLib()
    {
        var moduleDefinition = ModuleDefinition.ReadModule(assembly.CodeBase.Remove(0, 8));
        foreach (var assemblyNameReference in moduleDefinition.AssemblyReferences)
        {
            Trace.WriteLine(assemblyNameReference.FullName);
        }
        Assert.AreEqual(1, moduleDefinition.AssemblyReferences.Count(x => x.Name == "mscorlib"));
    }
#endif

    [Test]
    public virtual void AlsoNotifyFor()
    {
        var instance = assembly.GetInstance("ClassAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void WithNotifyInChildByInterface()
    {
        var instance = assembly.GetInstance("ClassWithNotifyInChildByInterface");
        var propertyEventCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            propertyEventCount++;
        };
        instance.Property = "a";

        Assert.AreEqual(1, propertyEventCount);
        propertyEventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property = "a";
        Assert.AreEqual(0, propertyEventCount);
    }

    [Test]
    public void WithNotifyInChildByAttribute()
    {
        var instance = assembly.GetInstance("ClassWithNotifyInChildByAttribute");
        var propertyEventCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            propertyEventCount++;
        };
        instance.Property = "a";

        Assert.AreEqual(1, propertyEventCount);
        propertyEventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property = "a";
        Assert.AreEqual(0, propertyEventCount);
    }

    [Test]
    public void AlsoNotifyForMultiple()
    {
        var instance = assembly.GetInstance("ClassAlsoNotifyForMultiple");

        var property1EventCalled = false;
        var property2EventCalled = false;
        var property3EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
            if (args.PropertyName == "Property2")
            {
                property2EventCalled = true;
            }
            if (args.PropertyName == "Property3")
            {
                property3EventCalled = true;
            }
        };
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
        Assert.IsTrue(property2EventCalled);
        Assert.IsTrue(property3EventCalled);
        property1EventCalled = false;
        property2EventCalled = false;
        property3EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.IsFalse(property1EventCalled);
        Assert.IsFalse(property2EventCalled);
        Assert.IsFalse(property3EventCalled);
    }

    [Test]
    public virtual void WithFieldGetButNoFieldSet()
    {
        var instance = assembly.GetInstance("ClassWithFieldGetButNoFieldSet");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithDoNotNotify()
    {
        var type = assembly.GetType("ClassWithDoNotNotify", true);
        Assert.IsEmpty(type.GetCustomAttributes(false));
    }

    [Test]
    public void WithNotifyPropertyChangingAttribute_MustCleanAttribute()
    {
        var type = assembly.GetType("ClassWithNotifyPropertyChangingAttribute", true);
        Assert.IsEmpty(type.GetCustomAttributes(false));
    }

    [Test]
    public void WithNotifyPropertyChangingAttribute_MustWeaveNotification()
    {
        var instance = assembly.GetInstance("ClassWithNotifyPropertyChangingAttribute");

        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public void WithNotifyPropertyChangingAttributeGeneric_MustWeaveNotification()
    {
        var type = assembly.GetType("ClassWithNotifyPropertyChangingAttributeGeneric`1",true);
        var makeGenericType = type.MakeGenericType(typeof(string));

        var instance = (dynamic)Activator.CreateInstance(makeGenericType);
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public void WithNotifyPropertyChangingAttributeOnParentAndChild()
    {
        var instance = assembly.GetInstance("ClassWithNotifyPropertyChangingAttributeChild");

        var property1EventCalled = false;
        var property2EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
            if (args.PropertyName == "Property2")
            {
                property2EventCalled = true;
                Assert.AreNotEqual("a", instance.Property2);
            }
        };
        instance.Property1 = "a";
        instance.Property2 = "a";

        Assert.IsTrue(property1EventCalled);
        Assert.IsTrue(property2EventCalled);
    }


    [Test]
    public void WithTernary()
    {
        var instance = assembly.GetInstance("ClassWithTernary");

        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 1;

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public virtual void WithDependencyAfterSet()
    {
        var instance = assembly.GetInstance("ClassWithDependencyAfterSet");

        var property1EventCalled = false;
        var property2EventCalled = false;
        ((INotifyPropertyChanging) instance).PropertyChanging += (sender, args) =>
                                                                   {
                                                                       if (args.PropertyName == "Property1")
                                                                       {
                                                                           property1EventCalled = true;
                                                                       }
                                                                       if (args.PropertyName == "Property2")
                                                                       {
                                                                           property2EventCalled = true;
                                                                           Assert.IsNull(instance.Property2);
                                                                       }
                                                                   };
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
        Assert.IsTrue(property2EventCalled);
    }

    [Test]
    public virtual void VirtualForNonSealed()
    {
        var type = assembly.GetType("ClassThatIsNotSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanging");
        Assert.IsTrue(methodInfo.IsVirtual);
    }
    
    [Test]
    public virtual void SealedForSealed()
    {
        var type = assembly.GetType("ClassThatIsSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanging");
        Assert.IsFalse(methodInfo.IsVirtual);
    }


    [Test]
    public virtual void WithTryCatchInSet()
    {
        var instance = assembly.GetInstance("ClassWithTryCatchInSet");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void WithPropertySetInCatch()
    {
        var instance = assembly.GetInstance("ClassWithPropertySetInCatch");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericChildWithPropertyOnChanging()
    {
        var instance = assembly.GetInstance("GenericChildWithPropertyOnChanging.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericBaseWithPropertyOnChanging()
    {
        var instance = assembly.GetInstance("GenericBaseWithPropertyOnChanging.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void WithDependsOnAndDoNotNotify()
    {
        var instance = assembly.GetInstance("ClassWithDependsOnAndDoNotNotify");
        EventTester.TestProperty(instance, true);
    }


    [Test]
    public virtual void UsingPublicFieldThroughParameter()
    {
        var classWithPublicField = assembly.GetInstance("ClassWithPublicField");
        var classUsingPublicFieldThroughParameter = assembly.GetInstance("ClassUsingPublicFieldThroughParameter");
        classUsingPublicFieldThroughParameter.Write(classWithPublicField);
    }

    [Test]
    public virtual void Equality()
    {
        var instance = assembly.GetInstance("ClassEquality");
        EventTester.TestProperty(instance, "StringProperty", "foo");
        EventTester.TestProperty(instance, "IntProperty", 2);
        EventTester.TestProperty(instance, "NullableIntProperty", 2);
        EventTester.TestProperty(instance, "BoolProperty", true);
        EventTester.TestProperty(instance, "NullableBoolProperty", true);
        EventTester.TestProperty(instance, "ObjectProperty", "foo");
        EventTester.TestProperty(instance, "ArrayProperty", new[] { "foo" });
        EventTester.TestProperty(instance, "ShortProperty", (short)1);
        EventTester.TestProperty(instance, "UShortProperty", (ushort)1);
        EventTester.TestProperty(instance, "ByteProperty", (byte)1);
        EventTester.TestProperty(instance, "SByteProperty", (sbyte)1);
        EventTester.TestProperty(instance, "CharProperty", 'd');
    }
    [Test]
    public virtual void WithCompilerGeneratedAttribute()
    {
        var instance = assembly.GetInstance("ClassWithCompilerGeneratedAttribute");
        EventTester.TestPropertyNotCalled(instance);
    }

    [Test]
    public virtual void WithGeneratedCodeAttribute()
    {
        var instance = assembly.GetInstance("ClassWithGeneratedCodeAttribute");
        EventTester.TestProperty(instance,false);
    }

    [Test]
    public virtual void NoBackingNoEqualityField()
    {
        var instance = assembly.GetInstance("ClassNoBackingNoEqualityField");

        var eventCalled = false;
        instance.PropertyChanging += new PropertyChangingEventHandler((sender, args) =>
        {
            if (args.PropertyName == "StringProperty")
            {
                eventCalled = true;
            }
        });

        instance.StringProperty = "aString";
        Assert.IsTrue(eventCalled);
    }


    [Test]
    public virtual void NoBackingEqualityField()
    {
        var instance = assembly.GetInstance("ClassNoBackingWithEqualityField");

        var eventCalled = false;
        instance.PropertyChanging += new PropertyChangingEventHandler((sender, args) =>
        {
            if (args.PropertyName == "StringProperty")
            {
                eventCalled = true;
            }
        });

        instance.StringProperty = "aString";
        Assert.IsTrue(eventCalled);
    }

    [Test]
    public virtual void WithFieldFromOtherClass()
    {
        var instance = assembly.GetInstance("ClassWithFieldFromOtherClass");

        var eventCalled = false;
        instance.PropertyChanging += new PropertyChangingEventHandler((sender, args) =>
                                                                        {
                                                                            if (args.PropertyName == "Property1")
                                                                            {
                                                                                eventCalled = true;
                                                                            }
                                                                        });

        instance.Property1 = "aString";
        Assert.IsTrue(eventCalled);
    }

    [Test]
    public virtual void WithIndexerClass()
    {
        var instance = assembly.GetInstance("ClassWithIndexer");

        var eventCalled = false;
        instance.PropertyChanging += new PropertyChangingEventHandler((sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                eventCalled = true;
            }
        });

        instance[4] = "aString";
        Assert.AreEqual("aString", instance[4]);
        instance.Property1 = "aString2";
        Assert.IsTrue(eventCalled);
    }

    [Test]
    public virtual void WithOnceRemovedINotify()
    {
        var instance = assembly.GetInstance("ClassWithOnceRemovedINotify");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void WithBranchingReturn1()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturn1");
        var property1EventCalled = false;
        ((INotifyPropertyChanging) instance).PropertyChanging += (sender, args) =>
                                                                     {
                                                                         if (args.PropertyName == "Property1")
                                                                         {
                                                                             property1EventCalled = true;
                                                                         }
                                                                     };
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public virtual void WithBranchingReturn2True()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturn2");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.HasValue = true;
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public virtual void WithBranchingReturn2False()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturn2");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.HasValue = false;
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public virtual void ClassWithBranchingReturnAndNoFieldTrue()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturnAndNoField");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.HasValue = true;
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public virtual void ClassWithBranchingReturnAndNoFieldFalse()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturnAndNoField");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.HasValue = false;
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public virtual void WithBranchingAndBeforeAfterReturn()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturnAndBefore");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void WithGeneric()
    {
        var instance = assembly.GetInstance("ClassWithGenericChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void GenericChildWithProperty()
    {
        var instance = assembly.GetInstance("GenericChildWithProperty.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void GenericBaseWithProperty()
    {
        var instance = assembly.GetInstance("GenericBaseWithProperty.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void GenericChildWithPropertyBefore()
    {
        var instance = assembly.GetInstance("GenericChildWithPropertyBefore.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void GenericBaseWithPropertyBefore()
    {
        var instance = assembly.GetInstance("GenericBaseWithPropertyBefore.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public virtual void Nested()
    {
        var instance1 = assembly.GetInstance("ClassWithNested+ClassNested");
        EventTester.TestProperty(instance1, false);
        var instance2 = assembly.GetInstance("ClassWithNested+ClassNested+ClassNestedNested");
        EventTester.TestProperty(instance2, false);
    }


    [Test]
    public virtual void AlreadyHasNotification()
    {
        var instance = assembly.GetInstance("ClassAlreadyHasNotification");
        var property1EventCalled = false;
		var property2EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
				property1EventCalled= true;
            }
            if (args.PropertyName == "Property2")
			{
				property2EventCalled = true;
            }
        };
        instance.Property1 = "a";

		Assert.IsTrue(property1EventCalled);
		Assert.IsTrue(property2EventCalled);
		property1EventCalled = false;
		property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
		Assert.IsFalse(property1EventCalled);
		Assert.IsFalse(property2EventCalled);
    }
    [Test]
    public virtual void AlreadyHasSingleNotification()
    {
		var instance = assembly.GetInstance("ClassAlreadyHasSingleNotification");
		var property1EventCalled = false;
		var property2EventCalled = false;
		((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
		{
			if (args.PropertyName == "Property1")
			{
				property1EventCalled = true;
			}
			if (args.PropertyName == "Property2")
			{
				property2EventCalled = true;
			}
		};
		instance.Property1 = "a";

		Assert.IsTrue(property1EventCalled);
		Assert.IsTrue(property2EventCalled);
		property1EventCalled = false;
		property2EventCalled = false;
		//Property has not changed on re-set so event not fired
		instance.Property1 = "a";
		Assert.IsFalse(property1EventCalled);
		Assert.IsFalse(property2EventCalled);
    }  
    
    [Test]
    public virtual void AlreadyHasSingleNotificationDiffParamLocation()
    {
        var instance = assembly.GetInstance("ClassAlreadyHasSingleNotificationDiffParamLocation");
		var callCount = 0;
		((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
		{
			if (args.PropertyName == "Property1")
			{
                callCount++;
			}
		};
		instance.Property1 = "a";

        Assert.AreEqual(1, callCount);
        callCount  = 0;
		//Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.AreEqual(0, callCount);
    }

    [Test]
    public virtual void AlreadyHasSingleNotificationDiffSignature()
    {
        var instance = assembly.GetInstance("ClassAlreadyHasSingleNotificationDiffSignature");
		var callCount = 0;
		((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
		{
			if (args.PropertyName == "Property1")
			{
                callCount++;
			}
		};
		instance.Property1 = "a";

        Assert.AreEqual(1, callCount);
        callCount  = 0;
		//Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.AreEqual(0, callCount);
    }

    [Test]
    public virtual void WithBeforeAfterImplementation()
    {
        var instance = assembly.GetInstance("ClassWithBeforeImplementation");
        EventTester.TestProperty(instance, true);
    }
    [Test]
    public virtual void WithBoolPropUsingStringProp()
    {
        var instance = assembly.GetInstance("ClassWithBoolPropUsingStringProp");
        var boolPropertyCalled = false;
        var stringPropertyCalled = false;
        var stringComparePropertyCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
                                                                              {
                                                                                  if (args.PropertyName == "BoolProperty")
                                                                                  {
                                                                                      boolPropertyCalled = true;
                                                                                  }
                                                                                  if (args.PropertyName == "StringProperty")
                                                                                  {
                                                                                      stringPropertyCalled = true;
                                                                                  }
                                                                                  if (args.PropertyName == "StringCompareProperty")
                                                                                  {
                                                                                      stringComparePropertyCalled = true;
                                                                                  }
                                                                              };
        instance.StringProperty = "magicString";

        Assert.IsTrue(boolPropertyCalled);
        Assert.IsTrue(stringPropertyCalled);
        Assert.IsTrue(stringComparePropertyCalled);

         boolPropertyCalled = false;
         stringPropertyCalled = false;
         stringComparePropertyCalled = false;
         instance.StringProperty = "notMagicString";

         Assert.IsFalse(boolPropertyCalled);
         Assert.IsTrue(stringPropertyCalled);
         Assert.IsTrue(stringComparePropertyCalled);
    }

    [Test]
    public virtual void WithBeforeAndSimpleImplementation()
    {
        var instance = assembly.GetInstance("ClassWithBeforeAndSimpleImplementation");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual void HierarchyBeforeAndSimple()
    {
        var instance = assembly.GetInstance("HierarchyBeforeAndSimple.ClassChild");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BeforeCalled);
    }

    [Test]
    public virtual void WithPropertyChangingArgImplementation()
    {
        var instance = assembly.GetInstance("ClassWithPropertyChangingArgImplementation");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual void WithCustomPropertyChanging()
    {
        var instance = assembly.GetInstance("ClassWithCustomPropertyChanging");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void WithExplicitPropertyChanging()
    {
        var instance = assembly.GetInstance("ClassWithExplicitPropertyChanging");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void DependsOn()
    {
        var instance = assembly.GetInstance("ClassDependsOn");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual void WithNotifyInBase()
    {
        var instance = assembly.GetInstance("ClassWithNotifyInBase");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual void Child1()
    {
        var instance = assembly.GetInstance("ComplexHierarchy.ClassChild1");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public virtual void Child2()
    {
        var instance = assembly.GetInstance("ComplexHierarchy.ClassChild2");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void Child3()
    {
        var type = assembly.GetType("ComplexHierarchy.ClassChild3", true);
        dynamic instance = Activator.CreateInstance(type);
        var property1EventCalled = false;
        var property2EventCalled = false;
        instance.PropertyChanging += new PropertyChangingEventHandler((sender, args) =>
                                                                        {
                                                                            if (args.PropertyName == "Property1")
                                                                            {
                                                                                property1EventCalled = true;
                                                                            }
                                                                            if (args.PropertyName == "Property2")
                                                                            {
                                                                                property2EventCalled = true;
                                                                            }
                                                                        });
        instance.Property1 = "a";
        instance.Property2 = "a";

        Assert.IsTrue(property1EventCalled);
        Assert.IsTrue(property2EventCalled);
    }

    [Test]
    public virtual void WithLogicInSet()
    {
        var instance = assembly.GetInstance("ClassWithLogicInSet");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void WithOwnImplementation()
    {
        var instance = assembly.GetInstance("ClassWithOwnImplementation");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }


    [Test]
    public virtual void WithOnChangedAndOnPropertyChanging()
    {
        var instance = assembly.GetInstance("ClassWithOnChangedAndOnPropertyChanging");
        Assert.AreEqual(0, instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.AreEqual(1, instance.OnProperty1ChangingCalled);
    }


    [Test]
    public virtual void WithOnChangedAndNoOnPropertyChanging()
    {
        var instance = assembly.GetInstance("ClassWithOnChangedAndNoOnPropertyChanging");
        Assert.AreEqual(0, instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.AreEqual(1, instance.OnProperty1ChangingCalled);
    }

    [Test]
    public void ReactiveUI()
    {
        var instance = assembly.GetInstance("ClassReactiveUI");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public virtual void WithOnChanging()
    {
        var instance = assembly.GetInstance("ClassWithOnChanging");
        Assert.IsFalse(instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.OnProperty1ChangingCalled);
    }

    [Test]
    public virtual void WithGenericAndLambda()
    {
        var instance = assembly.GetInstance("ClassWithGenericAndLambdaImp");
        EventTester.TestProperty(instance, false);
    }


    [Test]
    public virtual void WithOnChangingBefore()
    {
        var instance = assembly.GetInstance("ClassWithOnChangingBefore");
        Assert.IsFalse(instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.OnProperty1ChangingCalled);
    }


    [Test]
    public virtual void TransitiveDependencies()
    {
        var propertyNames = new List<string>();
        var instance = assembly.GetInstance("TransitiveDependencies");
        instance.PropertyChanging += new PropertyChangingEventHandler((sender, x) => propertyNames.Add(x.PropertyName));
        instance.My = "s";
        Assert.Contains("My",propertyNames);
        Assert.Contains("MyA",propertyNames);
        Assert.Contains("MyAB",propertyNames);
        Assert.Contains("MyABC",propertyNames);

    }
    [Test]
    public virtual void CircularProperties()
    {
        var instance = assembly.GetInstance("ClassCircularProperties");
        instance.Self = "s";
        instance.PropertyA1 = "s";
        instance.PropertyA2 = "s";
        instance.PropertyB1 = "s";
        instance.PropertyB2 = "s";

    }

    [Test]
    public virtual void WithPropertyImpOfAbstractProperty()
    {
        var instance = assembly.GetInstance("ClassWithPropertyImp");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual void EqualityWithDouble()
    {
        var instance = assembly.GetInstance("ClassEqualityWithDouble");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 2d;

        Assert.IsTrue(property1EventCalled);
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = 2d;
        Assert.IsFalse(property1EventCalled);
    }
    [Test]
    public virtual void EqualityWithStruct()
    {
        var instance = assembly.GetInstance("ClassEqualityWithStruct");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        var property1 = assembly.GetInstance("ClassEqualityWithStruct+SimpleStruct");
        instance.Property1 = property1;
        Assert.IsTrue(property1EventCalled);
    }
    [Test]
    public virtual void EqualityWithStructOverload()
    {
        var instance = assembly.GetInstance("ClassEqualityWithStructOverload");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        var property1 = assembly.GetInstance("ClassEqualityWithStructOverload+SimpleStruct");
        property1.X = 5;
        instance.Property1 = property1;

        Assert.IsTrue(property1EventCalled);
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = property1;
        Assert.IsFalse(property1EventCalled);
    }
    [Test]
    public virtual void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }

}