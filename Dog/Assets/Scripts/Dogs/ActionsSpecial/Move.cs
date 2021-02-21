using Assets.Scripts.ActionManagement;
using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Dogs.ActionsSpecial
{
	public class Move : IDogAction
	{
		private readonly IDogActionDestination _actionDestination;
		private readonly IPlan<Dog, IDogActionMovement> _plan;
		private readonly ActionPlanner<Dog, IDogActionMovement> _actionPlanner;
		private readonly ActionStateMachine<Dog, IDogActionMovement, float> _actionStateMachine;

		public Move(GameObject gameObject, IReadOnlyList<IDogActionMovement> actionsMovement, IDogActionDestination actionDestination)
		{
			// Set action planner
			_actionPlanner = new ActionPlanner<Dog, IDogActionMovement>();
			// Set action state machine
			_actionStateMachine = new ActionStateMachine<Dog, IDogActionMovement, float>();
			// Run through movement actions
			for (int i = 0; i < actionsMovement.Count; i++)
			{
				// Create movement action
				var actionMovement = actionsMovement[i].Create(gameObject, actionDestination);
				// Add movement action
				_actionPlanner.AddAction(actionMovement);
			}
			// Set destination action
			_actionDestination = actionDestination;
			// Get plan
			_plan = _actionPlanner.GetPlan();
			// Set plan
			_actionPlanner.PopulatePlan(_plan, _actionPlanner.Actions[0]);
			// Subscribe to plan completed
			_actionStateMachine.PlanCompleted_.AddListener(() => _actionStateMachine.CancelAction());
		}

		public bool IsValid(Dog state) => _actionDestination.IsTraversable(state) && !_actionDestination.IsReached(state);

		public float GetCost(Dog state) => 1 + Vector2.Distance(state.Position, _actionDestination.GetPosition(state));

		public void UpdateState(Dog state) => state.Position = _actionDestination.GetPosition(state);

		public float GetTransitionIn()
		{
			// Set plan
			_actionStateMachine.Plan = _plan;
			// Return in transition
			return _actionStateMachine.GetTransitionIn();
		}

		public IEnumerator ExecuteAction(float transitionIn, Func<float?> getTransitionOut)
		{
			// Execute action
			yield return _actionStateMachine.ExecuteAction(transitionIn, getTransitionOut);
		}
	}
}
