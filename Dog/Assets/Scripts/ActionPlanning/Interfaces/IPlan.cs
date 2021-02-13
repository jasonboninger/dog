using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IPlan<TState, TAction>
	{
		IReadOnlyList<IStep<TState, TAction>> Steps { get; }
		int Cycles { get; }
	}
}
