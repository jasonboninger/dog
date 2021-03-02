using Assets.Scripts.Dogs.States;

namespace Assets.Scripts.Dogs.Goals
{
	public class HangOut : Goal
	{
		public override bool IsAchieved(Dog state) => state.Chilled;

		public override float EstimateProximity(Dog state) => 0;
	}
}
