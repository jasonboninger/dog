using Assets.Scripts.ActionManagement.Interfaces;
using Assets.Scripts.ActionPlanning.Interfaces;
using BoningerWorks.Events;
using System;
using System.Collections;

namespace Assets.Scripts.ActionManagement
{
	public class ActionStateMachine<TState, TActionState, TTransition> : IActionState<TTransition>
	where TState : class, IState<TState>
	where TActionState : class, IAction<TState>, IActionState<TTransition>
	where TTransition : struct
	{
		private readonly BwEvent _planCompleted_ = new BwEvent();
		public IBwEvent PlanCompleted_ => _planCompleted_;

		private IPlan<TState, TActionState> _plan;
		private int _planIndex;
		public IPlan<TState, TActionState> Plan
		{
			get
			{
				// Return plan
				return _plan;
			}
			set
			{
				// Get action
				var action = Action;
				// Set plan
				_plan = value;
				// Set plan index
				_planIndex = 0;
				// Check if executing and action changed
				if (_executing && !Equals(Action, action))
				{
					// Set internal out transition
					_transitionOutInternal = Action?.GetTransitionIn() ?? default;
				}
			}
		}

		public TActionState Action
		{
			get
			{
				// Return action
				return _plan != null && _plan.Steps.Count > _planIndex ? _plan.Steps[_planIndex].Action : null;
			}
		}

		private bool _executing;
		private bool _cancel;
		private TTransition? _transitionOutExternal;
		private TTransition? _transitionOutInternal;
		private TTransition? _transitionOutAction;

		private readonly Action<TTransition> _setTransitionOut;

		public ActionStateMachine() => _setTransitionOut = transitionOut => _transitionOutAction = transitionOut;
		public ActionStateMachine(IPlan<TState, TActionState> plan) : this() => Plan = plan;

		public TTransition GetTransitionIn() => Action?.GetTransitionIn() ?? default;

		public IEnumerator ExecuteAction(TTransition transitionIn, Func<TTransition?> getTransitionOut, Action<TTransition> setTransitionOut)
		{
			// Set executing
			_executing = true;
			// Get wrapped get out transition
			var getTransitionOutWrapped = _GetTransitionOutWrapped(getTransitionOut);
			// Create synchronous cycles
			var synchronousCycles = 0;
			// Loop forever
			while (true)
			{
				// Get enumerator
				var enumerator = Action?.ExecuteAction(transitionIn, getTransitionOutWrapped, _setTransitionOut);
				// Check if enumerator exists or synchronous cycles is too much
				if (enumerator != null || synchronousCycles++ > 100)
				{
					// Reset synchronous cycles
					synchronousCycles = 0;
					// Execute action
					yield return enumerator;
				}
				// Check if external out transition or cancel
				if (_transitionOutExternal.HasValue || _cancel)
				{
					// Stop loop
					break;
				}
				// Check if internal out transition
				if (_transitionOutInternal.HasValue)
				{
					// Set in transition
					transitionIn = _transitionOutInternal.Value;
					// Clear transitions
					_ClearTransitions();
				}
				else
				{
					// Increment plan index
					_planIndex++;
					// Check if action does not exist
					if (Action == null)
					{
						// Emit plan completed
						_planCompleted_.Invoke();
					}
					// Get action
					var action = Action;
					// Check if action does not exist
					if (action == null)
					{
						// Check if action out transition exists
						if (_transitionOutAction.HasValue)
						{
							// Set out transition
							setTransitionOut(_transitionOutAction.Value);
						}
						// Stop loop
						break;
					}
					// Set in transition
					transitionIn = _transitionOutAction ?? action.GetTransitionIn();
					// Clear transitions
					_ClearTransitions();
				}
			}
			// Clear transitions
			_ClearTransitions();
			// Set plan index
			_planIndex = 0;
			// Set not cancel
			_cancel = false;
			// Set not executing
			_executing = false;
		}

		public void CancelAction()
		{
			// Set cancel
			_cancel = true;
		}

		private Func<TTransition?> _GetTransitionOutWrapped(Func<TTransition?> getTransitionOut)
		{
			// Return wrapped get out transition
			return () =>
			{
				// Set external out transition
				_transitionOutExternal = getTransitionOut();
				// Return external out transition or internal out transition or cancel
				return _transitionOutExternal ?? _transitionOutInternal ?? (_cancel ? default(TTransition) : (TTransition?)null);
			};
		}

		private void _ClearTransitions()
		{
			// Set no external out transition
			_transitionOutExternal = null;
			// Set no internal out transition
			_transitionOutInternal = null;
			// Set no action out transition
			_transitionOutAction = null;
		}
	}
}
