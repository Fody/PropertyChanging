using System.ComponentModel;

namespace AssemblyWithBase.MultiTypes
{
    public class BaseClass1<T,Z> : INotifyPropertyChanging
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