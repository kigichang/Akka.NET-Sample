﻿using System;

using Akka;
using Akka.Actor;

using PenguinJoke.Answer;
using PenguinJoke.Question;
using PenguinJoke.Role;

namespace Router2
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("Router2");

			var router = system.ActorOf(PenguinRounter.Props(), "penguin-router");

			var king = system.ActorOf(PenguinKing.Props(10, router), "penguin-king");

			var reporter = system.ActorOf(Reporter.Props(), "reporter");

			Console.WriteLine("Press Enter to start sending message!!!");
			Console.ReadLine();

			for (int i = 0; i < 100; i++)
			{
				router.Tell(Interest.Instance, reporter);
			}

			Console.ReadLine();
			router.Tell(QueryCount.Instance, reporter);

			Console.ReadLine();
			router.Tell(KillOne.Instance, reporter);

			Console.ReadLine();
			Console.WriteLine("End !!!");
		}
	}
}
