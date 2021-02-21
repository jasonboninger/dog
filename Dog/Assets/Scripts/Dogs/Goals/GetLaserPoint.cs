using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs.Goals
{
	public class GetLaserPoint : IGoal<Dog>
	{
		public float CostEffort => 0;

		public float CostLimit => 100;

		public float EstimateProximity(Dog state) => state.LaserPointer.On && state.LaserPointer.Visible
			? Vector2.Distance(state.Position, state.LaserPointer.Position)
			: 0;

		public float IsAchieved(Dog state) => state.LaserPointer.On && state.LaserPointer.Visible ? 0 : 1;
	}
}
