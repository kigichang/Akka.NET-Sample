using System;
using Akka;
using Akka.Actor;

namespace DeployNode
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Start node");
			var system = ActorSystem.Create("DepolyNode");

			Console.ReadLine();
			Console.WriteLine("End!!!");
		}
	}
}
