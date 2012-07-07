using System.ComponentModel;

namespace AssemblyWithBase.Simple
{
	public class BaseClass : INotifyPropertyChanging
	{
		public event PropertyChangingEventHandler PropertyChanging;
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