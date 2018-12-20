using System;
using System.Reactive;
using System.Reactive.Linq;
using EventStore.ClientAPI;

namespace Theta.Platform.Messaging.EventStore
{
	public static class EventStoreHelpers
	{
		public static IObservable<Unit> GetConnectedStream(IEventStoreConnection connection)
		{
			return Observable.FromEventPattern<ClientConnectionEventArgs>(
					handler => connection.Connected += handler,
					handler => connection.Connected -= handler)
				.Select(_ => Unit.Default);
		}

		public static IObservable<Unit> GetDisconnectedStream(IEventStoreConnection connection)
		{
			return Observable.FromEventPattern<ClientConnectionEventArgs>(
					handler => connection.Disconnected += handler,
					handler => connection.Disconnected -= handler)
				.Select(_ => Unit.Default);
		}

		public static IObservable<Unit> GetReconnectingStream(IEventStoreConnection connection)
		{
			return Observable.FromEventPattern<ClientReconnectingEventArgs>(
					handler => connection.Reconnecting += handler,
					handler => connection.Reconnecting -= handler)
				.Select(_ => Unit.Default);
		}

		public static IObservable<string> GetAuthFailedStream(IEventStoreConnection connection)
		{
			return Observable.FromEventPattern<ClientAuthenticationFailedEventArgs>(
					handler => connection.AuthenticationFailed += handler,
					handler => connection.AuthenticationFailed -= handler)
				.Select(ep => ep.EventArgs.Reason);
		}

		public static IObservable<Exception> GetErrorOccurredStream(IEventStoreConnection connection)
		{
			return Observable.FromEventPattern<ClientErrorEventArgs>(
					handler => connection.ErrorOccurred += handler,
					handler => connection.ErrorOccurred -= handler)
				.Select(ep => ep.EventArgs.Exception);
		}

		public static IObservable<string> GetClosedStream(IEventStoreConnection connection)
		{
			return Observable.FromEventPattern<ClientClosedEventArgs>(
					handler => connection.Closed += handler,
					handler => connection.Closed -= handler)
				.Select(ep => ep.EventArgs.Reason);
		}
	}
}