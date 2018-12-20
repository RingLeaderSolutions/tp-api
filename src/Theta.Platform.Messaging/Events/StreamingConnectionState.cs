namespace Theta.Platform.Messaging.Events
{
	public enum StreamingConnectionState
	{
		Idle,
		Connecting,
		AuthenticationFailed,
		Connected,
		Reconnecting,
		Disconnected,
		Closed
	}
}