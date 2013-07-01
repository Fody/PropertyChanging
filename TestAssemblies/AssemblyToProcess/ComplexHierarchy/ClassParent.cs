using System.ComponentModel;

namespace ComplexHierarchy
{
    public class ClassParent: INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
    }
}