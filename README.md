[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/PropertyChanging.Fody.svg?style=flat)](https://www.nuget.org/packages/PropertyChanging.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/PropertyChanging/master/Icons/package_icon.png)

Injects [INotifyPropertyChanging](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanging.aspx) code into properties at compile time.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)


## The nuget package

https://nuget.org/packages/PropertyChanging.Fody/

    PM> Install-Package PropertyChanging.Fody


### Your Code

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


### What gets compiled

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


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)


## Contributors

 * [Cameron MacFarland](https://github.com/distantcam)
 * [Simon Cropp](https://github.com/simoncropp)


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
