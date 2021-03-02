using Assets.Scripts.Dogs.Actions;
using Assets.Scripts.Dogs.States;
using UnityEngine;

namespace Assets.Scripts.Dogs.Goals
{
	public class GetLaserPoint : Goal
	{
		public GetLaserPoint()
		: base
		(
			typeof(UJumpOnLaserPoint)
		)
		{ }

		public override bool IsAchieved(Dog state) => state.LaserPointer.Never;
		
		public override float EstimateProximity(Dog state) => Vector2.Distance(state.Position, state.LaserPointer.Position);
	}
}
