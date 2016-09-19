using System;

using Akka.Actor;
using PenguinJoke.Role;
using PenguinJoke.Question;

namespace Supervisor
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("Supervisor");

			var router = system.ActorOf(PenguinRounter.Props(), "penguin-router");

			var king = system.ActorOf(PenguinKing.Props(10, router), "penguin-king");

			var reporter = system.ActorOf(Reporter.Props(), "reporter");

			Console.WriteLine("Press Enter to Start");
			Console.ReadLine();

			for (int i = 0; i < 100; i++)
			{
				router.Tell(Hit.Instance, reporter);
			}

			Console.ReadLine();
			Console.WriteLine("End!!!");
		}
	}
}
