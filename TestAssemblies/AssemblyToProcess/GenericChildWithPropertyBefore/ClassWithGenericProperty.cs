using System.ComponentModel;

namespace GenericChildWithPropertyBefore
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName, object before)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

    }
}