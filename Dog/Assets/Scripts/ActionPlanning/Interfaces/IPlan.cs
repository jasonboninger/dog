using Assets.Scripts.ActionPlanning.Enums;
using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IPlan<TState, TAction>
	{
		EPlanningOutcome Outcome { get; }
		IReadOnlyList<IStep<TState, TAction>> Steps { get; }
		int Cycles { get; }
		float Cost { get; }
	}
}
