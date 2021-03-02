using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IGoal<TState, TAction>
	where TState : class, IState<TState>, new()
	where TAction : IAction<TState>
	{
		IReadOnlyList<TAction> Actions { get; }
		
		bool IsAchieved(TState state);

		float EstimateProximity(TState state);
	}
}
