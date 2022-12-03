using System.ComponentModel;

public class ClassEqualityWithStructOverload : INotifyPropertyChanging
{
    public SimpleStruct Property1 { get; set; }

#pragma warning disable 660,661
    public struct SimpleStruct
#pragma warning restore 660,661
    {

        public int X ;
        public static bool operator ==(SimpleStruct left, SimpleStruct right)
        {
            return left.X == right.X;
        }

        public static bool operator !=(SimpleStruct left, SimpleStruct right)
        {
            return !(left == right);
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
}