using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Actions;
using Assets.Scripts.Dogs.Goals;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(UDogOwner))]
	[RequireComponent(typeof(UDogActions))]
	public class UDog : MonoBehaviour
	{
		[SerializeField] private Transform _actionsContainer = default;

		private ActionPlanner<Dog, IDogAction> _planner;
		private Dog _state;
		private IPlan<Dog, IDogAction> _plan;
		private int _step;
		private Controls _controls;
		private IGoal<Dog> _goal;
		private UDogOwner _owner;
		private UDogActions _actions;
		private IDogAction _actionDefault;
		private Vector2 _position;

		protected void Awake()
		{
			// Set planner
			_planner = new ActionPlanner<Dog, IDogAction>();
			// Set state
			_state = _planner.GetState();
			_state.Position = new Vector2(transform.position.x, transform.position.z);
			_state.Speed = new Vector2(0, 0);
			// Set plan
			_plan = _planner.GetPlan();
			// Set controls
			_controls = new Controls(_state, transform, GetComponent<Animator>(), new Looker());
			// Set goal
			_goal = new GetLaserPoint();
			// Set owner
			_owner = GetComponent<UDogOwner>();
			// Set actions
			_actions = GetComponent<UDogActions>();
			// Subscribe to click
			_owner.Click_.AddListener(_SetLaserPointer);
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
			_owner.Click_.RemoveListener(_SetLaserPointer);
		}

		protected void Update()
		{
			// Set state
			_SetState();
			// Set action
			_SetAction();
		}

		private void _InitializeActions()
		{
			// Set default action
			_actionDefault = _actionsContainer.GetComponentInChildren<UIdle>();
			// Initialize default action
			_actionDefault.Initialize(_controls);
			// Add actions
			_planner.AddAction(_actionsContainer.GetComponentInChildren<UMove>());
			// Initialize actions
			for (int i = 0; i < _planner.Actions.Count; i++)
			{
				// Initialize action
				_planner.Actions[i].Initialize(_controls);
			}
		}

		private IEnumerator _ExecuteActions()
		{
			// Wait for update
			yield return null;
			// Enter actions
			_actions.Enter(_actions.GetTransitionTime());
			// Loop forever
			while (true)
			{
				// Execute actions
				yield return _actions.Execute(() => false);
				// Get action
				var action = _GetAction();
				// Get transition time
				var transitionTime = action.GetTransitionTime();
				// Exit actions
				_actions.Exit(transitionTime);
				// Set action
				_actions.Action = action;
				// Enter actions
				_actions.Enter(transitionTime);
			}
		}

		private void _SetState()
		{
			// Get position
			var position = new Vector2(transform.position.x, transform.position.z);
			// Set position
			_state.Position = position;
			// Set speed
			_state.Speed = position - _position;
			// Set position
			_position = position;
		}

		private IDogAction _GetAction()
		{
			// Increment step
			_step++;
			// Check if step exists
			if (_step < _plan.Steps.Count)
			{
				// Return action
				return _plan.Steps[_step].Action;
			}
			// Populate plan
			_planner.PopulatePlan(_plan, _state, _goal);
			// Return action
			return _plan.Success && _plan.Steps.Count > 0 ? _plan.Steps[0].Action : _actionDefault;
		}

		private void _SetAction()
		{
			// Populate plan
			_planner.PopulatePlan(_plan, _state, _goal);
			// Get action
			var action = _plan.Success && _plan.Steps.Count > 0 ? _plan.Steps[0].Action : _actionDefault;
			// Check if action changed
			if (!action.Equals(_actions.Action))
			{
				// Set step
				_step = 0;
				// Set action
				_actions.Action = action;
			}
		}

		private void _SetLaserPointer(Vector3 position)
		{
			// Set laser pointer
			_state.LaserPointer.On = true;
			_state.LaserPointer.Position = new Vector2(position.x, position.z);
		}
	}
}
