using System;
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
}

