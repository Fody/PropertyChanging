using System.ComponentModel;

namespace AssemblyWithBase.GenericFromAbove
{
    public class BaseClass2 : BaseClass1<object>
    {
    }
    public class BaseClass3 : BaseClass2, INotifyPropertyChanging
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