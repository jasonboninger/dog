using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Interfaces;
using Assets.Scripts.Dogs.States;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Dogs.Goals
{
	public abstract class Goal : IGoal<Dog, IDogAction>
	{
		private List<IDogAction> _actions;
		public IReadOnlyList<IDogAction> Actions => _actions ?? throw new InvalidOperationException("Goal has not been initialized.");

		public abstract bool IsAchieved(Dog state);
		
		public abstract float EstimateProximity(Dog state);

		private readonly Type[] _actionTypes;

		protected Goal() => _actionTypes = Array.Empty<Type>();
		protected Goal(params Type[] actionTypes) => _actionTypes = actionTypes;

		public void Initialize(params IReadOnlyDictionary<Type, IDogAction>[] actionTypeToActionMappings)
		{
			// Create actions
			_actions = new List<IDogAction>();
			// Run through action types
			for (int i = 0; i < _actionTypes.Length; i++)
			{
				var actionType = _actionTypes[i];
				// Run through action type to action mappings
				for (int k = 0; k < actionTypeToActionMappings.Length; k++)
				{
					// Try to get action
					if (actionTypeToActionMappings[k].TryGetValue(actionType, out var action))
					{
						// Add action
						_actions.Add(action);
					}
				}
			}
		}
	}
}
