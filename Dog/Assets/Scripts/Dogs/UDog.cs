using Assets.Scripts.ActionManagement;
using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Enums;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.ActionsSpecial;
using Assets.Scripts.Dogs.Goals;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(UDogOwner))]
	public class UDog : MonoBehaviour
	{
		[SerializeField] private Transform _actionsContainer = default;
		[SerializeField] private Transform _actionsMovementContainer = default;

		private ActionPlanner<Dog, IDogAction> _actionPlanner;
		private ActionStateMachine<Dog, IDogAction, float> _actionStateMachine;
		private Dog _state;
		private IPlan<Dog, IDogAction> _planActive;
		private IPlan<Dog, IDogAction> _planTest;
		private Controls _controls;
		private IGoal<Dog> _goal;
		private UDogOwner _owner;
		private IDogAction _actionDefault;
		private Vector2 _position;

		protected void Awake()
		{
			// Set action planner
			_actionPlanner = new ActionPlanner<Dog, IDogAction>();
			// Set state
			_state = _actionPlanner.GetState();
			_state.Position = new Vector2(transform.position.x, transform.position.z);
			_state.Speed = new Vector2(0, 0);
			// Set active plan
			_planActive = _actionPlanner.GetPlan();
			// Set test plan
			_planTest = _actionPlanner.GetPlan();
			// Set action state machine
			_actionStateMachine = new ActionStateMachine<Dog, IDogAction, float>();
			// Set controls
			_controls = new Controls(_state, transform, GetComponent<Animator>(), new Looker());
			// Set goal
			_goal = new GetLaserPoint();
			// Set owner
			_owner = GetComponent<UDogOwner>();
			// Subscribe to point
			_owner.Point_.AddListener(_AimLaserPointer);
			// Subscribe to click
			_owner.Click_.AddListener(_ToggleLaserPointer);
		}

		protected void Start()
		{
			// Initialize actions
			_InitializeActions();
			// Set state
			_SetState();
			// Set plan
			_SetPlan();
			// Subscribe to plan completed
			_actionStateMachine.PlanCompleted_.AddListener(() => _SetPlan());
			// Get in transition
			var transitionIn = _actionStateMachine.GetTransitionIn();
			// Execute actions
			StartCoroutine(_actionStateMachine.ExecuteAction(transitionIn, getTransitionOut: () => null, setTransitionOut: transitionOut => { }));
		}

		protected void OnDestroy()
		{
			// Unsubscribe from point
			_owner.Point_.RemoveListener(_AimLaserPointer);
			// Unsubscribe from click
			_owner.Click_.RemoveListener(_ToggleLaserPointer);
		}

		protected void Update()
		{
			// Set state
			_SetState();
			// Set plan
			_SetPlan();
		}

		private void _InitializeActions()
		{
			// Set default action
			_actionDefault = new Idle();
			// Get movement actions
			var actionsMovement = _actionsMovementContainer.GetComponentsInChildren<UDogActionMovement>();
			// Run through movement actions
			for (int i = 0; i < actionsMovement.Length; i++)
			{
				var actionMovement = actionsMovement[i];
				// Initialize movement action
				actionMovement.Initialize(_controls);
			}
			// Get actions
			var actions = _actionsContainer.GetComponentsInChildren<UDogAction>();
			// Add and initialize actions
			for (int i = 0; i < actions.Length; i++)
			{
				var action = actions[i];
				// Initialize action
				action.Initialize(_controls);
				// Add action
				_actionPlanner.AddAction(action);
				// Check if destination action
				if (action is IDogActionDestination actionDestination)
				{
					// Create movement action
					var actionMovement = new Move(action.gameObject, actionsMovement, actionDestination);
					// Add movement action
					_actionPlanner.AddAction(actionMovement);
				}
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

		private void _SetPlan()
		{
			// Set test plan
			_actionPlanner.PopulatePlan(_planTest, _state, _goal);
			// Check if test plan is not valid
			if (_planTest.Outcome != EPlanningOutcome.Success || _planTest.Steps.Count == 0)
			{
				// Set test plan
				_actionPlanner.PopulatePlan(_planTest, _actionDefault);
			}
			// Set plan
			_actionStateMachine.Plan = _planTest;
			// Get active plan
			var planActive = _planActive;
			// Set active plan
			_planActive = _planTest;
			// Set test plan
			_planTest = planActive;
		}

		private void _AimLaserPointer(Vector3? position)
		{
			// Check if position exists
			if (position.HasValue)
			{
				// Set laser pointer visible
				_state.LaserPointer.Visible = true;
				// Set laser pointer position
				_state.LaserPointer.Position = new Vector2(position.Value.x, position.Value.z);
			}
			else
			{
				// Set laser pointer not visible
				_state.LaserPointer.Visible = false;
			}
		}

		private void _ToggleLaserPointer()
		{
			// Set laser pointer on/off
			_state.LaserPointer.On = !_state.LaserPointer.On;
		}
	}
}
