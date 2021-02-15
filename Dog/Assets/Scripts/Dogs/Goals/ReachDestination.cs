using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.Models;
using UnityEngine;

namespace Assets.Scripts.Dogs.Goals
{
	public class ReachDestination : IGoal<State>
	{
		public float CostEffort => 0;

		public float CostLimit => 100;

		public float EstimateProximity(State state)
		{
			// Get destination
			var destination = state.Destination;
			// Return distance
			return destination.HasValue ? Vector2.Distance(state.Position, destination.Value) : 0;
		}

		public float IsAchieved(State state) => state.Destination.HasValue ? 0 : 1;
	}
}
