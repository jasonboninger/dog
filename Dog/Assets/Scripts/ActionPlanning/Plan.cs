using Assets.Scripts.ActionPlanning.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning
{
	partial class ActionPlanner<TState, TAction>
	{
		private class Plan : IPlan<TState, TAction>
		{
			public readonly List<Step> steps = new List<Step>();
			IReadOnlyList<IStep<TState, TAction>> IPlan<TState, TAction>.Steps => steps;

			public int cycles = 0;
			int IPlan<TState, TAction>.Cycles => cycles;
		}
	}
}
