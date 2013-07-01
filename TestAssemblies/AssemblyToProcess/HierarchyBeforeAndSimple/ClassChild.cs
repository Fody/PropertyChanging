using PropertyChanging;

namespace HierarchyBeforeAndSimple
{
    public class ClassChild : ClassBase
    {

        public string Property1 { get; set; }

        [DoNotNotify]
        public bool BeforeCalled { get; set; }

        public void OnPropertyChanging(string propertyName, object before)
        {
            BeforeCalled = true;
            OnPropertyChanging(propertyName);
        }

    }
}