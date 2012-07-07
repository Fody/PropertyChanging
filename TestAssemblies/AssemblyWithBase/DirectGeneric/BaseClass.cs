using System.ComponentModel;

namespace AssemblyWithBase.DirectGeneric
{
    public class BaseClass<T> : INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public virtual void OnPropertyChanging(string propertyName)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}