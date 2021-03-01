using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.States;

namespace Assets.Scripts.Dogs.Goals
{
	public class SearchForLaserPoint : IGoal<Dog>
	{
		public bool IsAchieved(Dog state) => state.LaserPointer.Never;
		
		public float EstimateProximity(Dog state) => 0;
	}
}
