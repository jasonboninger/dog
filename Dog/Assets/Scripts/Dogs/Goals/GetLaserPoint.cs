using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs.Goals
{
	public class GetLaserPoint : IGoal<Dog>
	{
		public bool IsAchieved(Dog state) => state.LaserPointer.Never;
		
		public float EstimateProximity(Dog state) => Vector2.Distance(state.Position, state.LaserPointer.Position);
	}
}
