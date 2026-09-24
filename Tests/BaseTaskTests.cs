#pragma warning disable CS0618

public class WeavingTaskTests
{
    static TestResult testResult;

    static WeavingTaskTests()
    {
        VerifyICSharpCodeDecompiler.Initialize();

        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            ignoreCodes: new[] { "0x80131869" }
#if NETCOREAPP2_0
            , runPeVerify: false
#endif
        );
    }

    [Test]
    public virtual async Task AlsoNotifyFor()
    {
        var instance = testResult.GetInstance("ClassAlsoNotifyFor");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public async Task WithNotifyInChildByInterface()
    {
        var instance = testResult.GetInstance("ClassWithNotifyInChildByInterface");
        var propertyEventCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) => { propertyEventCount++; };
        instance.Property = "a";

        await Assert.That(propertyEventCount).IsEqualTo(1);
        propertyEventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property = "a";
        await Assert.That(propertyEventCount).IsEqualTo(0);
    }

    [Test]
    public async Task WithNotifyInChildByAttribute()
    {
        var instance = testResult.GetInstance("ClassWithNotifyInChildByAttribute");
        var propertyEventCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) => { propertyEventCount++; };
        instance.Property = "a";

        await Assert.That(propertyEventCount).IsEqualTo(1);
        propertyEventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property = "a";
        await Assert.That(propertyEventCount).IsEqualTo(0);
    }

    [Test]
    public async Task AlsoNotifyForMultiple()
    {
        var instance = testResult.GetInstance("ClassAlsoNotifyForMultiple");

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

        await Assert.That(property1EventCalled).IsTrue();
        await Assert.That(property2EventCalled).IsTrue();
        await Assert.That(property3EventCalled).IsTrue();
        property1EventCalled = false;
        property2EventCalled = false;
        property3EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        await Assert.That(property1EventCalled).IsFalse();
        await Assert.That(property2EventCalled).IsFalse();
        await Assert.That(property3EventCalled).IsFalse();
    }

    [Test]
    public virtual async Task WithFieldGetButNoFieldSet()
    {
        var instance = testResult.GetInstance("ClassWithFieldGetButNoFieldSet");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task WithDoNotNotify()
    {
        var type = testResult.Assembly.GetType("ClassWithDoNotNotify", true);
        await Assert.That(type.GetCustomAttributes(false)).IsEmpty();
    }

    [Test]
    public async Task WithNotifyPropertyChangingAttribute_MustCleanAttribute()
    {
        var type = testResult.Assembly.GetType("ClassWithNotifyPropertyChangingAttribute", true);
        await Assert.That(type.GetCustomAttributes(false)).IsEmpty();
    }

    [Test]
    public async Task WithNotifyPropertyChangingAttribute_MustWeaveNotification()
    {
        var instance = testResult.GetInstance("ClassWithNotifyPropertyChangingAttribute");

        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = "a";

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public async Task WithNotifyPropertyChangingAttributeGeneric_MustWeaveNotification()
    {
        var type = testResult.Assembly.GetType("ClassWithNotifyPropertyChangingAttributeGeneric`1", true);
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

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public async Task WithNotifyPropertyChangingAttributeOnParentAndChild()
    {
        var instance = testResult.GetInstance("ClassWithNotifyPropertyChangingAttributeChild");

        var property1EventCalled = false;
        var property2EventCalled = false;
        string property2ValueWhenChanging = null;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }

            if (args.PropertyName == "Property2")
            {
                property2EventCalled = true;
                property2ValueWhenChanging = instance.Property2;
            }
        };
        instance.Property1 = "a";
        instance.Property2 = "a";

        await Assert.That(property1EventCalled).IsTrue();
        await Assert.That(property2EventCalled).IsTrue();
        await Assert.That(property2ValueWhenChanging).IsNotEqualTo("a");
    }

    [Test]
    public async Task WithTernary()
    {
        var instance = testResult.GetInstance("ClassWithTernary");

        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 1;

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithDependencyAfterSet()
    {
        var instance = testResult.GetInstance("ClassWithDependencyAfterSet");

        var property1EventCalled = false;
        var property2EventCalled = false;
        string property2ValueWhenChanging = "notSet";
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }

            if (args.PropertyName == "Property2")
            {
                property2EventCalled = true;
                property2ValueWhenChanging = instance.Property2;
            }
        };
        instance.Property1 = "a";

        await Assert.That(property1EventCalled).IsTrue();
        await Assert.That(property2EventCalled).IsTrue();
        await Assert.That(property2ValueWhenChanging).IsNull();
    }

    [Test]
    public virtual async Task VirtualForNonSealed()
    {
        var type = testResult.Assembly.GetType("ClassThatIsNotSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanging");
        await Assert.That(methodInfo.IsVirtual).IsTrue();
    }

    [Test]
    public virtual async Task SealedForSealed()
    {
        var type = testResult.Assembly.GetType("ClassThatIsSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanging");
        await Assert.That(methodInfo.IsVirtual).IsFalse();
    }

    [Test]
    public virtual async Task WithTryCatchInSet()
    {
        var instance = testResult.GetInstance("ClassWithTryCatchInSet");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task WithPropertySetInCatch()
    {
        var instance = testResult.GetInstance("ClassWithPropertySetInCatch");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task GenericChildWithPropertyOnChanging()
    {
        var instance = testResult.GetInstance("GenericChildWithPropertyOnChanging.ClassWithGenericPropertyChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public async Task GenericBaseWithPropertyOnChanging()
    {
        var instance = testResult.GetInstance("GenericBaseWithPropertyOnChanging.ClassWithGenericPropertyChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task WithDependsOnAndDoNotNotify()
    {
        var instance = testResult.GetInstance("ClassWithDependsOnAndDoNotNotify");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual async Task UsingPublicFieldThroughParameter()
    {
        var classWithPublicField = testResult.GetInstance("ClassWithPublicField");
        var classUsingPublicFieldThroughParameter = testResult.GetInstance("ClassUsingPublicFieldThroughParameter");
        classUsingPublicFieldThroughParameter.Write(classWithPublicField);
    }

    [Test]
    public virtual async Task Equality()
    {
        var instance = testResult.GetInstance("ClassEquality");
        await EventTester.TestProperty(instance, "StringProperty", "foo");
        await EventTester.TestProperty(instance, "IntProperty", 2);
        await EventTester.TestProperty(instance, "NullableIntProperty", 2);
        await EventTester.TestProperty(instance, "BoolProperty", true);
        await EventTester.TestProperty(instance, "NullableBoolProperty", true);
        await EventTester.TestProperty(instance, "ObjectProperty", "foo");
        await EventTester.TestProperty(instance, "ArrayProperty", new[] { "foo" });
        await EventTester.TestProperty(instance, "ShortProperty", (short)1);
        await EventTester.TestProperty(instance, "UShortProperty", (ushort)1);
        await EventTester.TestProperty(instance, "ByteProperty", (byte)1);
        await EventTester.TestProperty(instance, "SByteProperty", (sbyte)1);
        await EventTester.TestProperty(instance, "CharProperty", 'd');
    }

    [Test]
    public virtual async Task WithCompilerGeneratedAttribute()
    {
        var instance = testResult.GetInstance("ClassWithCompilerGeneratedAttribute");
        await EventTester.TestPropertyNotCalled(instance);
    }

    [Test]
    public virtual async Task WithGeneratedCodeAttribute()
    {
        var instance = testResult.GetInstance("ClassWithGeneratedCodeAttribute");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task NoBackingNoEqualityField()
    {
        var instance = testResult.GetInstance("ClassNoBackingNoEqualityField");

        var eventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "StringProperty")
            {
                eventCalled = true;
            }
        };

        instance.StringProperty = "aString";
        await Assert.That(eventCalled).IsTrue();
    }

    [Test]
    public virtual async Task NoBackingEqualityField()
    {
        var instance = testResult.GetInstance("ClassNoBackingWithEqualityField");

        var eventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "StringProperty")
            {
                eventCalled = true;
            }
        };

        instance.StringProperty = "aString";
        await Assert.That(eventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithFieldFromOtherClass()
    {
        var instance = testResult.GetInstance("ClassWithFieldFromOtherClass");

        var eventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                eventCalled = true;
            }
        };

        instance.Property1 = "aString";
        await Assert.That(eventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithIndexerClass()
    {
        var instance = testResult.GetInstance("ClassWithIndexer");

        var eventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                eventCalled = true;
            }
        };

        instance[4] = "aString";
        await Assert.That((string)instance[4]).IsEqualTo("aString");
        instance.Property1 = "aString2";
        await Assert.That(eventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithOnceRemovedINotify()
    {
        var instance = testResult.GetInstance("ClassWithOnceRemovedINotify");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task WithBranchingReturn1()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturn1");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = "a";

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithBranchingReturn2True()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturn2");
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

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithBranchingReturn2False()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturn2");
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

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task ClassWithBranchingReturnAndNoFieldTrue()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturnAndNoField");
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

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task ClassWithBranchingReturnAndNoFieldFalse()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturnAndNoField");
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

        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithBranchingAndBeforeAfterReturn()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturnAndBefore");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task WithGeneric()
    {
        var instance = testResult.GetInstance("ClassWithGenericChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task GenericChildWithProperty()
    {
        var instance = testResult.GetInstance("GenericChildWithProperty.ClassWithGenericPropertyChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task GenericBaseWithProperty()
    {
        var instance = testResult.GetInstance("GenericBaseWithProperty.ClassWithGenericPropertyChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task GenericChildWithPropertyBefore()
    {
        var instance = testResult.GetInstance("GenericChildWithPropertyBefore.ClassWithGenericPropertyChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task GenericBaseWithPropertyBefore()
    {
        var instance = testResult.GetInstance("GenericBaseWithPropertyBefore.ClassWithGenericPropertyChild");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task Nested()
    {
        var instance1 = testResult.GetInstance("ClassWithNested+ClassNested");
        await EventTester.TestProperty(instance1, false);
        var instance2 = testResult.GetInstance("ClassWithNested+ClassNested+ClassNestedNested");
        await EventTester.TestProperty(instance2, false);
    }


    [Test]
    public virtual async Task AlreadyHasNotification()
    {
        var instance = testResult.GetInstance("ClassAlreadyHasNotification");
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

        await Assert.That(property1EventCalled).IsTrue();
        await Assert.That(property2EventCalled).IsTrue();
        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        await Assert.That(property1EventCalled).IsFalse();
        await Assert.That(property2EventCalled).IsFalse();
    }

    [Test]
    public virtual async Task AlreadyHasSingleNotification()
    {
        var instance = testResult.GetInstance("ClassAlreadyHasSingleNotification");
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

        await Assert.That(property1EventCalled).IsTrue();
        await Assert.That(property2EventCalled).IsTrue();
        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        await Assert.That(property1EventCalled).IsFalse();
        await Assert.That(property2EventCalled).IsFalse();
    }

    [Test]
    public virtual async Task AlreadyHasSingleNotificationDiffParamLocation()
    {
        var instance = testResult.GetInstance("ClassAlreadyHasSingleNotificationDiffParamLocation");
        var callCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                callCount++;
            }
        };
        instance.Property1 = "a";

        await Assert.That(callCount).IsEqualTo(1);
        callCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        await Assert.That(callCount).IsEqualTo(0);
    }

    [Test]
    public virtual async Task AlreadyHasSingleNotificationDiffSignature()
    {
        var instance = testResult.GetInstance("ClassAlreadyHasSingleNotificationDiffSignature");
        var callCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                callCount++;
            }
        };
        instance.Property1 = "a";

        await Assert.That(callCount).IsEqualTo(1);
        callCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        await Assert.That(callCount).IsEqualTo(0);
    }

    [Test]
    public virtual async Task WithBeforeAfterImplementation()
    {
        var instance = testResult.GetInstance("ClassWithBeforeImplementation");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual async Task WithBoolPropUsingStringProp()
    {
        var instance = testResult.GetInstance("ClassWithBoolPropUsingStringProp");
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

        await Assert.That(boolPropertyCalled).IsTrue();
        await Assert.That(stringPropertyCalled).IsTrue();
        await Assert.That(stringComparePropertyCalled).IsTrue();

        boolPropertyCalled = false;
        stringPropertyCalled = false;
        stringComparePropertyCalled = false;
        instance.StringProperty = "notMagicString";

        await Assert.That(boolPropertyCalled).IsFalse();
        await Assert.That(stringPropertyCalled).IsTrue();
        await Assert.That(stringComparePropertyCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithBeforeAndSimpleImplementation()
    {
        var instance = testResult.GetInstance("ClassWithBeforeAndSimpleImplementation");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual async Task HierarchyBeforeAndSimple()
    {
        var instance = testResult.GetInstance("HierarchyBeforeAndSimple.ClassChild");
        await EventTester.TestProperty(instance, false);
        await Assert.That((bool)instance.BeforeCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithPropertyChangingArgImplementation()
    {
        var instance = testResult.GetInstance("ClassWithPropertyChangingArgImplementation");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual async Task WithCustomPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithCustomPropertyChanging");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task WithExplicitPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithExplicitPropertyChanging");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task DependsOn()
    {
        var instance = testResult.GetInstance("ClassDependsOn");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual async Task WithNotifyInBase()
    {
        var instance = testResult.GetInstance("ClassWithNotifyInBase");
        await EventTester.TestProperty(instance, true);
    }

    [Test]
    public virtual async Task Child1()
    {
        var instance = testResult.GetInstance("ComplexHierarchy.ClassChild1");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task Child2()
    {
        var instance = testResult.GetInstance("ComplexHierarchy.ClassChild2");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task Child3()
    {
        var instance = testResult.GetInstance("ComplexHierarchy.ClassChild3");
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
        instance.Property2 = "a";

        await Assert.That(property1EventCalled).IsTrue();
        await Assert.That(property2EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithLogicInSet()
    {
        var instance = testResult.GetInstance("ClassWithLogicInSet");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task WithOwnImplementation()
    {
        var instance = testResult.GetInstance("ClassWithOwnImplementation");
        await EventTester.TestProperty(instance, false);
        await Assert.That((bool)instance.BaseNotifyCalled).IsTrue();
    }


    [Test]
    public virtual async Task WithOnChangedAndOnPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithOnChangedAndOnPropertyChanging");
        await Assert.That((int)instance.OnProperty1ChangingCalled).IsEqualTo(0);
        await EventTester.TestProperty(instance, false);
        await Assert.That((int)instance.OnProperty1ChangingCalled).IsEqualTo(1);
    }


    [Test]
    public virtual async Task WithOnChangedAndNoOnPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithOnChangedAndNoOnPropertyChanging");
        await Assert.That((int)instance.OnProperty1ChangingCalled).IsEqualTo(0);
        await EventTester.TestProperty(instance, false);
        await Assert.That((int)instance.OnProperty1ChangingCalled).IsEqualTo(1);
    }

    [Test]
    public async Task ReactiveUI()
    {
        var instance = testResult.GetInstance("ClassReactiveUI");
        await EventTester.TestProperty(instance, false);
        await Assert.That((bool)instance.BaseNotifyCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithOnChanging()
    {
        var instance = testResult.GetInstance("ClassWithOnChanging");
        await Assert.That((bool)instance.OnProperty1ChangingCalled).IsFalse();
        await EventTester.TestProperty(instance, false);
        await Assert.That((bool)instance.OnProperty1ChangingCalled).IsTrue();
    }

    [Test]
    public virtual async Task WithGenericAndLambda()
    {
        var instance = testResult.GetInstance("ClassWithGenericAndLambdaImp");
        await EventTester.TestProperty(instance, false);
    }


    [Test]
    public virtual async Task WithOnChangingBefore()
    {
        var instance = testResult.GetInstance("ClassWithOnChangingBefore");
        await Assert.That((bool)instance.OnProperty1ChangingCalled).IsFalse();
        await EventTester.TestProperty(instance, false);
        await Assert.That((bool)instance.OnProperty1ChangingCalled).IsTrue();
    }


    [Test]
    public virtual async Task TransitiveDependencies()
    {
        var propertyNames = new List<string>();
        var instance = testResult.GetInstance("TransitiveDependencies");
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, x) => propertyNames.Add(x.PropertyName);
        instance.My = "s";
        await Assert.That(propertyNames).Contains("My");
        await Assert.That(propertyNames).Contains("MyA");
        await Assert.That(propertyNames).Contains("MyAB");
        await Assert.That(propertyNames).Contains("MyABC");

    }

    [Test]
    public virtual async Task CircularProperties()
    {
        var instance = testResult.GetInstance("ClassCircularProperties");
        instance.Self = "s";
        instance.PropertyA1 = "s";
        instance.PropertyA2 = "s";
        instance.PropertyB1 = "s";
        instance.PropertyB2 = "s";

    }

    [Test]
    public virtual async Task WithPropertyImpOfAbstractProperty()
    {
        var instance = testResult.GetInstance("ClassWithPropertyImp");
        await EventTester.TestProperty(instance, false);
    }

    [Test]
    public virtual async Task EqualityWithDouble()
    {
        var instance = testResult.GetInstance("ClassEqualityWithDouble");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 2d;

        await Assert.That(property1EventCalled).IsTrue();
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = 2d;
        await Assert.That(property1EventCalled).IsFalse();
    }

    [Test]
    public virtual async Task EqualityWithStruct()
    {
        var instance = testResult.GetInstance("ClassEqualityWithStruct");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        var property1 = testResult.GetInstance("ClassEqualityWithStruct+SimpleStruct");
        instance.Property1 = property1;
        await Assert.That(property1EventCalled).IsTrue();
    }

    [Test]
    public virtual async Task EqualityWithStructOverload()
    {
        var instance = testResult.GetInstance("ClassEqualityWithStructOverload");
        var property1EventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        var property1 = testResult.GetInstance("ClassEqualityWithStructOverload+SimpleStruct");
        property1.X = 5;
        instance.Property1 = property1;

        await Assert.That(property1EventCalled).IsTrue();
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = property1;
        await Assert.That(property1EventCalled).IsFalse();
    }

    [Test]
    public async Task ClassWithNullableBackingField()
    {
        var instance = testResult.GetInstance("ClassWithNullableBackingField");
        var isFlagEventCalled = false;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) =>
        {
            if (args.PropertyName == "IsFlag")
            {
                isFlagEventCalled = true;
            }
        };
        instance.IsFlag = true;
        await Assert.That(isFlagEventCalled).IsTrue();

        isFlagEventCalled = false;
        instance.IsFlag = true;
        await Assert.That(isFlagEventCalled).IsFalse();
    }

    [Test]
    [Arguments(nameof(ClassDoNotCheckEquality), 1, 2)]
    [Arguments(nameof(ClassDoNotCheckEqualityWholeClass), 2, 2)]
    [Arguments(nameof(ClassDoNotCheckEqualityWholeClassInherited), 2, 2)]
    public async Task ClassDoNotCheckEquality(string className, int expectedCountProperty1, int expectedCountProperty2)
    {
        var instance = testResult.GetInstance(className);

        instance.Property1 = "sameValue";
        instance.Property1 = "sameValue";
        instance.Property2 = "sameValue";
        instance.Property2 = "sameValue";

        await Assert.That((int)instance.TimesProperty1Changing).IsEqualTo(expectedCountProperty1);
        await Assert.That((int)instance.TimesProperty2Changing).IsEqualTo(expectedCountProperty2);
    }

#if NETFRAMEWORK
    [Test]
    public async Task ClassWithNullableBackingFieldIl()
    {
        using var file = new PEFile(testResult.AssemblyPath);
        var property = new PropertyToDisassemble(file, "ClassWithNullableBackingField", "IsFlag", PropertyParts.Setter);

        await Verifier.Verify(property).UniqueForAssemblyConfiguration();
    }

    [Test]
    public async Task ClassWithNullableAutoPropertyIl()
    {
        using var file = new PEFile(testResult.AssemblyPath);
        var property = new PropertyToDisassemble(file, "ClassWithNullableAutoProperty", "IsFlag", PropertyParts.Setter);

        await Verifier.Verify(property).UniqueForAssemblyConfiguration();
    }

#endif // NETFRAMEWORK
}
