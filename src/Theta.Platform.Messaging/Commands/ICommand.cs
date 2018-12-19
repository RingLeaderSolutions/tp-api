namespace Theta.Platform.Messaging.Commands
{
    public class Command : ICommand
    {
        public string Type { get; set; }
    }

	public interface ICommand
	{
		string Type { get; }
	}
}