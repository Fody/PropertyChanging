using System.ComponentModel;

namespace HierarchyBeforeAndSimple
{
    public class ClassBase : INotifyPropertyChanging
	{

		public event PropertyChangingEventHandler PropertyChanging;

		public void OnPropertyChanging(string propertyName)
		{
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
		}

	}
}