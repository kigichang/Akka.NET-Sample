using System;

using Akka;
using Akka.Actor;
using PenguinJoke.Answer;
using PenguinJoke.Question;
using PenguinJoke.Role;




namespace PenguinJoke
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("PenguinJoke");

			// 記者 Actor
			var reporter = system.ActorOf(Reporter.Props());

			// 10 隻 企鵝，只後一隻是東東
			IActorRef[] penguins = new IActorRef[10];

			int i = 1;

			for (; i <= 9; i++)
			{
				penguins[i - 1] = system.ActorOf(Penguin.Props($"Penguin {i}"));
			}

			penguins[i - 1] = system.ActorOf(DongDong.Props());

			// 記者詢問每隻企鵝的興趣是什麼
			foreach (IActorRef penguin in penguins)
			{
				penguin.Tell(Interest.Instance, reporter);
			}


			Console.ReadLine();
			Console.WriteLine("End!!!");
		}
	}
}
