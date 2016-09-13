using System;

using Akka;
using Akka.Actor;
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

		public Penguin(string name)
		{
			Name = name;
			Receive<Interest>(interest =>
			{
				Console.WriteLine($"{Name} receive Interest!!");
				Sender.Tell(new Three(Name, "吃飯", "睡覺", "打東東"), Self);

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
				Sender.Tell(new Two(Name, "吃飯", "睡覺"), Self);
			});

			Receive<Why>(why =>
			{
				Console.WriteLine($"{Name} receive Why!!");
				Sender.Tell(new Because(Name, "我就是東東"), Self);
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

}

