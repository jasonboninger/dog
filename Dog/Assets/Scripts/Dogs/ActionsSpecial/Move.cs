using Assets.Scripts.ActionManagement;
using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.ActionsMovement;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs.ActionsSpecial
{
	public class Move : IDogAction
	{
		private readonly IDogMovement _movement;
		private readonly IPlan<Dog, IDogAction> _plan;
		private readonly ActionPlanner<Dog, IDogAction> _actionPlanner = new ActionPlanner<Dog, IDogAction>();
		private readonly ActionStateMachine<Dog, IDogAction, float> _actionStateMachine = new ActionStateMachine<Dog, IDogAction, float>();

		public Move(MonoBehaviour monoBehaviour, Controls controls, IDogMovement movement)
		{
			// Set movement
			_movement = movement;
			// Create action
			var walk = monoBehaviour.gameObject.AddComponent<UWalk>();
			// Initialize action
			walk.Initialize(controls);
			// Set movement
			walk.Movement = _movement;
			// Get plan
			_plan = _actionPlanner.GetPlan();
			// Set plan
			_actionPlanner.PopulatePlan(_plan, walk);
		}

		public bool IsValid(Dog state) => true;

		public float GetCost(Dog state) => Vector2.Distance(state.Position, _movement.GetDestination(state));

		public void UpdateState(Dog state) => state.Position = _movement.GetDestination(state);

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
