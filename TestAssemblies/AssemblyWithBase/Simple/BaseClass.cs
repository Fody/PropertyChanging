using System.ComponentModel;

namespace AssemblyWithBase.Simple
{
	public class BaseClass : INotifyPropertyChanging
	{
		public event PropertyChangingEventHandler PropertyChanging;
		public virtual void OnPropertyChanging(string text1)
		{
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(text1));
		}

	}
}