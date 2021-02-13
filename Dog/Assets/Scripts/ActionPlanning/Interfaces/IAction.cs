namespace Assets.Scripts.ActionPlanning.Interfaces
{
	public interface IAction<in TState>
	{
		bool IsValid(TState state);

		void UpdateState(TState state);

		float GetCost(TState state);
	}
}
