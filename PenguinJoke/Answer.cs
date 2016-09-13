using System;
using Akka.Actor;

namespace PenguinJoke.Answer
{
	/// <summary>
	/// 兩個興趣的回覆
	/// </summary>
	public class Two
	{
		public string Name { get; private set; }

		public string Interest1 { get; private set; }

		public string Interest2 { get; private set; }

		public Two(string name, string interest1, string interest2)
		{
			Name = name;
			Interest1 = interest1;
			Interest2 = interest2;
		}

		public override string ToString()
		{
			return $"{Name}: {Interest1}, {Interest2}.";
		}
	}

	/// <summary>
	/// 三個興趣的回覆
	/// </summary>
	public sealed class Three : Two
	{
		public string Interest3 { get; private set; }

		public Three(string name, string interest1, string interest2, string interest3) : base(name, interest1, interest2)
		{
			Interest3 = interest3;
		}


		public override string ToString()
		{
			return $"{Name}: {Interest1}, {Interest2}, {Interest3}";
		}
	}

	/// <summary>
	/// 為什麼的回覆
	/// </summary>
	public sealed class Because
	{
		public string Name { get; private set; }

		public string Message { get; private set; }

		public Because(string name, string message)
		{
			Name = name;
			Message = message;
		}

		public override string ToString()
		{
			return $"{Name}: {Message}";
		}
	}

	/// <summary>
	/// 回覆被詢問幾次
	/// </summary>
	public sealed class Counter
	{
		public string Name { get; private set; }

		public int Count { get; private set; }

		public Counter(string name, int count)
		{
			Name = name;
			Count = count;
		}

		public override string ToString()
		{
			return $"{Name} is queried {Count}.";
		}
	}


	public sealed class DontHitMe
	{
		public string Name { get; private set; }

		public int Hits { get; private set; }

		public DontHitMe(string name, int hits)
		{
			Name = name;
			Hits = hits;
		}

		public override string ToString()
		{
			return $"{Name} is hit {Hits}";
		}
	}


	public sealed class PenguinReady
	{
		public IActorRef Penguin { get; private set; }

		public PenguinReady(IActorRef penguin)
		{
			Penguin = penguin;
		}


		public override string ToString()
		{
			return $"{Penguin.Path} ready!!!";
		}
	}

}

