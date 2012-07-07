using System.ComponentModel;

namespace ComplexHierachy
{
    public class ClassParent: INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
    }
}