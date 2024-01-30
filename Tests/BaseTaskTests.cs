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

    [Fact]
    public virtual void AlsoNotifyFor()
    {
        var instance = testResult.GetInstance("ClassAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public void WithNotifyInChildByInterface()
    {
        var instance = testResult.GetInstance("ClassWithNotifyInChildByInterface");
        var propertyEventCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) => { propertyEventCount++; };
        instance.Property = "a";

        Assert.Equal(1, propertyEventCount);
        propertyEventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property = "a";
        Assert.Equal(0, propertyEventCount);
    }

    [Fact]
    public void WithNotifyInChildByAttribute()
    {
        var instance = testResult.GetInstance("ClassWithNotifyInChildByAttribute");
        var propertyEventCount = 0;
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, args) => { propertyEventCount++; };
        instance.Property = "a";

        Assert.Equal(1, propertyEventCount);
        propertyEventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property = "a";
        Assert.Equal(0, propertyEventCount);
    }

    [Fact]
    public void AlsoNotifyForMultiple()
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

        Assert.True(property1EventCalled);
        Assert.True(property2EventCalled);
        Assert.True(property3EventCalled);
        property1EventCalled = false;
        property2EventCalled = false;
        property3EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
        Assert.False(property2EventCalled);
        Assert.False(property3EventCalled);
    }

    [Fact]
    public virtual void WithFieldGetButNoFieldSet()
    {
        var instance = testResult.GetInstance("ClassWithFieldGetButNoFieldSet");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void WithDoNotNotify()
    {
        var type = testResult.Assembly.GetType("ClassWithDoNotNotify", true);
        Assert.Empty(type.GetCustomAttributes(false));
    }

    [Fact]
    public void WithNotifyPropertyChangingAttribute_MustCleanAttribute()
    {
        var type = testResult.Assembly.GetType("ClassWithNotifyPropertyChangingAttribute", true);
        Assert.Empty(type.GetCustomAttributes(false));
    }

    [Fact]
    public void WithNotifyPropertyChangingAttribute_MustWeaveNotification()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public void WithNotifyPropertyChangingAttributeGeneric_MustWeaveNotification()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public void WithNotifyPropertyChangingAttributeOnParentAndChild()
    {
        var instance = testResult.GetInstance("ClassWithNotifyPropertyChangingAttributeChild");

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
                Assert.NotEqual("a", instance.Property2);
            }
        };
        instance.Property1 = "a";
        instance.Property2 = "a";

        Assert.True(property1EventCalled);
        Assert.True(property2EventCalled);
    }

    [Fact]
    public void WithTernary()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void WithDependencyAfterSet()
    {
        var instance = testResult.GetInstance("ClassWithDependencyAfterSet");

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
                Assert.Null(instance.Property2);
            }
        };
        instance.Property1 = "a";

        Assert.True(property1EventCalled);
        Assert.True(property2EventCalled);
    }

    [Fact]
    public virtual void VirtualForNonSealed()
    {
        var type = testResult.Assembly.GetType("ClassThatIsNotSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanging");
        Assert.True(methodInfo.IsVirtual);
    }

    [Fact]
    public virtual void SealedForSealed()
    {
        var type = testResult.Assembly.GetType("ClassThatIsSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanging");
        Assert.False(methodInfo.IsVirtual);
    }

    [Fact]
    public virtual void WithTryCatchInSet()
    {
        var instance = testResult.GetInstance("ClassWithTryCatchInSet");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void WithPropertySetInCatch()
    {
        var instance = testResult.GetInstance("ClassWithPropertySetInCatch");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericChildWithPropertyOnChanging()
    {
        var instance = testResult.GetInstance("GenericChildWithPropertyOnChanging.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void GenericBaseWithPropertyOnChanging()
    {
        var instance = testResult.GetInstance("GenericBaseWithPropertyOnChanging.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void WithDependsOnAndDoNotNotify()
    {
        var instance = testResult.GetInstance("ClassWithDependsOnAndDoNotNotify");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public virtual void UsingPublicFieldThroughParameter()
    {
        var classWithPublicField = testResult.GetInstance("ClassWithPublicField");
        var classUsingPublicFieldThroughParameter = testResult.GetInstance("ClassUsingPublicFieldThroughParameter");
        classUsingPublicFieldThroughParameter.Write(classWithPublicField);
    }

    [Fact]
    public virtual void Equality()
    {
        var instance = testResult.GetInstance("ClassEquality");
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

    [Fact]
    public virtual void WithCompilerGeneratedAttribute()
    {
        var instance = testResult.GetInstance("ClassWithCompilerGeneratedAttribute");
        EventTester.TestPropertyNotCalled(instance);
    }

    [Fact]
    public virtual void WithGeneratedCodeAttribute()
    {
        var instance = testResult.GetInstance("ClassWithGeneratedCodeAttribute");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void NoBackingNoEqualityField()
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
        Assert.True(eventCalled);
    }

    [Fact]
    public virtual void NoBackingEqualityField()
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
        Assert.True(eventCalled);
    }

    [Fact]
    public virtual void WithFieldFromOtherClass()
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
        Assert.True(eventCalled);
    }

    [Fact]
    public virtual void WithIndexerClass()
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
        Assert.Equal("aString", instance[4]);
        instance.Property1 = "aString2";
        Assert.True(eventCalled);
    }

    [Fact]
    public virtual void WithOnceRemovedINotify()
    {
        var instance = testResult.GetInstance("ClassWithOnceRemovedINotify");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void WithBranchingReturn1()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void WithBranchingReturn2True()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void WithBranchingReturn2False()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void ClassWithBranchingReturnAndNoFieldTrue()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void ClassWithBranchingReturnAndNoFieldFalse()
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

        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void WithBranchingAndBeforeAfterReturn()
    {
        var instance = testResult.GetInstance("ClassWithBranchingReturnAndBefore");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void WithGeneric()
    {
        var instance = testResult.GetInstance("ClassWithGenericChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void GenericChildWithProperty()
    {
        var instance = testResult.GetInstance("GenericChildWithProperty.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void GenericBaseWithProperty()
    {
        var instance = testResult.GetInstance("GenericBaseWithProperty.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void GenericChildWithPropertyBefore()
    {
        var instance = testResult.GetInstance("GenericChildWithPropertyBefore.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void GenericBaseWithPropertyBefore()
    {
        var instance = testResult.GetInstance("GenericBaseWithPropertyBefore.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void Nested()
    {
        var instance1 = testResult.GetInstance("ClassWithNested+ClassNested");
        EventTester.TestProperty(instance1, false);
        var instance2 = testResult.GetInstance("ClassWithNested+ClassNested+ClassNestedNested");
        EventTester.TestProperty(instance2, false);
    }


    [Fact]
    public virtual void AlreadyHasNotification()
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

        Assert.True(property1EventCalled);
        Assert.True(property2EventCalled);
        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
        Assert.False(property2EventCalled);
    }

    [Fact]
    public virtual void AlreadyHasSingleNotification()
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

        Assert.True(property1EventCalled);
        Assert.True(property2EventCalled);
        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
        Assert.False(property2EventCalled);
    }

    [Fact]
    public virtual void AlreadyHasSingleNotificationDiffParamLocation()
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

        Assert.Equal(1, callCount);
        callCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.Equal(0, callCount);
    }

    [Fact]
    public virtual void AlreadyHasSingleNotificationDiffSignature()
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

        Assert.Equal(1, callCount);
        callCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.Equal(0, callCount);
    }

    [Fact]
    public virtual void WithBeforeAfterImplementation()
    {
        var instance = testResult.GetInstance("ClassWithBeforeImplementation");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public virtual void WithBoolPropUsingStringProp()
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

        Assert.True(boolPropertyCalled);
        Assert.True(stringPropertyCalled);
        Assert.True(stringComparePropertyCalled);

        boolPropertyCalled = false;
        stringPropertyCalled = false;
        stringComparePropertyCalled = false;
        instance.StringProperty = "notMagicString";

        Assert.False(boolPropertyCalled);
        Assert.True(stringPropertyCalled);
        Assert.True(stringComparePropertyCalled);
    }

    [Fact]
    public virtual void WithBeforeAndSimpleImplementation()
    {
        var instance = testResult.GetInstance("ClassWithBeforeAndSimpleImplementation");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public virtual void HierarchyBeforeAndSimple()
    {
        var instance = testResult.GetInstance("HierarchyBeforeAndSimple.ClassChild");
        EventTester.TestProperty(instance, false);
        Assert.True(instance.BeforeCalled);
    }

    [Fact]
    public virtual void WithPropertyChangingArgImplementation()
    {
        var instance = testResult.GetInstance("ClassWithPropertyChangingArgImplementation");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public virtual void WithCustomPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithCustomPropertyChanging");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void WithExplicitPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithExplicitPropertyChanging");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void DependsOn()
    {
        var instance = testResult.GetInstance("ClassDependsOn");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public virtual void WithNotifyInBase()
    {
        var instance = testResult.GetInstance("ClassWithNotifyInBase");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public virtual void Child1()
    {
        var instance = testResult.GetInstance("ComplexHierarchy.ClassChild1");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void Child2()
    {
        var instance = testResult.GetInstance("ComplexHierarchy.ClassChild2");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void Child3()
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

        Assert.True(property1EventCalled);
        Assert.True(property2EventCalled);
    }

    [Fact]
    public virtual void WithLogicInSet()
    {
        var instance = testResult.GetInstance("ClassWithLogicInSet");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void WithOwnImplementation()
    {
        var instance = testResult.GetInstance("ClassWithOwnImplementation");
        EventTester.TestProperty(instance, false);
        Assert.True(instance.BaseNotifyCalled);
    }


    [Fact]
    public virtual void WithOnChangedAndOnPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithOnChangedAndOnPropertyChanging");
        Assert.Equal(0, instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.Equal(1, instance.OnProperty1ChangingCalled);
    }


    [Fact]
    public virtual void WithOnChangedAndNoOnPropertyChanging()
    {
        var instance = testResult.GetInstance("ClassWithOnChangedAndNoOnPropertyChanging");
        Assert.Equal(0, instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.Equal(1, instance.OnProperty1ChangingCalled);
    }

    [Fact]
    public void ReactiveUI()
    {
        var instance = testResult.GetInstance("ClassReactiveUI");
        EventTester.TestProperty(instance, false);
        Assert.True(instance.BaseNotifyCalled);
    }

    [Fact]
    public virtual void WithOnChanging()
    {
        var instance = testResult.GetInstance("ClassWithOnChanging");
        Assert.False(instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.True(instance.OnProperty1ChangingCalled);
    }

    [Fact]
    public virtual void WithGenericAndLambda()
    {
        var instance = testResult.GetInstance("ClassWithGenericAndLambdaImp");
        EventTester.TestProperty(instance, false);
    }


    [Fact]
    public virtual void WithOnChangingBefore()
    {
        var instance = testResult.GetInstance("ClassWithOnChangingBefore");
        Assert.False(instance.OnProperty1ChangingCalled);
        EventTester.TestProperty(instance, false);
        Assert.True(instance.OnProperty1ChangingCalled);
    }


    [Fact]
    public virtual void TransitiveDependencies()
    {
        var propertyNames = new List<string>();
        var instance = testResult.GetInstance("TransitiveDependencies");
        ((INotifyPropertyChanging)instance).PropertyChanging += (sender, x) => propertyNames.Add(x.PropertyName);
        instance.My = "s";
        Assert.Contains("My", propertyNames);
        Assert.Contains("MyA", propertyNames);
        Assert.Contains("MyAB", propertyNames);
        Assert.Contains("MyABC", propertyNames);

    }

    [Fact]
    public virtual void CircularProperties()
    {
        var instance = testResult.GetInstance("ClassCircularProperties");
        instance.Self = "s";
        instance.PropertyA1 = "s";
        instance.PropertyA2 = "s";
        instance.PropertyB1 = "s";
        instance.PropertyB2 = "s";

    }

    [Fact]
    public virtual void WithPropertyImpOfAbstractProperty()
    {
        var instance = testResult.GetInstance("ClassWithPropertyImp");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public virtual void EqualityWithDouble()
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

        Assert.True(property1EventCalled);
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = 2d;
        Assert.False(property1EventCalled);
    }

    [Fact]
    public virtual void EqualityWithStruct()
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
        Assert.True(property1EventCalled);
    }

    [Fact]
    public virtual void EqualityWithStructOverload()
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

        Assert.True(property1EventCalled);
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = property1;
        Assert.False(property1EventCalled);
    }

    [Fact]
    public void ClassWithNullableBackingField()
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
        Assert.True(isFlagEventCalled);

        isFlagEventCalled = false;
        instance.IsFlag = true;
        Assert.False(isFlagEventCalled);
    }

#if NETFRAMEWORK
    [Fact]
    public async Task ClassWithNullableBackingFieldIl()
    {
        using var file = new PEFile(testResult.AssemblyPath);
        var property = new PropertyToDisassemble(file, "ClassWithNullableBackingField", "IsFlag", PropertyParts.Setter);

        await Verifier.Verify(property).UniqueForAssemblyConfiguration();
    }

    [Fact]
    public async Task ClassWithNullableAutoPropertyIl()
    {
        using var file = new PEFile(testResult.AssemblyPath);
        var property = new PropertyToDisassemble(file, "ClassWithNullableAutoProperty", "IsFlag", PropertyParts.Setter);

        await Verifier.Verify(property).UniqueForAssemblyConfiguration();
    }
#endif // NETFRAMEWORK
}
