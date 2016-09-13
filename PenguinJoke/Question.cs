using System;
namespace PenguinJoke.Question
{
	/// <summary>
	/// 詢問興趣
	/// </summary>
	public sealed class Interest
	{
		private static readonly Interest instance = new Interest();

		private Interest() 
		{
		}

		public static Interest Instance
		{
			get
			{
				return instance;
			}
		}
	}

	/// <summary>
	/// 收到只有兩個興趣的回覆，反問為什麼
	/// </summary>
	public sealed class Why
	{
		private static readonly Why instance = new Why();

		private Why()
		{
		}

		public static Why Instance
		{
			get
			{
				return instance;
			}
		}
	}
}

