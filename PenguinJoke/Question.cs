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

	/// <summary>
	/// 詢問被問了幾次
	/// </summary>
	public sealed class QueryCount
	{
		private static readonly QueryCount instance = new QueryCount();

		private QueryCount()
		{
		}

		public static QueryCount Instance
		{
			get
			{
				return instance;
			}
		}
	}

	/// <summary>
	/// 殺掉其中一隻企鵝
	/// </summary>
	public sealed class KillOne
	{
		private static readonly KillOne instance = new KillOne();

		private KillOne()
		{
		}

		public static KillOne Instance
		{
			get
			{
				return instance;
			}
		}
	}

	/// <summary>
	/// 打企鵝
	/// </summary>
	public sealed class Hit
	{
		private static readonly Hit instance = new Hit();

		private Hit()
		{
		}

		public static Hit Instance
		{
			get
			{
				return instance;
			}
		}
	}

}

