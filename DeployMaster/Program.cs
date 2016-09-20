using System;

using Akka;
using Akka.Actor;
using PenguinJoke.Role;
using PenguinJoke.Answer;
using PenguinJoke.Question;

namespace DeployMaster
{

	public class DeployReporter : Reporter
	{
		public DeployReporter()
		{
			Receive<PenguinReady>(ready =>
			{
				/* 當有企鵝準備好時，用 Watch 來追蹤。
				 * 當某個企鵝死掉時，Router 會收到 Terminated 訊息，可做後續的處理 
				 */
				Context.Watch(ready.Penguin);
				Console.WriteLine($"{ready.Penguin.Path} ready");
				ready.Penguin.Tell(Interest.Instance, Self);
			});


			Receive<Terminated>(term =>
			{
				Console.WriteLine($"{term.ActorRef.Path} terminated");
			});
		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create<DeployReporter>();
		}
	}


	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("DeployMaster");

			var reporter = system.ActorOf(DeployReporter.Props(), "reporter");

			var king = system.ActorOf(PenguinKing.Props(10, reporter), "penguin-king");

			Console.ReadLine();
			Console.WriteLine("End!!!");
		}
	}
}
