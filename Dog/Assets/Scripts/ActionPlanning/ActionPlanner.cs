using Assets.Scripts.ActionPlanning.Enums;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Utilities;
using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning
{
	public partial class ActionPlanner<TState, TAction>
	where TState : class, IState<TState>, new()
	where TAction : IAction<TState>
	{
		private readonly List<TAction> _actions = new List<TAction>();
		public IReadOnlyList<TAction> Actions => _actions;

		private readonly Steps _steps;
		private readonly Reusable<Plan> _plans;
		private readonly Explorer _explorer;

		private readonly States _states = new States();

		public ActionPlanner(int cyclesLimit = 1000)
		{
			// Set steps
			_steps = new Steps(_states);
			// Set plans
			_plans = new Reusable<Plan>(_ResetPlan);
			// Set explorer
			_explorer = new Explorer(cyclesLimit, _steps, _states, _actions);
		}

		public TState GetState()
		{
			// Return state
			return _states.Get();
		}

		public void ReleaseState(TState state)
		{
			// Release state
			_states.Release(state);
		}

		public IPlan<TState, TAction> GetPlan()
		{
			// Return plan
			return _plans.GetOrCreate();
		}

		public void PopulatePlan(IPlan<TState, TAction> plan, TState state, IGoal<TState, TAction> goal)
		{
			// Get converted plan
			var planConverted = (Plan)plan;
			// Reset plan
			_ResetPlan(planConverted);
			// Populate plan
			_explorer.PopulatePlan(planConverted, state, goal);
		}
		public void PopulatePlan(IPlan<TState, TAction> plan, TAction action)
		{
			// Get converted plan
			var planConverted = (Plan)plan;
			// Reset plan
			_ResetPlan(planConverted);
			// Set outcome
			planConverted.outcome = EPlanningOutcome.Success;
			// Get state
			var state = _states.Get();
			// Create step
			var step = _steps.Get(state, state, action);
			// Add step
			planConverted.steps.Add(step);
			// Set cycles
			planConverted.cycles = 0;
			// Set cost
			planConverted.cost = 0;
		}

		public void ReleasePlan(IPlan<TState, TAction> plan)
		{
			// Return plan to pool
			_plans.ReturnToPool((Plan)plan);
		}

		public void AddAction(TAction action)
		{
			// Add action
			_actions.Add(action);
		}

		public void RemoveAction(TAction action)
		{
			// Remove action
			_actions.Remove(action);
		}

		private void _ResetPlan(Plan plan)
		{
			// Set outcome
			plan.outcome = EPlanningOutcome.NotPlanned;
			// Get reusable steps
			var stepsReusable = _steps;
			// Get steps
			var steps = plan.steps;
			// Run through steps
			for (int i = steps.Count - 1; i >= 0; i--)
			{
				// Release step
				stepsReusable.Release(steps[i]);
				// Remove step
				steps.RemoveAt(i);
			}
			// Clear cycles
			plan.cycles = 0;
			// Clear cost
			plan.cost = 0;
		}
	}
}
