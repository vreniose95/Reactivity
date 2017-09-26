using System;

namespace Reactivity.Exceptions
{
	public class ReactionExecutionException : Exception
	{
		public ReactionExecutionException()
		{
		}

		public ReactionExecutionException(string message) : base(message)
		{
		}

		public ReactionExecutionException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
