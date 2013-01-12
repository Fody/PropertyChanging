## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Injects [INotifyPropertyChanging](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanging.aspx) code into properties at compile time.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget package http://nuget.org/packages/PropertyChanging.Fody 

### Your Code

    public class Person : INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanged;

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

## More Info

* [AccessBeforeValue](https://github.com/Fody/PropertyChanging/wiki/AccessBeforeValue)
* [Attributes](https://github.com/Fody/PropertyChanging/wiki/Attributes)
* [EqualityChecking](https://github.com/Fody/PropertyChanging/wiki/EqualityChecking)
* [EventInvokerSelectionInjection](https://github.com/Fody/PropertyChanging/wiki/EventInvokerSelectionInjection)
* [ExampleUsage](https://github.com/Fody/PropertyChanging/wiki/ExampleUsage)
* [NotificationInterception](https://github.com/Fody/PropertyChanging/wiki/NotificationInterception)
* [On_PropertyName_Changing](https://github.com/Fody/PropertyChanging/wiki/On_PropertyName_Changing)
* [PropertyDependencies](https://github.com/Fody/PropertyChanging/wiki/PropertyDependencies)
* [WeavingTaskOptions](https://github.com/Fody/PropertyChanging/wiki/WeavingTaskOptions)
* [WeavingWithoutAddingAReference](https://github.com/Fody/PropertyChanging/wiki/WeavingWithoutAddingAReference)
