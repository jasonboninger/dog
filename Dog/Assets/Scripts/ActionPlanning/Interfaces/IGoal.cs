namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IGoal<in TState>
	{
		bool IsAchieved(TState state);

		float EstimateProximity(TState state);
	}
}
