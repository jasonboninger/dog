using Assets.Scripts.ActionManagement.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.ActionManagement
{
	public abstract class UActionStateMachineGeneric<TActionState> : MonoBehaviour, IActionState
	where TActionState : IActionState
	{
		private TActionState _actionCurrent;
		private TActionState _actionNext;
		public TActionState Action
		{
			get
			{
				// Return next action
				return _actionNext;
			}
			set
			{
				// Check if action does not exist
				if (value == null)
				{
					// Throw error
					throw new InvalidOperationException("Action cannot be null.");
				}
				// Check if executing
				if (_executing)
				{
					// Set next action
					_actionNext = value;
					// Check if action changed
					if (!_actionCurrent.Equals(value))
					{
						// Set cancel
						_cancel = true;
					}
				}
				else
				{
					// Set next action
					_actionNext = value;
					// Set current action
					_actionCurrent = value;
				}
			}
		}

		private TActionState _actionDefault;
		public TActionState ActionDefault
		{
			get
			{
				// Return default action
				return _actionDefault;
			}
			set
			{
				// Set default action
				_actionDefault = value;
				// Check if default action exists and action does not exist
				if (value != null && Action == null)
				{
					// Set action
					Action = value;
				}
			}
		}

		private bool _cancel;
		private bool _executing;

		public float GetTransitionTime() => _actionCurrent.GetTransitionTime();

		public void Enter(float transitionTime) => _actionCurrent.Enter(transitionTime);

		public IEnumerator Execute(Func<bool> cancelled)
		{
			// Set executing
			_executing = true;
			// Set wrapped cancelled
			var cancelledWrapped = _WrapCancelled(cancelled);
			// Loop forever
			while (true)
			{
				// Execute current action
				yield return _actionCurrent.Execute(cancelledWrapped);
				// Check if cancelled from outside
				if (cancelled())
				{
					// Stop loop
					break;
				}
				// Check if cancelled from inside
				if (_cancel)
				{
					// Set not cancel
					_cancel = false;
				}
				else
				{
					// Check if default action does not exist
					if (_actionDefault == null)
					{
						// Stop loop
						break;
					}
					// Set next action
					_actionNext = _actionDefault;
				}
				// Get transition time
				var transitionTime = _actionNext.GetTransitionTime();
				// Exit current action
				_actionCurrent.Exit(transitionTime);
				// Set current action
				_actionCurrent = _actionNext;
				// Enter current action
				_actionCurrent.Enter(transitionTime);
			}
			// Set not executing
			_executing = false;
		}

		public void Exit(float transitionTime) => _actionCurrent.Exit(transitionTime);

		private Func<bool> _WrapCancelled(Func<bool> cancelled)
		{
			// Return wrapped cancelled
			return () => _cancel || cancelled();
		}
	}
}
