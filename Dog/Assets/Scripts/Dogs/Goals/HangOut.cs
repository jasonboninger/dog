using Assets.Scripts.ActionPlanning.Interfaces;
using Assets.Scripts.Dogs.States;

namespace Assets.Scripts.Dogs.Goals
{
	public class HangOut : IGoal<Dog>
	{
		public bool IsAchieved(Dog state) => state.Chilled;

		public float EstimateProximity(Dog state) => 0;
	}
}
