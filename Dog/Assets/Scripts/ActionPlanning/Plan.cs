﻿using Assets.Scripts.ActionPlanning.Enums;
using Assets.Scripts.ActionPlanning.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning
{
	partial class ActionPlanner<TState, TAction>
	{
		private class Plan : IPlan<TState, TAction>
		{
			public EPlanningOutcome outcome = EPlanningOutcome.NotPlanned;
			EPlanningOutcome IPlan<TState, TAction>.Outcome => outcome;

			public readonly List<Step> steps = new List<Step>();
			IReadOnlyList<IStep<TState, TAction>> IPlan<TState, TAction>.Steps => steps;

			public int cycles = 0;
			int IPlan<TState, TAction>.Cycles => cycles;

			public float cost = 0;
			float IPlan<TState, TAction>.Cost => cost;
		}
	}
}
