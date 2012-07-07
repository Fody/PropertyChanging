using System.ComponentModel;

namespace HierachyBeforeAndSimple
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