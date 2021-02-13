using Assets.Scripts.ActionPlanning.Interfaces;

namespace Assets.Scripts.ActionPlanning
{
	partial class ActionPlanner<TState, TAction>
	{
		private class Step : IStep<TState, TAction>
		{
			public TState from;
			TState IStep<TState, TAction>.From => from;
			
			public TState to;
			TState IStep<TState, TAction>.To => to;
			
			public TAction action;
			TAction IStep<TState, TAction>.Action => action;
		}
	}
}
