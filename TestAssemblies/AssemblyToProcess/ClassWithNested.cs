using System.ComponentModel;

public class ClassWithNested
{

    public class ClassNested : INotifyPropertyChanging
    {
        public string Property1 { get; set; }

        public event PropertyChangingEventHandler PropertyChanging;

        public class ClassNestedNested : INotifyPropertyChanging
        {
            public string Property1 { get; set; }

            public event PropertyChangingEventHandler PropertyChanging;

        }
    }
}