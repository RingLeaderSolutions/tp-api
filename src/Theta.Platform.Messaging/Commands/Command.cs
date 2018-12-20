namespace Theta.Platform.Messaging.Commands
{
	public abstract class Command : ICommand
	{
		public string Type => this.GetType().Name;
	}
}