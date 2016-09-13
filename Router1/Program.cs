using System;

using Akka;
using Akka.Actor;
using Akka.Routing;

using PenguinJoke.Answer;
using PenguinJoke.Question;
using PenguinJoke.Role;

                 
namespace Router1
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("Router1");

			var router_props = Props.Create<DongDong>().WithRouter(new RoundRobinPool(5));

			var router = system.ActorOf(router_props, "dongdongs");

			var reporter = system.ActorOf(Reporter.Props());

			for (int i = 0; i < 100; i++)
			{
				router.Tell(Interest.Instance, reporter);
			}

			Console.ReadLine();
			Console.WriteLine("End !!!");
		}
	}
}
