using System;

using Akka;
using Akka.Actor;
using Akka.Routing;


using PenguinJoke.Answer;
using PenguinJoke.Question;

namespace PenguinJoke.Role
{
	/// <summary>
	/// 企鵝
	/// </summary>
	public class Penguin : ReceiveActor
	{
		public string Name { get; private set; }

		protected int count = 0;

		protected int hits = 0;


		public Penguin(string name)
		{
			Name = name;
			Receive<Interest>(interest =>
			{
				Console.WriteLine($"{Name} receive Interest!!");
				count += 1;
				Sender.Tell(new Three(Name, "吃飯", "睡覺", "打東東"), Self);
			});

			Receive<QueryCount>(query =>
			{
				Sender.Tell(new Counter(Name, count), Self);
			});

			Receive<Hit>(hit =>
			{
				hits += 1;
				Console.WriteLine($"{Name} got a Hit({hits})!!!");
				Sender.Tell(new DontHitMe(Name, hits), Self);
			});
		}

		public static Props Props(string name)
		{
			return Akka.Actor.Props.Create<Penguin>(() => new Penguin(name));
		}


		protected override void PreStart()
		{
			base.PreStart();
			Console.WriteLine($"{this} pre-start!!!");
		}

		protected override void PostStop()
		{
			base.PostStop();
			Console.WriteLine($"{this} post-stop!!!");
		}

		public override string ToString()
		{
			return $"Actor Name: [{Name}] Path: [{Self.Path}]";
		}
	}

	/// <summary>
	/// 叫東東的企鵝
	/// </summary>
	public class DongDong : Penguin
	{
		private void ChangeHandler()
		{
			Receive<Interest>(interest =>
			{
				Console.WriteLine($"{Name} receive Interest!!");
				count += 1;
				Sender.Tell(new Two(Name, "吃飯", "睡覺"), Self);
			});

			Receive<Why>(why =>
			{
				Console.WriteLine($"{Name} receive Why!!");
				Sender.Tell(new Because(Name, "我就是東東"), Self);
			});

			Receive<QueryCount>(query =>
			{
				Sender.Tell(new Counter(Name, count), Self);
			});

			Receive<Hit>(hit =>
			{
				hits += 1;
				Console.WriteLine($"{Name} got a Hit({hits})!!!");

				if (hits == 4)
				{
					throw new DontBotherMeException();
				}
				else if (hits == 6)
				{
					throw new ExplosionException();
				}
				else if (hits > 6)
				{
					throw new IamGodException();
				}
				else {
					Sender.Tell(new DontHitMe(Name, hits), Self);
				}

			});
		}

		public DongDong() : base("東東")
		{
			Become(ChangeHandler);
		}


		public static Props Props()
		{
			return Akka.Actor.Props.Create<DongDong>();
		}

	}

	/// <summary>
	/// 記者
	/// </summary>
	public class Reporter : ReceiveActor
	{

		public Reporter()
		{
			// 收到三個興趣的回覆
			Receive<Three>(three => Console.WriteLine(three));

			// 收到二個興趣的回覆，反問為什麼
			Receive<Two>(two =>
			{
				Console.WriteLine(two);
				Sender.Tell(Why.Instance, Self);
			});

			// 收到為什麼的回覆
			Receive<Because>(because => Console.WriteLine(because));

			Receive<Counter>(counter => Console.WriteLine(counter));

			Receive<DontHitMe>(hit => Console.WriteLine(hit));
		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create<Reporter>();
		}

		public override string ToString()
		{
			return $"Report Actor, Path: [{Self.Path}]";
		}

		protected override void PreStart()
		{
			base.PreStart();
			Console.WriteLine($"{this} pre-start!!!");
		}
	}


	public class PenguinRounter : ReceiveActor
	{
		private Router router = new Router(new RoundRobinRoutingLogic());

		public PenguinRounter()
		{
			Handler();
		}

		private void Handler()
		{
			Receive<PenguinReady>(ready =>
			{
				Context.Watch(ready.Penguin);
				router = router.AddRoutee(ready.Penguin);
				Console.WriteLine($"Penguin Router watch {ready.Penguin.Path}");
			});


			Receive<Terminated>(term =>
			{
				router = router.RemoveRoutee(term.ActorRef);
				Console.WriteLine($"Penguin Router remove {term.ActorRef.Path}");
			});


			Receive<Interest>(interest =>
			{
				router.Route(Interest.Instance, Sender);
			});

			Receive<QueryCount>(query =>
			{
				foreach (Routee actor in router.Routees)
				{
					actor.Send(QueryCount.Instance, Sender);
				}
			});

			Receive<Hit>(hit =>
			{
				router.Route(Hit.Instance, Sender);
			});

			Receive<KillOne>(kill =>
			{
				var enumerator = router.Routees.GetEnumerator();
				if (enumerator.MoveNext())
				{
					enumerator.Current.Send(PoisonPill.Instance, Self);
				}

			});

		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create<PenguinRounter>();
		}
	}

	public class PenguinKing : ReceiveActor
	{
		public int PenguinCount { get; private set; }

		private IActorRef Router;

		protected override SupervisorStrategy SupervisorStrategy()
		{
			return new OneForOneStrategy( //or AllForOneStrategy
				10,
				TimeSpan.FromSeconds(30),
				Decider.From(x =>
				{
					if (x is DontBotherMeException) return Directive.Resume;
					else if (x is ExplosionException) return Directive.Resume;
					else if (x is IamGodException) return Directive.Stop;
					else return Directive.Escalate;
				}));
		}

		public PenguinKing(int count, IActorRef router)
		{
			PenguinCount = count;
			Router = router;

			Handler();
			Deploy();
		}

		private void Deploy()
		{
			for (int i = 0; i < PenguinCount - 1; i++)
			{
				var penguin = Context.ActorOf(Penguin.Props($"Penguin {i+1}"), $"penguin-{i+1}");

				penguin.Tell(new Identify(penguin.Path), Self);
			}

			var dongdong = Context.ActorOf(DongDong.Props(), "penguin-dongdong");
			dongdong.Tell(new Identify(dongdong.Path), Self);

		}

		private void Handler()
		{
			Receive<ActorIdentity>(id =>
			{
				if (id.Subject == null)
				{
					Console.WriteLine($"{id.MessageId} Not Found!!!");
				}
				else {
					Console.WriteLine($"{id.MessageId} Found!!!");

					Router.Tell(new PenguinReady(id.Subject), Self);
				}
			});
		}


		protected override void PreStart()
		{
			base.PreStart();
			Console.WriteLine("Penguin King pre-start!!!");
		}

		public static Props Props(int count, IActorRef router)
		{
			return Akka.Actor.Props.Create<PenguinKing>(() => new PenguinKing(count, router));
		}

	}
}

