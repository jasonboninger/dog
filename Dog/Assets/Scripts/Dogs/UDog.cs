using Assets.Scripts.ActionManagement;
using Assets.Scripts.ActionPlanning;
using Assets.Scripts.ActionPlanning.Enums;
using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.ActionsSpecial;
using Assets.Scripts.Dogs.Goals;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.Models;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Dogs
{
	public class UDog : MonoBehaviour
	{
		[SerializeField] private Transform _actionsContainer = default;
		[SerializeField] private Transform _actionsMovementContainer = default;

		private Animator _animator;
		private ActionPlanner<Dog, IDogAction> _actionPlanner;
		private ActionStateMachine<Dog, IDogAction, float> _actionStateMachine;
		private Dog _state;
		private IPlan<Dog, IDogAction> _planActive;
		private IPlan<Dog, IDogAction> _planTest;
		private IGoal<Dog, IDogAction> _goal;
		private GetLaserPoint _goalGetLaserPoint;
		private SearchForLaserPoint _goalSearchForLaserPoint;
		private HangOut _goalHangOut;
		private UDogOwner _owner;
		private Controls _controls;
		private IDogAction _actionDefault;
		private Vector2 _position;

		private readonly Dictionary<Type, IDogAction> _actionsStandard = new Dictionary<Type, IDogAction>();
		private readonly Dictionary<Type, IDogAction> _actionsMovement = new Dictionary<Type, IDogAction>();

		protected void Awake()
		{
			// Set animator
			_animator = GetComponentInChildren<Animator>();
			// Set action planner
			_actionPlanner = new ActionPlanner<Dog, IDogAction>();
			// Set state
			_state = _actionPlanner.GetState();
			_state.Position = new Vector2(_animator.transform.position.x, _animator.transform.position.z);
			_state.Speed = new Vector2(0, 0);
			// Set active plan
			_planActive = _actionPlanner.GetPlan();
			// Set test plan
			_planTest = _actionPlanner.GetPlan();
			// Set action state machine
			_actionStateMachine = new ActionStateMachine<Dog, IDogAction, float>();
		}

		public UDog Initialize(UDogOwner owner)
		{
			// Set owner
			_owner = owner;
			// Set controls
			_controls = new Controls(_state, _animator.transform, _animator, _owner, new Looker());
			// Return self
			return this;
		}

		protected void Start()
		{
			// Initialize actions
			_InitializeActions();
			// Initialize goals
			_InitializeGoals();
			// Set state
			_SetState();
			// Set goal
			_SetGoal();
			// Set plan
			_SetPlan();
			// Subscribe to plan completed
			_actionStateMachine.PlanCompleted_.AddListener(() => _SetPlan());
			// Get in transition
			var transitionIn = _actionStateMachine.GetTransitionIn();
			// Execute actions
			StartCoroutine(_actionStateMachine.ExecuteAction(transitionIn, getTransitionOut: () => null, setTransitionOut: transitionOut => { }));
		}

		protected void Update()
		{
			// Set state
			_SetState();
			// Set goal
			_SetGoal();
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
			var actions = _actionsContainer.GetComponentsInChildren<UDogActionStandard>();
			// Add and initialize actions
			for (int i = 0; i < actions.Length; i++)
			{
				var action = actions[i];
				// Get global
				var global = action.Global;
				// Get type
				var type = action.GetType();
				// Initialize action
				action.Initialize(_controls);
				// Check if global
				if (global)
				{
					// Add global standard action
					_actionPlanner.AddAction(action);
				}
				else
				{
					// Add standard action
					_actionsStandard.Add(type, action);
				}
				// Check if destination action
				if (action is IDogActionDestination actionDestination)
				{
					// Create movement action
					var actionMovement = new Move(action.gameObject, actionsMovement, actionDestination);
					// Check if global
					if (global)
					{
						// Add global movement action
						_actionPlanner.AddAction(actionMovement);
					}
					else
					{
						// Add movement action
						_actionsMovement.Add(type, actionMovement);
					}
				}
			}
		}

		private void _InitializeGoals()
		{
			// Create goals
			var goals = new Goal[]
			{
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
				_goalGetLaserPoint = new GetLaserPoint(),
				_goalSearchForLaserPoint = new SearchForLaserPoint(),
				_goalHangOut = new HangOut(),
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
			};
			// Run through goals
			for (int i = 0; i < goals.Length; i++)
			{
				// Initialize goal
				goals[i].Initialize(_actionsStandard, _actionsMovement);
			}
		}

		private void _SetState()
		{
			// Get position
			var position = new Vector2(_animator.transform.position.x, _animator.transform.position.z);
			// Set position
			_state.Position = position;
			// Set speed
			_state.Speed = position - _position;
			// Set position
			_position = position;
			// Set laser pointer position
			_state.LaserPointer.Position = _owner.LaserPointer.Position.HasValue
				? new Vector2(_owner.LaserPointer.Position.Value.x, _owner.LaserPointer.Position.Value.z)
				: default;
		}

		private void _SetGoal()
		{
			// Check if laser pointer is on and position exists
			if (_owner.LaserPointer.On && _owner.LaserPointer.Position.HasValue)
			{
				// Set goal
				_goal = _goalGetLaserPoint;
			}
			else
			{
				// Set goal
				_goal = _goalHangOut;
			}
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
	}
}
