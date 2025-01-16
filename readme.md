# <img src="/package_icon.png" height="30px"> PropertyChanging.Fody

[![NuGet Status](https://img.shields.io/nuget/v/PropertyChanging.Fody.svg)](https://www.nuget.org/packages/PropertyChanging.Fody/)

Injects [INotifyPropertyChanging](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanging.aspx) code into properties at compile time.

**See [Milestones](../../milestones?state=closed) for release notes.**


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [PropertyChanging.Fody NuGet package](https://nuget.org/packages/PropertyChanging.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package PropertyChanging.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Your Code

```csharp
[ImplementPropertyChanging]
public class Person
{
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }

    public string FullName
    {
        get
        {
            return string.Format("{0} {1}", GivenNames, FamilyName);
        }
    }
}
```


### What gets compiled

```csharp
public class Person : INotifyPropertyChanging
{
    public event PropertyChangingEventHandler PropertyChanging;

    string givenNames;
    public string GivenNames
    {
        get { return givenNames; }
        set
        {
            if (value != givenNames)
            {
                OnPropertyChanging("GivenNames");
                OnPropertyChanging("FullName");
                givenNames = value;
            }
        }
    }

    string familyName;
    public string FamilyName
    {
        get { return familyName; }
        set 
        {
            if (value != familyName)
            {
                OnPropertyChanging("FamilyName");
                OnPropertyChanging("FullName");
                familyName = value;
            }
        }
    }

    public string FullName
    {
        get
        {
            return string.Format("{0} {1}", GivenNames, FamilyName);
        }
    }

    public virtual void OnPropertyChanging(string propertyName)
    {
        var propertyChanging = PropertyChanging;
        if (propertyChanging != null)
        {
            propertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
```


## More Info

* [AccessBeforeValue](https://github.com/Fody/PropertyChanging/wiki/AccessBeforeValue)
* [Attributes](https://github.com/Fody/PropertyChanging/wiki/Attributes)
* [EqualityChecking](https://github.com/Fody/PropertyChanging/wiki/EqualityChecking)
* [EventInvokerSelectionInjection](https://github.com/Fody/PropertyChanging/wiki/EventInvokerSelectionInjection)
* [ExampleUsage](https://github.com/Fody/PropertyChanging/wiki/ExampleUsage)
* [NotificationInterception](https://github.com/Fody/PropertyChanging/wiki/NotificationInterception)
* [On_PropertyName_Changing](https://github.com/Fody/PropertyChanging/wiki/On_PropertyName_Changing)
* [PropertyDependencies](https://github.com/Fody/PropertyChanging/wiki/PropertyDependencies)
* [Options](https://github.com/Fody/PropertyChanging/wiki/Options)
* [WeavingWithoutAddingAReference](https://github.com/Fody/PropertyChanging/wiki/WeavingWithoutAddingAReference)


## Icon

Icon courtesy of [The Noun Project](https://thenounproject.com)