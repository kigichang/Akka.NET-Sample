using System;

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

			/**
			 * 以下註冊處理訊息的函式
			 */
			Receive<Interest>(interest =>
			{
				/* 處理詢問興趣 */
				Console.WriteLine($"{Name} receive Interest!!");
				count += 1;
				Sender.Tell(new Three(Name, "吃飯", "睡覺", "打東東"), Self);
			});

			Receive<QueryCount>(query =>
			{
				/* 處理被詢問的次數訊息，回覆被詢問的次數 */
				Sender.Tell(new Counter(Name, count), Self);
			});

			Receive<Hit>(hit =>
			{
				/* 處理被打的訊息，並回覆被打的次數 */
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

				/* 以下模擬發生不同的 Exception 情形，用來測試 Supervisor Strategy */
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

			// 收到企鵝回覆被詢問的次數
			Receive<Counter>(counter => Console.WriteLine(counter));

			// 收到企鵝回覆被打的次數
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

	/// <summary>
	/// 企鵝 Router, 負責派送訊息
	/// </summary>
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
				/* 當有企鵝準備好時，用 Watch 來追蹤。
				 * 當某個企鵝死掉時，Router 會收到 Terminated 訊息，可做後續的處理 
				 */
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
					/* 停用第一隻企鵝。傳送 PoisonPill 可以停用某個 Actor */
					enumerator.Current.Send(PoisonPill.Instance, Self);
				}

			});
		}

		public static Props Props()
		{
			return Akka.Actor.Props.Create<PenguinRounter>();
		}
	}

	/// <summary>
	/// 企鵝王，負責產生子企鵝，並管理
	/// </summary>
	public class PenguinKing : ReceiveActor
	{
		public int PenguinCount { get; private set; }

		private int Count = 0;

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
			Console.WriteLine($"Router is {router.Path}");
			Handler();
			Deploy();
		}

		/**
		 * 產生子企鵝 (actor)
		 */
		private void Deploy()
		{
			for (int i = 0; i < PenguinCount - 1; i++)
			{
				var penguin = Context.ActorOf(Penguin.Props($"Penguin {i+1}"), $"penguin-{i+1}");

				/* 使用 Identify 訊息，來追蹤子企鵝 (actor) 是否可以使用 */
				penguin.Tell(new Identify(penguin.Path), Self);
			}

			var dongdong = Context.ActorOf(DongDong.Props(), "penguin-dongdong");
			dongdong.Tell(new Identify(dongdong.Path), Self);

			Context.SetReceiveTimeout(TimeSpan.FromSeconds(5.0));
		}

		private void Handler()
		{
			/* 對 Actor 傳送 Identify 訊息，
			 * 如果該 Actor 可以被使用，則會回傳 ActorIdentity 訊息。
			 */
			Receive<ActorIdentity>(id =>
			{
				Count += 1;
				if (Count == PenguinCount)
				{
					Context.SetReceiveTimeout(null);
				}


				if (id.Subject == null)
				{
					Console.WriteLine($"{id.MessageId} Not Found!!!");
				}
				else {
					Console.WriteLine($"{id.MessageId} Found and Tell Router Penguin Ready!!!");
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

