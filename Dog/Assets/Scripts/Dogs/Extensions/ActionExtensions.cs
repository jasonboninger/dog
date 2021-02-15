using Assets.Scripts.ActionPlanning;
using Assets.Scripts.Dogs.Models;

namespace Assets.Scripts.Dogs.Extensions
{
	public static class ActionExtensions
	{
		public static TAction InitializeAndAddAndReturn<TAction>(this TAction action, ActionPlanner<State, UAction> planner, Dog dog)
		where TAction : UAction, IAction
		{
			// Initialize action
			action.Initialize(dog);
			// Add action
			planner.AddAction(action);
			// Return action
			return action;
		}
	}
}
