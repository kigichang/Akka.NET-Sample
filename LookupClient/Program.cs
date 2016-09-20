using System;
using Akka.Actor;

namespace LookupClient
{
	public class LookupReporter : PenguinJoke.Role.Reporter
	{
		private int Count = 0;
		private int Size = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:LookupClient.LookupReporter"/> class.
		/// </summary>
		/// <param name="path">遠端企鵝 (Actor) 的 URL</param>
		public LookupReporter(string[] path)
		{
			Size = path.Length;
			TrackingReceive();
			SendIdentifyRequest(path);
		}

		public static Props Props(string[] path)
		{
			return Akka.Actor.Props.Create<LookupReporter>(() => new LookupReporter(path));
		}


		private void TrackingReceive()
		{
			/* 當遠端的 Actor 是 ready 時，ActorIdentity.Subject 不會是 null */
			Receive<ActorIdentity>(id =>
			{
				Count += 1;
				if (Count == Size)
				{
					Context.SetReceiveTimeout(null);
				}

				if (id.Subject == null)
				{
					Console.WriteLine($"{id.MessageId} Not Found");
				}
				else {
					Console.WriteLine($"{id.MessageId} Found");
					Context.Watch(id.Subject);

					id.Subject.Tell(PenguinJoke.Question.Interest.Instance, Self);
				}
			});

			Receive<Terminated>(term =>
			{
				Console.WriteLine($"{term.ActorRef.Path} is terminated");
			});
		}

		/// <summary>
		/// 透過遠端企鵝的 URL 來詢問遠端的企鵝是否 ready
		/// </summary>
		/// <param name="path">Path.</param>
		private void SendIdentifyRequest(string[] path)
		{
			foreach (string p in path)
			{
				Console.WriteLine($"Tell {p}");
				Context.ActorSelection(p).Tell(new Identify(p), Self);
			}

			Context.SetReceiveTimeout(TimeSpan.FromSeconds(5.0));
		}
	}
	
	class MainClass
	{
		public static void Main(string[] args)
		{
			var system = ActorSystem.Create("LookupClient");

			var remote_path = @"akka.tcp://LookupServer@127.0.0.1:2552/user";

			var penguins = new string[10];

			for (int i = 1; i <= 9; i++)
			{
				penguins[i - 1] = $"{remote_path}/penguin-{i}";
			}

			penguins[9] = $"{remote_path}/dongdong";


			var reporter = system.ActorOf(LookupReporter.Props(penguins), "reporter");


			Console.ReadLine();
			Console.WriteLine("End!!!");
		}
	}



}
