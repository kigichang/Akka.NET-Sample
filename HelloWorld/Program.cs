using System;

using Akka;
using Akka.Actor;

namespace HelloWorld
{
	/**
	 * An Immutable Message
	 */
	public sealed class ToWhom
	{
		public string Name { get; private set; }

		public ToWhom(string name)
		{
			Name = name;
		}

	}

	public sealed class Response
	{
		public string Message { get; private set; }

		public Response(string message)
		{
			Message = message;
		}

	}

	public class MyActor : ReceiveActor
	{
		public MyActor()
		{
			Receive<ToWhom>(whom => Sender.Tell(new Response($"Hi {whom.Name}, Hello World")));
		}


		public static Props Prpos()
		{
			return Akka.Actor.Props.Create<MyActor>();
		}
	}

	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("HelloWorld");

			var actor = system.ActorOf(MyActor.Prpos());

			Console.WriteLine($"Actor Path: {actor.Path}");

			var actor2 = system.ActorSelection(actor.Path);

			var inbox = Inbox.Create(system);

			inbox.Send(actor, new ToWhom("小明"));

			var resp = (Response)inbox.Receive();
			Console.WriteLine($"receive message: {resp.Message}");

			var resp2 = (Response)actor2.Ask(new ToWhom("小華"), TimeSpan.FromMinutes(5.0)).Result;
			Console.WriteLine($"receive message: {resp2.Message}");

			actor.Tell(new ToWhom("小黃"), inbox.Receiver);
			var resp3 = (Response)inbox.ReceiveAsync(TimeSpan.FromSeconds(5.0)).Result;
			Console.WriteLine($"receive message: {resp3.Message}");

			Console.ReadLine();
			Console.WriteLine("End !!!");
		}
	}
}
