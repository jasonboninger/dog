using Assets.Scripts.Dogs.States;

namespace Assets.Scripts.Dogs.Goals
{
	public class SearchForLaserPoint : Goal
	{
		public override bool IsAchieved(Dog state) => state.LaserPointer.Never;
		
		public override float EstimateProximity(Dog state) => 0;
	}
}
