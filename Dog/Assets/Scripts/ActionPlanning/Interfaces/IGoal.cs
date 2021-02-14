namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IGoal<in TState>
	{
		float CostEffort { get; }
		float CostLimit { get; }

		float IsAchieved(TState state);

		float EstimateProximity(TState state);
	}
}
