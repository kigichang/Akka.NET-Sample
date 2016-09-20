using System;
using System.Configuration;

using Akka;
using Akka.Actor;
using Akka.Configuration;
using PenguinJoke.Role;

namespace LookupServer
{
	class MainClass
	{
		public static void Main(string[] args)
		{

			var system = ActorSystem.Create("LookupServer");


			var penguins = new IActorRef[10];

			int i = 1;

			for (; i <= 9; i++)
			{
				penguins[i - 1] = system.ActorOf(Penguin.Props($"penguin {i}"), $"penguin-{i}");

				Console.WriteLine($"PATH[{penguins[i - 1].Path}]");
			}

			penguins[i - 1] = system.ActorOf(DongDong.Props(), "dongdong");
			Console.WriteLine($"PATH[{penguins[i - 1].Path}]");

			Console.ReadLine();
			Console.WriteLine("LookupServer End!!!!");
		}
	}
}
