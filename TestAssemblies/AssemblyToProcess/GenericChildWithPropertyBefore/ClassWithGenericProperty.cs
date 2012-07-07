using System.ComponentModel;

namespace GenericChildWithPropertyBefore
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName, object before)
        {
            var propertyChanging = PropertyChanging;
            if (propertyChanging != null)
            {
                propertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

    }
}