using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.ActionPlanning
{
	partial class ActionPlanner<TState, TAction>
	{
		private class Explorer
		{
			private class StatesCache
			{
				private readonly States _states;

				private readonly List<TState> _cache = new List<TState>();

				public StatesCache(States states) => _states = states;

				public TState Get(TState state = null)
				{
					// Return state
					return _states.Get(state);
				}
				
				public TState GetAndCache(TState state = null)
				{
					// Get new state
					var stateNew = _states.Get(state);
					// Add new state
					_cache.Add(stateNew);
					// Return new state
					return stateNew;
				}

				public void ReleaseCache()
				{
					// Get states
					var states = _states;
					// Get cache
					var cache = _cache;
					// Run through cache in reverse
					for (int i = cache.Count - 1; i >= 0; i--)
					{
						// Release state
						states.Release(cache[i]);
						// Remove state
						cache.RemoveAt(i);
					}
				}
			}

			private class ActionPointsCache
			{
				private readonly Reusable<ActionPoint> _actionPoints = new Reusable<ActionPoint>(actionPoint =>
				{
					// Clear references
					actionPoint.previous = null;
					actionPoint.state = null;
					actionPoint.action = default;
				});
				private readonly List<ActionPoint> _cache = new List<ActionPoint>();

				public ActionPoint GetAndCache
				(
					ActionPoint previous,
					TState state,
					TAction action,
					float costKnown,
					float costEstimated
				)
				{
					// Get action point
					var actionPoint = _actionPoints.GetOrCreate();
					// Set action point
					actionPoint.previous = previous;
					actionPoint.state = state;
					actionPoint.action = action;
					actionPoint.costKnown = costKnown;
					actionPoint.costEstimated = costEstimated;
					// Add action point
					_cache.Add(actionPoint);
					// Return action point
					return actionPoint;
				}

				public void ReleaseCache()
				{
					// Get cache
					var cache = _cache;
					// Get action points
					var actionPoints = _actionPoints;
					// Run through cache in reverse
					for (int i = cache.Count - 1; i >= 0; i--)
					{
						// Return action point to pool
						actionPoints.ReturnToPool(cache[i]);
						// Remove action point
						cache.RemoveAt(i);
					}
				}
			}

			private class ActionPoint
			{
				public ActionPoint previous;
				public TState state;
				public TAction action;
				public float costKnown;
				public float costEstimated;
			}

			private IGoal<TState> _goal;

			private readonly Steps _steps;
			private readonly StatesCache _statesCache;
			private readonly IReadOnlyList<TAction> _actions;
			
			private readonly ActionPointsCache _actionPointsCache = new ActionPointsCache();
			private readonly List<ActionPoint> _actionPoints = new List<ActionPoint>();

			public Explorer(Steps steps, States states, IReadOnlyList<TAction> actions)
			{
				// Set steps
				_steps = steps;
				// Set states cache
				_statesCache = new StatesCache(states);
				// Set actions
				_actions = actions;
			}

			public void PopulatePlan(Plan plan, TState state, IGoal<TState> goal)
			{
				// Set goal
				_goal = goal;
				// Reset cache
				_ResetCache();
				// Get action points
				var actionPoints = _actionPoints;
				// Add action point
				actionPoints.Add(_actionPointsCache.GetAndCache(previous: null, state, action: default, costKnown: 0, costEstimated: 0));
				// Create cycles
				var cycles = 0;
				// Search action points
				while (actionPoints.Count > 0)
				{
					// Increment cycles
					if (cycles++ > 1000)
					{
						// Throw error
						throw new InvalidOperationException("Too many cycles.");
					}
					// Get action point
					var actionPoint = actionPoints[0];
					// Remove action point
					actionPoints.RemoveAt(0);
					// Check if goal is achieved
					if (goal.IsAchieved(actionPoint.state))
					{
						// Populate steps
						_PopulateSteps(plan, actionPoint);
						// Set cycles
						plan.cycles = cycles;
						// Stop loop
						break;
					}
					// Add action points
					_AddActionPoints(actionPoint);
				}
				// Clear action points
				actionPoints.Clear();
				// Reset cache
				_ResetCache();
			}

			private void _AddActionPoints(ActionPoint actionPoint)
			{
				// Get state
				var state = actionPoint.state;
				// Get actions
				var actions = _actions;
				// Get count
				var count = actions.Count;
				// Run through actions
				for (int i = 0; i < count; i++)
				{
					// Add action point
					_AddActionPoint(actionPoint, state, actions[i]);
				}
			}

			private void _AddActionPoint(ActionPoint previous, TState state, TAction action)
			{
				// Check if action is not valid
				if (!action.IsValid(state))
				{
					// Cannot perform action
					return;
				}
				// Get known cost
				var costKnown = previous.costKnown + action.GetCost(state);
				// Get new state
				var stateNew = _statesCache.GetAndCache(state);
				// Update new state
				action.UpdateState(stateNew);
				// Get estimated cost
				var costEstimated = costKnown + _goal.EstimateProximity(stateNew);
				// Get action point
				var actionPoint = _actionPointsCache.GetAndCache(previous, stateNew, action, costKnown, costEstimated);
				// Get action points
				var actionPoints = _actionPoints;
				// Get count
				var count = actionPoints.Count;
				// Run through action points
				for (int i = 0; i < count; i++)
				{
					// Check if estimated cost is less than or equal to action point
					if (costEstimated <= actionPoints[i].costEstimated)
					{
						// Insert action point
						actionPoints.Insert(i, actionPoint);
						// Action point added
						return;
					}
				}
				// Add action point
				actionPoints.Add(actionPoint);
			}

			private void _PopulateSteps(Plan plan, ActionPoint actionPoint)
			{
				// Get reusable steps
				var stepsReusable = _steps;
				// Get steps
				var steps = plan.steps;
				// Add steps
				while (true)
				{
					// Get previous
					var previous = actionPoint.previous;
					// Check if previous does not exist
					if (previous == null)
					{
						// No more steps
						break;
					}
					// Get step
					var step = stepsReusable.Get
						(
							from: _statesCache.Get(previous.state),
							to: _statesCache.Get(actionPoint.state),
							action: actionPoint.action
						);
					// Add step
					steps.Add(step);
					// Set action point
					actionPoint = previous;
				}
				// Reverse steps
				steps.Reverse();
			}

			private void _ResetCache()
			{
				// Release states cache
				_statesCache.ReleaseCache();
				// Release action points cache
				_actionPointsCache.ReleaseCache();
			}
		}
	}
}
