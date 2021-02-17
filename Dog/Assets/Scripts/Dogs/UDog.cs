using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Actions;
using Assets.Scripts.Dogs.Extensions;
using Assets.Scripts.Dogs.Goals;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(UOwner))]
	[RequireComponent(typeof(UIdle))]
	[RequireComponent(typeof(UWalk))]
	public class UDog : MonoBehaviour
	{
		private ActionPlanner<Dog, UAction> _planner;
		private Dog _state;
		private IPlan<Dog, UAction> _plan;
		private Controls _controls;
		private IGoal<Dog> _goal;
		private UOwner _owner;
		private IAction _action;
		private int _actionIndex;
		private UIdle _actionDefault;
		private bool _cancel;
		private Vector2 _position;

		private readonly Func<bool> _cancelled;

		public UDog() => _cancelled = () => _cancel;

		protected void Awake()
		{
			// Set planner
			_planner = new ActionPlanner<Dog, UAction>();
			// Set state
			_state = _planner.GetState();
			// Set plan
			_plan = _planner.GetPlan();
			// Set controls
			_controls = new Controls(_state, transform, GetComponent<Animator>(), new Looker());
			// Set goal
			_goal = new ReachDestination();
			// Set owner
			_owner = GetComponent<UOwner>();
			// Set state
			_state.Set
				(
					EAction.Idle,
					position: new Vector2(transform.position.x, transform.position.z),
					speed: new Vector2(0, 0),
					destination: null
				);
			// Subscribe to click
			_owner.Click_.AddListener(_SetDestination);
		}

		protected void Start()
		{
			// Initialize actions
			_InitializeActions();
			// Execute actions
			StartCoroutine(_ExecuteActions());
		}

		protected void OnDestroy()
		{
			// Unsubscribe from click
			_owner.Click_.RemoveListener(_SetDestination);
		}

		protected void Update()
		{
			// Update state
			_UpdateState();
			// Set action
			_SetAction();
		}

		private void _InitializeActions()
		{
			// Add default action
			_actionDefault = GetComponent<UIdle>().InitializeAndAddAndReturn(_planner, _controls);
			// Set action
			_action = _actionDefault;
			// Set action index
			_actionIndex = 0;
			// Add other actions
			GetComponent<UWalk>().InitializeAndAddAndReturn(_planner, _controls);
		}

		private IEnumerator _ExecuteActions()
		{
			// Wait for update
			yield return null;
			// Get action
			var action = _action;
			// Get transition time
			var transitionTime = action.GetTransitionTime();
			// Loop forever
			while (true)
			{
				// Enter action
				action.Enter(transitionTime);
				// Execute action
				yield return action.Execute(_cancelled);
				// Check if cancel
				if (_cancel)
				{
					// Set not cancel
					_cancel = false;
				}
				else
				{
					// Increase action index
					_actionIndex++;
					// Check if next action exists
					if (_actionIndex < _plan.Steps.Count)
					{
						// Set action
						_action = _plan.Steps[_actionIndex].Action;
					}
					else
					{
						// Set action
						_SetAction();
					}
				}
				// Set transition time
				transitionTime = _action.GetTransitionTime();
				// Exit action
				action.Exit(transitionTime);
				// Set action
				action = _action;
				// Enter action
				action.Enter(transitionTime);
			}
		}

		private void _UpdateState()
		{
			// Get position
			var position = new Vector2(transform.position.x, transform.position.z);
			// Update position
			_state.Position = position;
			// Update speed
			_state.Speed = position - _position;
			// Set position
			_position = position;
		}

		private void _SetAction()
		{
			// Populate plan
			_planner.PopulatePlan(_plan, _state, _goal);
			// Get action
			var action = _plan.Success && _plan.Steps.Count > 0 ? _plan.Steps[0].Action : _actionDefault;
			// Check if action changed
			if (_action.Id != action.Id)
			{
				// Set action
				_action = action;
				// Set action index
				_actionIndex = 0;
				// Cancel action
				_cancel = true;
			}
		}

		private void _SetDestination(Vector3 destination)
		{
			// Set destination
			_state.Destination = new Vector2(destination.x, destination.z);
		}
	}
}
