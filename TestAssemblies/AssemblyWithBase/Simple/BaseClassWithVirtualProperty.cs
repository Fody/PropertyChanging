namespace AssemblyWithBase.Simple
{
    using System.ComponentModel;

    public class BaseClassWithVirtualProperty : INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;

        public virtual string Property1 { get; set; }

        public virtual void OnPropertyChanging(string text1)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(text1));
            }
        }
    }
}
